using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Lexemes/Verb")]
public class Verb : Lexeme
{
    public string text = "";
    public bool transitive = false;

    Infix _preFirst;
    Infix _first;
    Infix _second;
    
    public override string Render()
    {
        var sb = new StringBuilder();

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

    public override int GetSlotCount()
    {
        return 3;
    }

    public override bool SlotCanHoldLexeme(int slotIndex, Lexeme lexeme)
    {
        if (lexeme is not Infix infix)
            return false;

        return infix.position == slotIndex;
    }

    public override bool SlotIsOccupied(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                return _preFirst != null;
            case 1:
                return _first != null;
            case 2:
                return _second != null;
        }

        throw new Exception($"Invalid slot index ({slotIndex})");
    }

    public override void InsertLexeme(int slotIndex, Lexeme lexeme)
    {
        var infix = lexeme as Infix;
        
        switch (slotIndex)
        {
            case 0:
                _preFirst = infix;
                break;
            case 1:
                _first = infix;
                break;
            case 2:
                _second = infix;
                break;
        }
    }

    public override void RemoveLexeme(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                _preFirst = null;
                break;
            case 1:
                _first = null;
                break;
            case 2:
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
}