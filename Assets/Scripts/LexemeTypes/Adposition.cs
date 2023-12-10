using UnityEngine;

[CreateAssetMenu(menuName = "Lexemes/Adposition")]
public class Adposition : Lexeme
{
    public string text = "";
    public bool causesLenition = false;
    public override string Render()
    {
        return text;
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
        //dslfj
    }

    public override void OnRemoved()
    {
        //
    }

    public override bool CanBeStandaloneWord()
    {
        return true;
    }

    public override bool CanBeUsedWith(Lexeme hostWord)
    {
        return hostWord is Noun;
    }
}