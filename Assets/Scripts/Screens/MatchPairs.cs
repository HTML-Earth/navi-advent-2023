using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchPairs : MonoBehaviour, IMatchPairs
{
    public List<MatchPairsExercise> matchPairsExercises;
    public Transform leftParent;
    public Transform rightParent;

    public AudioClip selectSound;
    public AudioClip correctPairSound;
    public AudioClip wrongPairSound;
    public AudioClip victorySound;

    AudioSource _source;

    List<(string, string)> _wordPairs;
    WordPairButton[] _wordButtons;
    int _selectedWord;
    int _matchCount;
    
    TextMeshProUGUI _countText;

    bool _gameIsActive;
    //int _currentExercise;
    int _exerciseCount;

    int _triesLeft;
    TextMeshProUGUI _triesText;
    
    GameObject _messageParent;
    TextMeshProUGUI _messageText;
    GameObject _nextPairsButton;
    GameObject _restartButton;
    GameObject _quitButton;

    GameObject _wordButton;

    void Awake()
    {
        _wordButton = Resources.Load<GameObject>("Prefabs/WordPairButton");
        _triesText = transform.Find("Tries").GetComponent<TextMeshProUGUI>();
        _countText = transform.Find("Count").GetComponent<TextMeshProUGUI>();
        _messageParent = transform.Find("'upxare").gameObject;
        _messageText = transform.Find("'upxare/pamrel").GetComponent<TextMeshProUGUI>();
        _nextPairsButton = transform.Find("'upxare/NextPairs").gameObject;
        _restartButton = transform.Find("'upxare/Restart").gameObject;
        _quitButton = transform.Find("'upxare/Quit").gameObject;
        _source = GetComponent<AudioSource>();
        if (!Settings.current.GetSoundEnabled())
            _source.mute = true;

        _exerciseCount = matchPairsExercises.Count;
        _countText.text = $"0/{_exerciseCount}";
        //_currentExercise = -1;
        NewExercise();
    }

    void NewExercise()
    {
        // _currentExercise++;
        // if (_currentExercise >= matchPairsExercises.Count)
        //     _currentExercise = 0;
        
        foreach (Transform t in leftParent)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in rightParent)
        {
            Destroy(t.gameObject);
        }

        _selectedWord = -1;
        _matchCount = 0;
        _triesLeft = 3;
        _gameIsActive = true;
        
        UpdateTriesText();

        _wordPairs = new List<(string, string)>();
        foreach (var pair in matchPairsExercises[0].wordPairs)
        {
            var split = pair.Split(',');
            _wordPairs.Add((split[0], split[1]));
        }

        var wordPairCount = _wordPairs.Count;
        
        _wordButtons = new WordPairButton[wordPairCount * 2];

        for (var i = 0; i < _wordPairs.Count; i++)
        {
            var wordPair = _wordPairs[i];
            var leftWord = Instantiate(_wordButton, leftParent);
            var rightWord = Instantiate(_wordButton, rightParent);

            leftWord.transform.Find("pamrel").GetComponent<TextMeshProUGUI>().text = wordPair.Item1;
            rightWord.transform.Find("pamrel").GetComponent<TextMeshProUGUI>().text = wordPair.Item2;

            _wordButtons[i] = leftWord.GetComponent<WordPairButton>();
            _wordButtons[i].Init(i, this);
            
            _wordButtons[i + wordPairCount] = rightWord.GetComponent<WordPairButton>();
            _wordButtons[i + wordPairCount].Init(i + wordPairCount, this);
        }

        var shuffleCount = wordPairCount * 3;
        for (var i = 0; i < shuffleCount; i++)
        {
            _wordButtons[Random.Range(0, wordPairCount)].transform.SetSiblingIndex(0);
        }
        
        matchPairsExercises.RemoveAt(0);
    }

    public void TrySelect(int index)
    {
        if (!_gameIsActive)
            return;
        
        if (_selectedWord != -1)
        {
            var pairCount = _wordPairs.Count;
            var previousSelectionWasLeft = _selectedWord < pairCount;
            var newSelectionIsLeft = index < pairCount;
            
            if (previousSelectionWasLeft != newSelectionIsLeft)
            {
                //check pair
                if (_selectedWord % pairCount == index % pairCount)
                {
                    _wordButtons[index].Match();
                    _wordButtons[_selectedWord].Match();
                    _selectedWord = -1;
                    Succeed();
                    return;
                }
                else
                {
                    _wordButtons[_selectedWord].Fail();
                    _wordButtons[index].Fail();
                    _selectedWord = -1;
                    Fail();
                    return;
                }
            }
            else
            {
                _wordButtons[_selectedWord].Deselect();
            }
        }

        _source.clip = selectSound;
        _source.Play();
        
        _wordButtons[index].Select();
        _selectedWord = index;
    }

    void Succeed()
    {
        _matchCount++;
        if (_matchCount == _wordPairs.Count)
        {
            OnGameEnd(true);
        }
        else
        {
            _source.clip = correctPairSound;
            _source.Play();
        }
    }

    void Fail()
    {
        _triesLeft--;
        UpdateTriesText();
        if (_triesLeft < 1)
        {
            OnGameEnd(false);
        }
        else
        {
            _source.clip = wrongPairSound;
            _source.Play();
        }
    }
    
    void OnGameEnd(bool victory)
    {
        _gameIsActive = false;
        _messageParent.SetActive(true);

        _countText.text = $"{_exerciseCount - matchPairsExercises.Count}/{_exerciseCount}";

        if (victory)
        {
            if (matchPairsExercises.Count < 1)
            {
                _messageText.text = "Wou! You got all of them!";
                _quitButton.SetActive(true);
            }
            else
            {
                _messageText.text = $"Seysonìltsan! {matchPairsExercises.Count} left.";
                _nextPairsButton.SetActive(true);
            }
            
            _source.clip = victorySound;
            _source.Play();
        }
        else
        {
            _messageText.text = "You ran out of tries.\nWant to start over?";
            _restartButton.SetActive(true);
            
            _source.clip = wrongPairSound;
            _source.Play();
        }
    }

    public void Retry()
    {
        // _messageParent.SetActive(false);
        // NewExercise();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void NextPairs()
    {
        _messageParent.SetActive(false);
        _nextPairsButton.SetActive(false);
        _restartButton.SetActive(false);
        _quitButton.SetActive(false);
        NewExercise();
    }

    void UpdateTriesText()
    {
        _triesText.text = $"Tries left: {_triesLeft}";
    }
}
