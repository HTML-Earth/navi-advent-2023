using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Exercises/StoryPart/Question")]
public class StoryQuestion : StoryPart
{
    [TextArea] public string question;
    public List<string> answers;
    public int correctAnswer;
    public string answerExplanation;
}