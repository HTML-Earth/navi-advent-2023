using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Ayopin")]
public class Ayopin : ScriptableObject
{
    [Header("SentenceAssemble")]
    public Color wordBg;
    public Color wordBgHover;
    public Color wordBgDragging;
    
    public Color lexSlot;
    public Color lexSlotHover;
    public Color lexSlotDragging;

    [Header("MatchPairs")]
    public Color unselectedWordBg;
    public Color hoveredWordBg;
    public Color selectedWordBg;
    public Color matchedWordBg;
    public Color failedWordBg;
}
