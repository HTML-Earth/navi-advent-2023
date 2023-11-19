using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SentenceAssemble : MonoBehaviour
{
    float _screenWidth;
    GameObject _wordPrefab;
    public WordBankExercise wordBankExercise;
    
    public WordDropArea wordDropArea;
    public Transform wordBankArea;
    public TextMeshProUGUI originalSentence;
    
    GameObject _darken;
    GameObject _messageParent;
    TextMeshProUGUI _messageText;
    
    void Awake()
    {
        var margin = 100;
        _screenWidth = GetComponent<RectTransform>().rect.width - margin * 2;
        _wordPrefab = Resources.Load<GameObject>("Prefabs/Word");
        _darken = transform.Find("darken").gameObject;
        _messageParent = transform.Find("'upxare").gameObject;
        _messageText = transform.Find("'upxare/pamrel").GetComponent<TextMeshProUGUI>();
        originalSentence.text = wordBankExercise.originalSentence;
        InstantiateWords();
    }

    void InstantiateWords()
    {
        var shuffledWordList = wordBankExercise.words.OrderBy(_ => Random.value).ToList(); // should be fine with small lists
        
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

        ReflowWords();
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

        var correct = wordBankExercise.CheckSolution(droppedWords, out var message);

        _darken.SetActive(true);
        _messageParent.SetActive(true);
        _messageText.text = correct ? "Seysonìltsan!" : $"Keftxo.\n{message}";
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
