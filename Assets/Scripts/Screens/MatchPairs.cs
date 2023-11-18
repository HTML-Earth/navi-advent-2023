using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchPairs : MonoBehaviour
{
    public List<MatchPairsExercise> matchPairsExercises;
    public Transform leftParent;
    public Transform rightParent;

    List<(string, string)> _wordPairs;
    WordPairButton[] _wordButtons;
    int _selectedWord;
    int _matchCount;

    bool _gameIsActive;
    int _currentExercise;

    int _triesLeft;
    TextMeshProUGUI _triesText;
    
    GameObject _messageParent;
    TextMeshProUGUI _messageText;

    GameObject _wordButton;

    void Awake()
    {
        _wordButton = Resources.Load<GameObject>("Prefabs/WordPairButton");
        _triesText = transform.Find("Tries").GetComponent<TextMeshProUGUI>();
        _messageParent = transform.Find("'upxare").gameObject;
        _messageText = transform.Find("'upxare/pamrel").GetComponent<TextMeshProUGUI>();

        _currentExercise = -1;
        NewExercise();
    }

    void NewExercise()
    {
        _currentExercise++;
        if (_currentExercise >= matchPairsExercises.Count)
            _currentExercise = 0;
        
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
        foreach (var pair in matchPairsExercises[_currentExercise].wordPairs)
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
    }

    void Fail()
    {
        _triesLeft--;
        UpdateTriesText();
        if (_triesLeft < 1)
        {
            OnGameEnd(false);
        }
    }
    
    void OnGameEnd(bool victory)
    {
        _gameIsActive = false;
        _messageParent.SetActive(true);
        _messageText.text = victory ? "Seysonìltsan!" : "Keftxo.";
    }

    public void Retry()
    {
        _messageParent.SetActive(false);
        NewExercise();
    }

    void UpdateTriesText()
    {
        _triesText.text = $"Tries left: {_triesLeft}";
    }
}
