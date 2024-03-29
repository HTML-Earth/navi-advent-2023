using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordPairButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    bool _isSelected;
    bool _isMatched;
    bool _isHovering;
    bool _isFlashing;

    int _index;
    
    public Image bg;
    public TextMeshProUGUI buttonText;

    IMatchPairs _matchPairs;

    public void Init(int index, IMatchPairs matchPairs)
    {
        _index = index;
        _matchPairs = matchPairs;
        _isMatched = false;
        _isSelected = false;
        _isHovering = false;
        LatemOpin();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isMatched || _isSelected || _isFlashing)
            return;
        
        _isHovering = true;
        LatemOpin();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isMatched || _isSelected || _isFlashing)
            return;

        _isHovering = false;
        LatemOpin();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isMatched || _isSelected || _isFlashing)
            return;
        
        _matchPairs.TrySelect(_index);
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }

    public void Select()
    {
        _isSelected = true;
        _isHovering = false;
        LatemOpin();
    }

    public void Deselect()
    {
        _isSelected = false;
        _isHovering = false;
        LatemOpin();
    }

    public void Fail()
    {
        _isSelected = false;
        _isHovering = false;
        StartCoroutine(FailFlash());
    }

    IEnumerator FailFlash()
    {
        _isFlashing = true;
        bg.color = Settings.current.ayopin.failedWordBg;
        buttonText.color = Color.white;

        yield return new WaitForSeconds(0.3f);
        
        _isFlashing = false;
        LatemOpin();
    }

    public void Match()
    {
        _isMatched = true;
        _isHovering = false;
        LatemOpin();
    }

    void LatemOpin()
    {
        bg.color = _isMatched
            ? Settings.current.ayopin.matchedWordBg
            : _isSelected
                ? Settings.current.ayopin.selectedWordBg
                : _isHovering
                    ? Settings.current.ayopin.hoveredWordBg
                    : Settings.current.ayopin.unselectedWordBg;
        
        buttonText.color =  _isMatched
            ? Color.black
            : _isSelected
                ? Color.white
                : Color.black;
    }
}
