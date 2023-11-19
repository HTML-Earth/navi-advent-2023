using System.Collections.Generic;
using UnityEngine;

public abstract class Lexeme : ScriptableObject
{
    public abstract string Render();
    public abstract string Root();

    public string GetLastSound()
    {
        var word = Render();

        //TODO the proper way
        // List<string> famrelvi = new List<string>();
        // for (int i = 0; i < word.Length; i++)
        // {
        //     
        // }
        
        return word[^1].ToString();
    }
    
    public abstract int GetSlotCount();
    public abstract bool SlotCanHoldLexeme(int slotIndex, Lexeme lexeme);
    public abstract bool SlotIsOccupied(int slotIndex);
    public abstract void InsertLexeme(int slotIndex, Lexeme lexeme);
    public abstract void RemoveLexeme(int slotIndex);
    public abstract void OnInserted(Lexeme hostWord);
    public abstract void OnRemoved();
    public abstract bool CanBeStandaloneWord();
}
