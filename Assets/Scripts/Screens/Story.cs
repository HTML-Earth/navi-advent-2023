using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
    public StoryExercise storyExercise;
    
    public AudioClip selectSound;
    public AudioClip correctPairSound;
    public AudioClip wrongPairSound;
    public AudioClip victorySound;

    GameObject _continueButton;
    Transform _parent;
    ScrollRect _scrollRect;
    AudioSource _source;

    GameObject _storyLinePrefab;
    GameObject _storySpeakerLinePrefab;
    GameObject _storyQuestionPrefab;

    Queue<StoryPart> parts;

    bool _instantText;

    void Awake()
    {
        _continueButton = transform.Find("Continue").gameObject;
        _parent = transform.Find("Scroll View/Viewport/Content");
        _scrollRect = transform.Find("Scroll View").GetComponent<ScrollRect>();
        _storyLinePrefab = Resources.Load<GameObject>("Prefabs/StoryLine");
        _storySpeakerLinePrefab = Resources.Load<GameObject>("Prefabs/StorySpeakerLine");
        _storyQuestionPrefab = Resources.Load<GameObject>("Prefabs/StoryQuestion");
        
        _source = GetComponent<AudioSource>();
        if (!Settings.current.GetSoundEnabled())
        {
            _source.mute = true;
            _instantText = true;
        }

        parts = new Queue<StoryPart>();
        foreach (var part in storyExercise.parts)
        {
            parts.Enqueue(part);
        }
    }

    void Start()
    {
        NextPart();
    }

    public void NextPart()
    {
        if (parts.Count < 1)
            return;
        
        _continueButton.SetActive(false);
        
        var nextPart = parts.Dequeue();
        GameObject prefab = null;

        switch (nextPart)
        {
            case StoryLine line:
            {
                var hasSpeaker = line.speakerName != "";
                prefab = Instantiate(hasSpeaker ? _storySpeakerLinePrefab : _storyLinePrefab, _parent);
                var lineUi = prefab.GetComponent<StoryLineText>();
                if (line.audio != null)
                {
                    _source.clip = line.audio;
                    _source.Play();
                }
                lineUi.Play(line, this);
                break;
            }
            case StoryQuestion question:
                prefab = Instantiate(_storyQuestionPrefab, _parent);
                
                var questionUi = prefab.GetComponent<StoryQuestionText>();
                questionUi.Play(question);
                
                prefab.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical() ;
                prefab.GetComponent<ContentSizeFitter>().SetLayoutVertical() ;
                break;
        }
        
        if (prefab == null)
            return;

        Canvas.ForceUpdateCanvases();

        _scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical() ;
        _scrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical() ;

        //_scrollRect.verticalNormalizedPosition = 0 ;
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        var t = 0f;
        var dur = 0.2f;
        var invDur = 1 / dur;
        var start = _scrollRect.verticalNormalizedPosition;
        while (t <= 1f * dur)
        {
            t += Time.deltaTime;
            _scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, 0, t * invDur);
            yield return null;
        }
    }

    public void DisplayContinueButton()
    {
        _continueButton.SetActive(true);
    }

    public bool InstantText()
    {
        return _instantText;
    }
}
