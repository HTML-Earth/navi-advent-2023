using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Exercises/WordBank")]
public class WordBankExercise : ScriptableObject
{
    public string originalSentence;
    public List<Lexeme> words;
    public List<string> solutions;
}