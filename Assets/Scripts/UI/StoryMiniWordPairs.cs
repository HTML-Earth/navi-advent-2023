using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoryMiniWordPairs : MonoBehaviour, IMatchPairs
{
    public Transform leftParent;
    public Transform rightParent;

    public AudioClip selectSound;
    public AudioClip correctPairSound;
    public AudioClip wrongPairSound;
    public AudioClip victorySound;
    
    List<(string, string)> _wordPairs;
    WordPairButton[] _wordButtons;
    int _selectedWord;
    int _matchCount;

    bool _gameIsActive;
    
    AudioSource _source;
    
    GameObject _wordButton;

    Story _story;
    
    void Awake()
    {
        _wordButton = Resources.Load<GameObject>("Prefabs/WordPairButton_Story");
        _source = GetComponent<AudioSource>();
        if (!Settings.current.GetSoundEnabled())
            _source.mute = true;
    }

    public void Play(StoryWordPairs storyWordPairs, Story story)
    {
        _story = story;
        
        _gameIsActive = true;
        
        _wordPairs = new List<(string, string)>();
        foreach (var pair in storyWordPairs.wordPairs)
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
            _story.Scroll(true);
            _story.DisplayContinueButton();
        }
        else
        {
            _source.clip = correctPairSound;
            _source.Play();
        }
    }

    void Fail()
    {
        _source.clip = wrongPairSound;
        _source.Play();
    }
}