using UnityEngine;

[CreateAssetMenu(menuName = "Exercises/StoryPart/Line")]
public class StoryLine : StoryPart
{
    [TextArea]
    public string line;
    public string speakerName;
    public Color color = Color.white;
    public Sprite speakerIcon;
    public AudioClip audio;
}