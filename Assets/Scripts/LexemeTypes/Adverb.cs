public class Adverb : Lexeme
{
    public override string Render()
    {
        throw new System.NotImplementedException();
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