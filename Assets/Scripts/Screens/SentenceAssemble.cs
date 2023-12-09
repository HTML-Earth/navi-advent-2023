using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SentenceAssemble : MonoBehaviour
{
    float _screenWidth;
    GameObject _wordPrefab;
    public List<WordBankExercise> wordBankExercises;
    Queue<WordBankExercise> _exercises;
    WordBankExercise _currentExercise;
    
    public WordDropArea wordDropArea;
    public Transform wordBankArea;
    public TextMeshProUGUI originalSentence;
    
    GameObject _darken;
    GameObject _messageParent;
    GameObject _restartButton;
    GameObject _nextButton;
    GameObject _quitButton;
    TextMeshProUGUI _messageText;
    
    void Awake()
    {
        var margin = 100;
        _screenWidth = GetComponent<RectTransform>().rect.width - margin * 2;
        _wordPrefab = Resources.Load<GameObject>("Prefabs/Word");
        _darken = transform.Find("darken").gameObject;
        _messageParent = transform.Find("'upxare").gameObject;
        _restartButton = transform.Find("'upxare/Restart").gameObject;
        _nextButton = transform.Find("'upxare/Next").gameObject;
        _quitButton = transform.Find("'upxare/Quit").gameObject;
        _messageText = transform.Find("'upxare/pamrel").GetComponent<TextMeshProUGUI>();
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

    public void OnWordDroppedOutsideDropArea(LexemeInstance wordInstance)
    {
        wordInstance.SetDraggable(false);

        var wordTransform = wordInstance.transform;
        
        if (wordTransform.parent != wordBankArea)
            wordTransform.SetParent(wordBankArea);
        
        wordTransform.SetParent(wordBankArea, true);
        StartCoroutine(wordInstance.MoveWord(wordTransform.localPosition, GetWordPosition(wordInstance)));
        ReflowWords();
    }

    Vector3 GetWordPosition(LexemeInstance word)
    {
        float x = -_screenWidth * 0.5f;
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
        float x = -_screenWidth * 0.5f;
        foreach (LexemeInstance word in WordBankWords())
        {
            word.transform.localPosition = new Vector3(x, 0, 0);
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
