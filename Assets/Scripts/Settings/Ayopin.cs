using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Ayopin")]
public class Ayopin : ScriptableObject
{
    [Header("SentenceAssemble")]
    public Color nounBg;
    public Color nounBgHover;
    public Color nounBgDragging;
    
    public Color caseEndingBg;
    public Color caseEndingBgHover;
    public Color caseEndingBgDragging;

    [Header("MatchPairs")]
    public Color unselectedWordBg;
    public Color hoveredWordBg;
    public Color selectedWordBg;
    public Color matchedWordBg;
    public Color failedWordBg;
}
