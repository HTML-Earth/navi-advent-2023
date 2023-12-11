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
    GameObject _quitButton;
    Transform _parent;
    ScrollRect _scrollRect;
    AudioSource _source;

    GameObject _storyTitlePrefab;
    GameObject _storyLinePrefab;
    GameObject _storySpeakerLinePrefab;
    GameObject _storyQuestionPrefab;
    GameObject _storyWordPairsPrefab;

    Queue<StoryPart> parts;

    bool _instantText;
    bool _instantContinueNextTime;

    void Awake()
    {
        _continueButton = transform.Find("Continue").gameObject;
        _quitButton = transform.Find("Quit").gameObject;
        _parent = transform.Find("Scroll View/Viewport/Content");
        _scrollRect = transform.Find("Scroll View").GetComponent<ScrollRect>();
        _storyTitlePrefab = Resources.Load<GameObject>("Prefabs/StoryTitle");
        _storyLinePrefab = Resources.Load<GameObject>("Prefabs/StoryLine");
        _storySpeakerLinePrefab = Resources.Load<GameObject>("Prefabs/StorySpeakerLine");
        _storyQuestionPrefab = Resources.Load<GameObject>("Prefabs/StoryQuestion");
        _storyWordPairsPrefab = Resources.Load<GameObject>("Prefabs/StoryWordPairs");
        
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
        
        _instantContinueNextTime = false;
        _continueButton.SetActive(false);
        
        var nextPart = parts.Dequeue();
        GameObject prefab = null;

        switch (nextPart)
        {
            case StoryLine line:
            {
                if (line.instantContinue)
                    _instantContinueNextTime = true;
                
                var hasSpeaker = line.speakerName != "";
                prefab = Instantiate(line.isTitle ? _storyTitlePrefab : hasSpeaker ? _storySpeakerLinePrefab : _storyLinePrefab, _parent);
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
                questionUi.Play(question, this);
                
                prefab.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical() ;
                prefab.GetComponent<ContentSizeFitter>().SetLayoutVertical() ;
                break;
            
            case StoryWordPairs wordPairs:
                _instantContinueNextTime = true;
                
                prefab = Instantiate(_storyWordPairsPrefab, _parent);
                
                var storyMiniWordPairs = prefab.GetComponent<StoryMiniWordPairs>();
                storyMiniWordPairs.Play(wordPairs, this);
                break;
        }
        
        if (prefab == null)
            return;
        
        Scroll();
    }

    public void Scroll(bool fixEmpty = false)
    {
        if (fixEmpty)
        {
            var placeholder = Instantiate(_storyLinePrefab, _parent);
            Destroy(placeholder);
        }   
        
        Canvas.ForceUpdateCanvases();

        _scrollRect.content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        _scrollRect.content.GetComponent<ContentSizeFitter>().SetLayoutVertical();

        if (fixEmpty)
            _scrollRect.verticalNormalizedPosition = 0;
        else
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
        if (_instantContinueNextTime)
        {
            NextPart();
            return;
        }
        
        if (parts.Count < 1)
            _quitButton.SetActive(true);
        else
            _continueButton.SetActive(true);
    }

    public bool InstantText()
    {
        return _instantText;
    }
}
