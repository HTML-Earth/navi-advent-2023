using UnityEngine;

[CreateAssetMenu(menuName = "Lexemes/Prefix")]
public class Prefix : Lexeme
{
    public string text = "";
    public bool lenites;
    public bool worksWithNouns;
    public bool worksWithVerbs;
    
    public override string Render()
    {
        return text.Replace("-", "");
    }

    public override string Root()
    {
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

    public override Lexeme GetLexemeFromSlot(int slotIndex)
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
        //
    }

    public override void OnRemoved()
    {
        //
    }

    public override bool CanBeStandaloneWord()
    {
        return false;
    }

    public override bool CanBeUsedWith(Lexeme hostWord)
    {
        return hostWord switch
        {
            Noun => worksWithNouns,
            Verb => worksWithVerbs,
            _ => false
        };
    }
}