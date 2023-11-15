using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lexemes/CaseEnding")]
public class CaseEnding : Lexeme
{
    public string text;
    public List<string> vowelEndings;
    public List<string> consonantEndings;
    public List<string> diphthongEndings;

    string _wordEndsWith = "";
    
    public override string Render()
    {
        switch (_wordEndsWith)
        {
            case "":
                return text;
            
            case "a":
            case "ä":
            case "e":
            case "i":
            case "ì":
            case "o":
            case "u":
            case "ù":
                return vowelEndings[0];
                
            case "aw":
            case "ay":
            case "ew":
            case "ey":
                return diphthongEndings[0];
            
            case "kx":
            case "g":
            case "w":
            case "r":
            case "y":
            case "p":
            case "s":
            case "tx":
            case "d":
            case "f":
            case "ng":
            case "h":
            case "k":
            case "l":
            case "z":
            case "ts":
            case "v":
            case "px":
            case "b":
            case "n":
            case "m":
            case "'":
                return consonantEndings[0];
        }
        
        Debug.LogWarning($"Could not find proper case ending for [{_wordEndsWith}]");
        return text;
    }

    public override int GetSlotCount()
    {
        return 0;
    }

    public override bool SlotCanHoldLexeme(int slotIndex, Lexeme lexeme)
    {
        return false;
    }

    public override bool SlotIsOccupied(int slotIndex)
    {
        throw new System.NotImplementedException();
    }

    public override void InsertLexeme(int slotIndex, Lexeme lexeme)
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveLexeme(int slotIndex)
    {
        throw new System.NotImplementedException();
    }

    public override void OnInserted(Lexeme hostWord)
    {
        _wordEndsWith = hostWord.GetLastSound();
    }

    public override void OnRemoved()
    {
        _wordEndsWith = "";
    }

    public override bool CanBeStandaloneWord()
    {
        return false;
    }
}