using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryQuestionText : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public AudioSource source;
    
    public AudioClip correctSound;
    public AudioClip wrongSound;

    GameObject _answerPrefab;
    Story _story;
    List<Button> _buttons = new();
    StoryQuestion _question;

    void Awake()
    {
        _answerPrefab = Resources.Load<GameObject>("Prefabs/StoryQuestionAnswer");
        if (!Settings.current.GetSoundEnabled())
            source.mute = true;
    }

    public void Play(StoryQuestion question, Story story)
    {
        _story = story;
        _question = question;
        
        if (question.question == "")
        {
            Destroy(tmp.transform.parent.gameObject);
        }
        else
        {
            tmp.text = question.question;
        }

        for (var i = 0; i < question.answers.Count; i++)
        {
            var answer = question.answers[i];
            var answerObj = Instantiate(_answerPrefab, transform);

            var text = answerObj.transform.Find("a/pamrel").GetComponent<TextMeshProUGUI>();
            text.text = answer;

            var button = answerObj.transform.Find("b").GetComponent<Button>();
            var buttonIndex = i;
            button.onClick.AddListener(() => Answer(buttonIndex));
            _buttons.Add(button);
        }
    }

    public void Answer(int index)
    {
        if (index != _question.correctAnswer)
        {
            _buttons[index].image.color = Color.red;
            _buttons[index].interactable = false;
            
            source.clip = wrongSound;
            source.Play();
        }
        else
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
            
            var answerObj = Instantiate(_answerPrefab, transform);

            var text = answerObj.transform.Find("a/pamrel").GetComponent<TextMeshProUGUI>();
            var button = answerObj.transform.Find("b").GetComponent<Button>();
            button.image.color = Color.green;
            
            if (_question.question == "")
            {
                text.text = $"\"{_question.answers[_question.correctAnswer]}\"";
            }
            else
            {
                text.text = $"Correct! {_question.answerExplanation}";
            }
            
            source.clip = correctSound;
            source.Play();
            
            _story.Scroll(true);
            _story.DisplayContinueButton();
        }
    }
}