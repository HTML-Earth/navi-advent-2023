using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Lexemes/Verb")]
public class Verb : Lexeme
{
    const int PositionOffset = +1;
    
    public string text = "";
    public bool transitive = false;

    Prefix _preFix;
    Infix _preFirst;
    Infix _first;
    Infix _second;
    
    public override string Render()
    {
        var sb = new StringBuilder();

        if (_preFix != null)
            sb.Append(_preFix.Render());
        
        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '.')
            {
                if (_preFirst != null)
                    sb.Append(_preFirst.Render());
        
                if (_first != null)
                    sb.Append(_first.Render());
                
                i++;
                break;
            }
            
            sb.Append(text[i]);
            i++;
        }
        while (i < text.Length)
        {
            if (text[i] == '.')
            {
                if (_second != null)
                    sb.Append(_second.Render());
                
                i++;
                break;
            }
            
            sb.Append(text[i]);
            i++;
        }
        while (i < text.Length)
        {
            sb.Append(text[i]);
            i++;
        }
        
        return sb.ToString();
    }

    public override string Root()
    {
        return text.Replace(".", "");
    }

    public override int GetSlotCount()
    {
        return 4;
    }

    public override bool SlotCanHoldLexeme(int slotIndex, Lexeme lexeme)
    {
        if (lexeme is Infix infix)
            return slotIndex == infix.position + PositionOffset;
        
        if (lexeme is Prefix)
            return slotIndex == -1 + PositionOffset;

        return false;
    }

    public override bool SlotIsOccupied(int slotIndex)
    {
        switch (slotIndex)
        {
            case -1 + PositionOffset:
                return _preFix != null;
            case 0 + PositionOffset:
                return _preFirst != null;
            case 1 + PositionOffset:
                return _first != null;
            case 2 + PositionOffset:
                return _second != null;
        }

        throw new Exception($"Invalid slot index ({slotIndex})");
    }

    public override Lexeme GetLexemeFromSlot(int slotIndex)
    {
        switch (slotIndex)
        {
            case -1 + PositionOffset:
                return _preFix;
            case 0 + PositionOffset:
                return _preFirst;
            case 1 + PositionOffset:
                return _first;
            case 2 + PositionOffset:
                return _second;
        }

        throw new Exception($"Invalid slot index ({slotIndex})");
    }

    public override void InsertLexeme(int slotIndex, Lexeme lexeme)
    {
        if (lexeme is Infix infix)
        {
            switch (slotIndex)
            {
                case 0 + PositionOffset:
                    _preFirst = infix;
                    break;
                case 1 + PositionOffset:
                    _first = infix;
                    break;
                case 2 + PositionOffset:
                    _second = infix;
                    break;
            }
        }

        if (lexeme is Prefix prefix)
            _preFix = prefix;
    }

    public override void RemoveLexeme(int slotIndex)
    {
        switch (slotIndex)
        {
            case -1 + PositionOffset:
                _preFix = null;
                break;
            case 0 + PositionOffset:
                _preFirst = null;
                break;
            case 1 + PositionOffset:
                _first = null;
                break;
            case 2 + PositionOffset:
                _second = null;
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