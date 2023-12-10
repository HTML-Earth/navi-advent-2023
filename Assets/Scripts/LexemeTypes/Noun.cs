using System;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Lexemes/Noun")]
public class Noun : Lexeme
{
    public string text = "";

    Lexeme _prefix;
    Lexeme _postfix;
    
    public override string Render()
    {
        var sb = new StringBuilder();
        
        if (_prefix != null)
            sb.Append(_prefix.Render());

        //TODO: lenition
        
        sb.Append(text);
        
        if (_postfix != null)
            sb.Append(_postfix.Render());
        
        return sb.ToString();
    }

    public override string Root()
    {
        return text;
    }

    public override int GetSlotCount()
    {
        return 2;
    }

    public override bool SlotCanHoldLexeme(int slotIndex, Lexeme lexeme)
    {
        if (lexeme is Prefix)
        {
            return slotIndex == 0;
        }
        
        if (lexeme is CaseEnding or Adposition)
        {
            return slotIndex == 1;
        }
        
        return false;
    }

    public override bool SlotIsOccupied(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                return _prefix != null;
            case 1:
                return _postfix != null;
        }

        throw new Exception($"Invalid slot index ({slotIndex})");
    }

    public override Lexeme GetLexemeFromSlot(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                return _prefix;
            case 1:
                return _postfix;
        }

        throw new Exception($"Invalid slot index ({slotIndex})");
    }

    public override void InsertLexeme(int slotIndex, Lexeme lexeme)
    {
        switch (slotIndex)
        {
            case 0:
                _prefix = lexeme;
                break;
            case 1:
                _postfix = lexeme;
                break;
        }
    }

    public override void RemoveLexeme(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                _prefix = null;
                break;
            case 1:
                _postfix = null;
                break;
        }
    }

    public override void OnInserted(Lexeme hostWord)
    {
        throw new System.NotImplementedException();
    }

    public override void OnRemoved()
    {
        throw new System.NotImplementedException();
    }

    public override bool CanBeStandaloneWord()
    {
        return true;
    }

    public override bool CanBeUsedWith(Lexeme hostWord)
    {
        return false;
    }
}