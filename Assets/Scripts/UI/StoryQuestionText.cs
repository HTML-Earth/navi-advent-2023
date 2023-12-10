using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryQuestionText : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    GameObject _answerPrefab;

    void Awake()
    {
        _answerPrefab = Resources.Load<GameObject>("Prefabs/StoryQuestionAnswer");
    }

    public void Play(StoryQuestion question)
    {
        if (question.question == "")
        {
            Destroy(tmp.transform.parent.gameObject);
            return;
        }
        
        tmp.text = question.question;

        foreach (var answer in question.answers)
        {
            var answerObj = Instantiate(_answerPrefab, transform);
            var text = answerObj.transform.Find("a/pamrel").GetComponent<TextMeshProUGUI>();
            text.text = answer;
        }
    }
}