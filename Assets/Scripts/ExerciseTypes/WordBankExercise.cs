using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Exercises/WordBank")]
public class WordBankExercise : ScriptableObject
{
    public string originalSentence;
    public List<Lexeme> words;
    public string solution;

    public bool CheckSolution(List<LexemeInstance> input, out string message)
    {
        message = "No problems found";

        var solutionWords = new Queue<string>(solution.Split(' '));
        while (solutionWords.Count > 0)
        {
            var word = solutionWords.Dequeue();
            
            var affixes = word.Split('|').ToList();
            var root = affixes[0];
            affixes.Remove(root);
            
            var foundWord = false;
            
            foreach (var inputWord in input)
            {
                if (inputWord.Lexeme.Root() != root)
                    continue;

                var inputAffixCount = 0;
                for (var i = 0; i < inputWord.Lexeme.GetSlotCount(); i++)
                {
                    if (inputWord.Lexeme.SlotIsOccupied(i))
                        inputAffixCount++;
                }
                
                if (inputAffixCount != affixes.Count)
                    continue;

                if (affixes.Count == 0)
                {
                    foundWord = true;
                    break;
                }
                
                var affixesFound = 0;

                foreach (var affix in affixes)
                {
                    var affixSplit = affix.Split(':');
                    var slotIndex = Convert.ToInt32(affixSplit[0]);
                    var affixName = affixSplit[1];
                
                    if (!inputWord.Lexeme.SlotIsOccupied(slotIndex))
                        break;
                    
                    var lex = inputWord.Lexeme.GetLexemeFromSlot(slotIndex);
                    if (lex.Root() == affixName)
                    {
                        affixesFound++;
                    }
                }

                if (affixesFound == affixes.Count)
                    foundWord = true;
            }

            if (!foundWord)
            {
                var sb = new StringBuilder();
                sb.Append(root);
                foreach (var affix in affixes)
                {
                    var affixSplit = affix.Split(':');
                    sb.Append($" ({affixSplit[1]})");
                }
                
                message = $"Your translation is missing: <b>{sb}</b>";
                return false;
            }
        }
        
        return true;
    }
}