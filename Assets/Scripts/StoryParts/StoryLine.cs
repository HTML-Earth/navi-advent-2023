using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Exercises/StoryPart/Line")]
public class StoryLine : StoryPart
{
    public string speakerName;
    [TextArea]
    public string line;
    public Color color = Color.white;
    public Sprite speakerIcon;
    public AudioClip audio;
    public List<float> highlightTimings;
}