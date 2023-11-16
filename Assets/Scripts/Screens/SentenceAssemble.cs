using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SentenceAssemble : MonoBehaviour
{
    float _screenWidth = 1000f; //TODO make dynamic
    GameObject _wordPrefab;
    public WordBankExercise wordBankExercise;
    
    public Transform wordBankArea;
    public TextMeshProUGUI originalSentence;
    
    void Awake()
    {
        _wordPrefab = Resources.Load<GameObject>("Prefabs/Word");
        originalSentence.text = wordBankExercise.originalSentence;
        InstantiateWords();
    }

    void InstantiateWords()
    {
        foreach (var word in wordBankExercise.words)
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
}
