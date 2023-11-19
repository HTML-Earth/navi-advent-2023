using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Ayopin")]
public class Ayopin : ScriptableObject
{
    [Header("SentenceAssemble")]
    public Color nounBg;
    public Color nounBgHover;
    public Color nounBgDragging;
    
    public Color prefixBg;
    public Color prefixBgHover;
    public Color prefixBgDragging;
    
    public Color infixBg;
    public Color infixBgHover;
    public Color infixBgDragging;
    
    public Color caseEndingBg;
    public Color caseEndingBgHover;
    public Color caseEndingBgDragging;

    [Header("MatchPairs")]
    public Color unselectedWordBg;
    public Color hoveredWordBg;
    public Color selectedWordBg;
    public Color matchedWordBg;
    public Color failedWordBg;

    public Color DefaultColorFromLexeme(Lexeme lexeme)
    {
        return lexeme switch
        {
            Prefix => prefixBg,
            Infix => infixBg,
            CaseEnding => caseEndingBg,
            _ => nounBg
        };
    }
    
    public Color HoverColorFromLexeme(Lexeme lexeme)
    {
        return lexeme switch
        {
            Prefix => prefixBgHover,
            Infix => infixBgHover,
            CaseEnding => caseEndingBgHover,
            _ => nounBgHover
        };
    }
    
    public Color DraggingColorFromLexeme(Lexeme lexeme)
    {
        return lexeme switch
        {
            Prefix => prefixBgDragging,
            Infix => infixBgDragging,
            CaseEnding => caseEndingBgDragging,
            _ => nounBgDragging
        };
    }
}
