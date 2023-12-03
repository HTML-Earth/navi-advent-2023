using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Exercises/Story")]
public class StoryExercise : ScriptableObject
{
    public List<StoryPart> parts;
}