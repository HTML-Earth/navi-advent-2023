using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryQuestionText : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    public void Play(StoryQuestion question)
    {
        if (question.question == "")
        {
            Destroy(tmp.transform.parent.gameObject);
            return;
        }
        
        tmp.text = question.question;
    }
}