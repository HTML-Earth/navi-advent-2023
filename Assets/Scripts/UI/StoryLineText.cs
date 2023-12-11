using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryLineText : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public Image icon;
    public AudioSource source;

    Story _story;

    public void Play(StoryLine line, Story story)
    {
        _story = story;
        
        if (icon != null)
        {
            icon.sprite = line.speakerIcon;
            icon.color = line.color;
        }
        
        if (line.audio != null)
        {
            if (!Settings.current.GetSoundEnabled())
                source.mute = true;
            source.clip = line.audio;
            source.Play();
        }
        
        if (line.highlightTimings.Count < 1 || story.InstantText() || line.audio == null)
        {
            tmp.text = line.line;
            OnFinishedLine();
        }
        else
        {
            var split = line.line.Split(' ');
            StartCoroutine(HighlightText(split, line.highlightTimings));
        }
    }

    IEnumerator HighlightText(string[] splitLine, List<float> timings)
    {
        float t = 0;
        
        var sb = new StringBuilder();
        for (var word = 0; word < splitLine.Length; word++)
        {
            if (word < timings.Count)
                t += timings[word];
            
            sb.Clear();
            for (var i = 0; i <= word; i++)
            {
                sb.Append(splitLine[i]);
                sb.Append(' ');
            }
            sb.Append("<color=grey>");
            for (var i = word+1; i < splitLine.Length; i++)
            {
                sb.Append(splitLine[i]);
                sb.Append(' ');
            }
            tmp.text = sb.ToString();
            
            while (source.time < t)
            {
                yield return null;
            }
            
            // if (word < timings.Count)
            // {
            //     
            //     yield return new WaitForSeconds(timings[word]);
            // }
            
        }
        OnFinishedLine();
    }

    void OnFinishedLine()
    {
        _story.DisplayContinueButton();
    }
}