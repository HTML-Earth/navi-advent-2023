public class Adjective : Lexeme
{
    public override string Render()
    {
        throw new System.NotImplementedException();
    }

    public override string Root()
    {
        throw new System.NotImplementedException();
    }

    public override int GetSlotCount()
    {
        return 2;
    }

    public override bool SlotCanHoldLexeme(int slotIndex, Lexeme lexeme)
    {
        //TODO: a- fu -a
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