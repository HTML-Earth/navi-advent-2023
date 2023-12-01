using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HangMan : MonoBehaviour
{
    public Transform wordParent;
    public Transform keyboardParent;
    public HangmanExercise exercise;
    public AudioClip correctLetterSound;
    public AudioClip wrongLetterSound;
    public AudioClip victorySound;
    public AudioClip defeatSound;

    AudioSource _source;
    
    List<string> _words;
    int _fullWordCount;

    string _currentWord;
    string _currentWordTranslation;
    int _currentWordIndex;
    List<(char, bool, TextMeshProUGUI)> _wordToGuess;
    List<char> _guessedChars;
    List<char> _wrongGuesses;
    int _livesLeft;

    TextMeshProUGUI _livesText;
    TextMeshProUGUI _wordCountText;
    TextMeshProUGUI _guessedText;
    
    GameObject _messageParent;
    GameObject _NewWordButton;
    GameObject _GoBackButton;
    TextMeshProUGUI _messageText;

    GameObject _pamrelviPrefab;
    GameObject _keyPrefab;

    List<Button> keys = new List<Button>();

    void Awake()
    {
        _pamrelviPrefab = Resources.Load<GameObject>("Prefabs/pamrelvi");
        _keyPrefab = Resources.Load<GameObject>("Prefabs/key");
        _livesText = transform.Find("lives").GetComponent<TextMeshProUGUI>();
        _wordCountText = transform.Find("wordcount").GetComponent<TextMeshProUGUI>();
        _guessedText = transform.Find("guessed").GetComponent<TextMeshProUGUI>();
        _messageParent = transform.Find("'upxare").gameObject;
        _NewWordButton = transform.Find("'upxare/Restart").gameObject;
        _GoBackButton = transform.Find("'upxare/End").gameObject;
        _messageText = transform.Find("'upxare/pamrel").GetComponent<TextMeshProUGUI>();
        _source = GetComponent<AudioSource>();
        if (!Settings.current.GetSoundEnabled())
            _source.mute = true;

        if (exercise == null || exercise.words.Count < 1)
        {
            Debug.LogError("NO WORDS IN LIST");
            return;
        }

        _words = new List<string>();
        foreach (var word in exercise.words)
        {
            _words.Add(word);
        }

        _fullWordCount = _words.Count;
        
        _currentWordIndex = Random.Range(0, _words.Count);
        InitNewWord(_words[_currentWordIndex]);
        InitKeyboard();
    }

    void InitKeyboard()
    {
        keys = new List<Button>();
        
        var row0 = new List<char>
        {
            'q','w','e','r','t','y','u','i','o','p'
        };
        var row1 = new List<char>
        {
            'a','s','d','f','g','h','ì','k','l'
        };
        var row2 = new List<char>
        {
            'z','ä','c','v','b','n','m'
        };
        var row3 = new List<char>
        {
            'A','Â','E','Ê','L','R','\''
        };

        var t0 = keyboardParent.transform.Find("0");
        var t1 = keyboardParent.transform.Find("1");
        var t2 = keyboardParent.transform.Find("2");
        var t3 = keyboardParent.transform.Find("3");
        
        foreach (var c in row0)
        {
            InitKey(t0, c);
        }

        foreach (var c in row1)
        {
            InitKey(t1, c);
        }

        foreach (var c in row2)
        {
            InitKey(t2, c);
        }

        foreach (var c in row3)
        {
            InitKey(t3, c);
        }
    }

    void InitKey(Transform parent, char c)
    {
        var key = Instantiate(_keyPrefab, parent);
        var text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(RenderPamrelvi(c).ToLower());
        key.transform.Find("pamrel").GetComponent<TextMeshProUGUI>().text = text;
        var button = key.GetComponent<Button>();
        button.onClick.AddListener(() => EnSi(c));
        button.onClick.AddListener(() => button.interactable = false);
        key.name = text;
        keys.Add(button);
    }

    void ResetKeyboard()
    {
        foreach (var key in keys)
        {
            key.interactable = true;
        }
    }

    void DisableKeyboard()
    {
        foreach (var key in keys)
        {
            key.interactable = false;
        }
    }

    void InitNewWord(string newWord)
    {
        _livesLeft = 10;
        _guessedChars = new List<char>();
        _wrongGuesses = new List<char>();
        _wordToGuess = new List<(char, bool, TextMeshProUGUI)>();
        
        foreach (Transform t in wordParent)
        {
            Destroy(t.gameObject);
        }

        var split = newWord.Split(',');
        _currentWord = split[0];
        _currentWordTranslation = split[1];
        
        foreach (var c in _currentWord)
        {
            var p = Instantiate(_pamrelviPrefab, wordParent);
            var txt = p.GetComponent<TextMeshProUGUI>();
            var isSpace = c == ' ';
            _wordToGuess.Add((c, isSpace, txt));
        }
        
        RenderLives();
        RenderWrongGuesses();
        RenderWordCount();
        RenderWord();
        ResetKeyboard();
    }

    void RenderWord()
    {
        foreach (var c in _wordToGuess)
        {
            c.Item3.text = c.Item2 ? RenderPamrelvi(c.Item1) : "_";
        }
    }

    public void EnSi(string pamrelvi)
    {
        var c = pamrelvi switch
        {
            "tx" => 'd',
            "kx" => 'q',
            "px" => 'b',
            
            "ts" => 'c',
            "ng" => 'g',
            
            "aw" => 'A',
            "ay" => 'Â',
            
            "ew" => 'E',
            "ey" => 'Ê',
            
            "ll" => 'L',
            "rr" => 'R',
            
            _ => pamrelvi[0]
        };

        EnSi(c);
    }

    void EnSi(char pamrelvi)
    {
        if (_guessedChars.Contains(pamrelvi))
        {
            Debug.LogWarning($"{RenderPamrelvi(pamrelvi)} was already guessed");
            //return;
        }
        
        _guessedChars.Add(pamrelvi);

        var correctIndices = new List<int>();

        for (var i = 0; i < _wordToGuess.Count; i++)
        {
            if (_wordToGuess[i].Item2)
                continue;

            if (_wordToGuess[i].Item1 == pamrelvi)
            {
                correctIndices.Add(i);
            }
        }

        if (correctIndices.Count < 1)
        {
            _source.clip = wrongLetterSound;
            _source.Play();
            _livesLeft--;
            RenderLives();
            _wrongGuesses.Add(pamrelvi);
            RenderWrongGuesses();
        }
        else
        {
            _source.clip = correctLetterSound;
            _source.Play();
        }

        foreach (var correctIndex in correctIndices)
        {
            var updatedChar = _wordToGuess[correctIndex];
            updatedChar.Item2 = true;
            _wordToGuess[correctIndex] = updatedChar;
        }
        
        RenderWord();
        CheckGuessState();
    }

    void RenderLives()
    {
        _livesText.text = $"Guesses left: {_livesLeft}";
    }

    void RenderWordCount()
    {
        _wordCountText.text = $"Words correctly guessed: {_fullWordCount - _words.Count}/{_fullWordCount}";
    }

    void RenderWrongGuesses()
    {
        if (_wrongGuesses.Count < 1)
        {
            _guessedText.text = "";
        }
        else
        {
            var sb = new StringBuilder();
            sb.Append("Wrong guesses:\n");
            foreach (var c in _wrongGuesses)
            {
                sb.Append(RenderPamrelvi(c));
                sb.Append(" ");
            }
            _guessedText.text = sb.ToString();
        }
    }

    void CheckGuessState()
    {
        if (_livesLeft == 0)
        {
            OnGameEnd(false);
            return;
        }

        var allGuessed = true;
        
        foreach (var c in _wordToGuess)
        {
            if (!c.Item2)
            {
                allGuessed = false;
                break;
            }
        }

        if (allGuessed)
        {
            OnGameEnd(true);
        }
    }

    void OnGameEnd(bool victory)
    {
        DisableKeyboard();
        
        _messageParent.SetActive(true);
        
        var theWord = RenderLiu(_currentWord);
        var firstLine = (victory ? "Seysonìltsan! You guessed it!" : "You ran out of guesses.") + $"\nThe word was \"{theWord}\".\n({_currentWordTranslation})";

        if (victory)
        {
            _source.clip = victorySound;
            _source.Play();
            _words.RemoveAt(_currentWordIndex);
            RenderWordCount();
        }
        else
        {
            _source.clip = defeatSound;
            _source.Play();
        }
        
        if (_words.Count < 1)
        {
            _messageText.text = $"{firstLine}\nYou've identified all of the words!";
            _NewWordButton.SetActive(false);
            _GoBackButton.SetActive(true);
        }
        else
        {
            _NewWordButton.SetActive(true);
            _GoBackButton.SetActive(false);
            _messageText.text = firstLine;
        }
    }

    string RenderPamrelvi(char c)
    {
        return c switch
        {
            'd' => "tx",
            'q' => "kx",
            'b' => "px",
            
            'c' => "ts",
            'g' => "ng",
            
            'A' => "aw",
            'Â' => "ay",
            
            'E' => "ew",
            'Ê' => "ey",
            
            'L' => "ll",
            'R' => "rr",
            
            _ => c.ToString()
        };
    }

    string RenderLiu(string w)
    {
        var sb = new StringBuilder();
        foreach (var c in w)
        {
            sb.Append(RenderPamrelvi(c));
        }

        return sb.ToString();
    }

    public void Restart()
    {
        _messageParent.SetActive(false);
        
         _currentWordIndex = Random.Range(0, _words.Count);
        InitNewWord(_words[_currentWordIndex]);
    }
}
