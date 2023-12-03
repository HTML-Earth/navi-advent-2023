using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryLineText : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public Image icon;

    public void Play(StoryLine line)
    {
        tmp.text = line.line;
        if (icon != null)
        {
            icon.color = line.color;
        }
    }
}