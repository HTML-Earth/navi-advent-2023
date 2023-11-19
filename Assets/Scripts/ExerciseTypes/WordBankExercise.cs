using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Exercises/WordBank")]
public class WordBankExercise : ScriptableObject
{
    public string originalSentence;
    public List<Lexeme> words;
    public List<string> solutions;
    public string solution;

    public bool CheckSolution(List<LexemeInstance> input, out string message)
    {
        message = "No problems found";
        var solutionWords = solution.Split(' ').ToList();

        foreach (var word in solutionWords)
        {
            var wordSplit = word.Split('-');
            var root = wordSplit[0];
            var caseEnding = wordSplit.Length > 1 ? wordSplit[1] : "";

            var foundWord = false;
            
            foreach (var inputWord in input)
            {
                if (inputWord.Lexeme.Root() == root)
                {
                    if (inputWord.Lexeme.SlotIsOccupied(1)) //postfix
                    {
                        if (wordSplit.Length == 1)
                            continue;
                        
                        if (inputWord.Lexeme.Render().EndsWith(caseEnding))
                        {
                            foundWord = true;
                        }
                    }
                    else
                    {
                        if (wordSplit.Length == 1)
                        {
                            foundWord = true;
                        }
                    }
                }
            }

            if (!foundWord)
            {
                message = $"Couldn't find [{word}]";
                return false;
            }
        }
        
        return true;
    }
}