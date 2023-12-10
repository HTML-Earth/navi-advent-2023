using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SentenceAssemble : MonoBehaviour
{
    const float Margin = 20;
    GameObject _wordPrefab;
    public List<WordBankExercise> wordBankExercises;
    Queue<WordBankExercise> _exercises;
    WordBankExercise _currentExercise;
    
    public WordDropArea wordDropArea;
    public Transform wordBankArea;
    public TextMeshProUGUI originalSentence;

    public AudioClip dragSound;
    public AudioClip dropSound;
    public AudioClip returnSound;
    public AudioClip clickSound;
    public AudioClip unclickSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    AudioSource _source;
    
    GameObject _darken;
    GameObject _messageParent;
    GameObject _restartButton;
    GameObject _nextButton;
    GameObject _quitButton;
    TextMeshProUGUI _messageText;
    
    void Awake()
    {
        _wordPrefab = Resources.Load<GameObject>("Prefabs/Word");
        _darken = transform.Find("darken").gameObject;
        _messageParent = transform.Find("'upxare").gameObject;
        _restartButton = transform.Find("'upxare/Restart").gameObject;
        _nextButton = transform.Find("'upxare/Next").gameObject;
        _quitButton = transform.Find("'upxare/Quit").gameObject;
        _messageText = transform.Find("'upxare/pamrel").GetComponent<TextMeshProUGUI>();
        _source = GetComponent<AudioSource>();
        if (!Settings.current.GetSoundEnabled())
            _source.mute = true;
        
        _exercises = new Queue<WordBankExercise>();
        foreach (var exercise in wordBankExercises)
        {
            _exercises.Enqueue(exercise);
        }
        _currentExercise = _exercises.Dequeue();
        InstantiateWords();
    }

    void InstantiateWords()
    {
        originalSentence.text = _currentExercise.originalSentence;
        var shuffledWordList = _currentExercise.words.OrderBy(_ => Random.value).ToList(); // should be fine with small lists
        
        foreach (var word in shuffledWordList)
        {
            GameObject lexInstance = Instantiate(_wordPrefab, wordBankArea);
            var lex = lexInstance.GetComponent<LexemeInstance>();
            lex.SetLexeme(word, this);
            lex.SetDraggable(true);
        }
        
        Canvas.ForceUpdateCanvases();
        
        foreach (var word in WordBankWords())
        {
            word.UpdateWidth();
        }

        StartCoroutine(ReflowAgain());
    }

    public void OnStartDrag()
    {
        _source.clip = dragSound;
        _source.Play();
    }

    public void OnDrop()
    {
        _source.clip = dropSound;
        _source.Play();
    }

    public void OnClick()
    {
        _source.clip = clickSound;
        _source.Play();
    }

    public void OnUnClick()
    {
        _source.clip = unclickSound;
        _source.Play();
    }
    
    public void OnWordDroppedOutsideDropArea(LexemeInstance wordInstance)
    {
        _source.clip = returnSound;
        _source.Play();
        
        wordInstance.SetDraggable(false);

        var wordTransform = wordInstance.GetComponent<RectTransform>();
        
        if (wordTransform.parent != wordBankArea)
            wordTransform.SetParent(wordBankArea);
        
        wordTransform.SetParent(wordBankArea, true);
        StartCoroutine(wordInstance.MoveWord(wordTransform.anchoredPosition, GetWordPosition(wordInstance)));
        ReflowWords();
    }

    Vector3 GetWordPosition(LexemeInstance word)
    {
        float x = Margin;//-_screenWidth * 0.5f - 60;
        foreach (LexemeInstance t in WordBankWords())
        {
            if (t == word)
                return new Vector3(x, 0, 0);
            
            x += t.Width + 20f;
        }
        return Vector3.zero;
    }
    
    public void ReflowWords()
    {
        float x = Margin;//-_screenWidth * 0.5f - 60;
        foreach (LexemeInstance word in WordBankWords())
        {
            word.SetWordPos(new Vector3(x, 0, 0));
            x += word.Width + 20f;
        }
    }

    List<LexemeInstance> WordBankWords()
    {
        var words = new List<LexemeInstance>();
        foreach (Transform transform in wordBankArea)
        {
            words.Add(transform.GetComponent<LexemeInstance>());
        }

        return words;
    }

    public void SubmitSolution()
    {
        var droppedWords = wordDropArea.DroppedWords();

        var correct = _currentExercise.CheckSolution(droppedWords, out var message);

        _darken.SetActive(true);
        _messageParent.SetActive(true);
        _messageText.text = correct ? "Seysonìltsan!" : message;
        
        _source.clip = correct ? correctSound : wrongSound;
        _source.Play();

        if (_exercises.Count > 0 || !correct)
        {
            _nextButton.SetActive(correct);
            _restartButton.SetActive(!correct);
            _quitButton.SetActive(false);
        }
        else
        {
            _messageText.text = "Wou! You got them all.";
            _nextButton.SetActive(false);
            _restartButton.SetActive(false);
            _quitButton.SetActive(true);
        }
    }

    void ClearWords()
    {
        wordDropArea.Clear();
        foreach (Transform transform in wordBankArea)
        {
            Destroy(transform.gameObject);
        }
        foreach (Transform transform in wordBankArea)
        {
            Destroy(transform.gameObject);
        }
        _darken.SetActive(false);
        _messageParent.SetActive(false);
    }

    public void Retry()
    {
        ClearWords();
        InstantiateWords();
    }

    public void NextSentence()
    {
        ClearWords();
        _currentExercise = _exercises.Dequeue();
        InstantiateWords();
    }

    IEnumerator ReflowAgain()
    {
        yield return null;
        ReflowWords();
    }
}
