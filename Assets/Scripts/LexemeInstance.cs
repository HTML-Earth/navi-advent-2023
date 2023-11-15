using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LexemeInstance : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    Vector2 _dragOffset;

    SentenceAssemble _sentenceAssemble;
    WordDropArea _wordDropArea;
    GameObject _wordPrefab;
    GameObject _slotPrefab;
    
    TextMeshProUGUI _text;
    Image _image;
    
    Lexeme _lexeme;
    float _width;
    
    int _slotCount;
    Dictionary<int, Lexeme> _insertedLexemes = new();
    Dictionary<int, (RectTransform, Image)> _insertedLexemeHandles = new();

    int _hoverState = -1; // -1 = word itself, 0,1,2... are inserted lexes
    
    bool _isDraggable;
    bool _isInDropArea;
    
    public Lexeme Lexeme => _lexeme;
    public float Width => _width;

    void Awake()
    {
        _wordPrefab = Resources.Load<GameObject>("Prefabs/Word");
        _slotPrefab = Resources.Load<GameObject>("Prefabs/slot");
        _text = transform.Find("text").GetComponent<TextMeshProUGUI>();
        _image = GetComponent<Image>();
        _image.color = Settings.current.ayopin.wordBg;
    }

    public void SetLexeme(Lexeme lexeme, SentenceAssemble sentenceAssemble)
    {
        this._sentenceAssemble = sentenceAssemble;
        this._lexeme = Instantiate(lexeme);
        _slotCount = this._lexeme.GetSlotCount();
        _text.text = this._lexeme.Render();
    }

    public void OnInsertedLexeme(int slot, Lexeme lex)
    {
        _insertedLexemes.Add(slot, lex);
        Refresh();
    }

    public void Refresh()
    {
        _text.text = _lexeme.Render();
        Canvas.ForceUpdateCanvases();
        UpdateWidth();
    }

    public void UpdateWidth()
    {
        var rectT = GetComponent<RectTransform>();
        _width = _text.textBounds.size.x + 40;
        rectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _width);
        
        if (_slotCount > 0)
        {
            var slotSpacing = _width / (_slotCount + 1);
            for (int i = 0; i < _slotCount; i++)
            {
                var slotPos = slotSpacing * (i + 1);

                if (!_insertedLexemes.ContainsKey(i) || _insertedLexemeHandles.ContainsKey(i))
                {
                    if (_insertedLexemeHandles.TryGetValue(i, out (RectTransform, Image) handle))
                    {
                        handle.Item1.localPosition = new Vector3(slotPos, 35, 0);
                    }
                    continue;
                }
                
                var slot = Instantiate(_slotPrefab, transform);
                slot.transform.localPosition = new Vector3(slotPos, 35, 0);
                
                _insertedLexemeHandles.Add(i, (slot.GetComponent<RectTransform>(), slot.GetComponent<Image>()));
            }
        }
    }

    public void SetDraggable(bool draggable)
    {
        _isDraggable = draggable;
    }

    public void OnDroppedInDropArea(WordDropArea dropArea)
    {
        _wordDropArea = dropArea;
        _isInDropArea = true;
        _sentenceAssemble.ReflowWords();
    }

    public void OnDroppedOntoWord()
    {
        _sentenceAssemble.ReflowWords();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
            return;

        if (_hoverState == -1)
        {
            if (_isInDropArea)
            {
                _wordDropArea.RemoveWord(this);
                _isInDropArea = false;
                _wordDropArea = null;
            }
        
            var position = transform.position;
            _dragOffset = eventData.position - new Vector2(position.x, position.y);
            _image.raycastTarget = false;
        }
        else
        {
            GameObject draggedOutWord = Instantiate(_wordPrefab, _sentenceAssemble.wordBankArea);
            var lex = draggedOutWord.GetComponent<LexemeInstance>();
            lex.SetLexeme(_insertedLexemes[_hoverState], _sentenceAssemble);
            lex.SetDraggable(true);
            lex.Refresh();
            
            draggedOutWord.transform.position = transform.position;
            eventData.pointerDrag = draggedOutWord;

            Destroy(_insertedLexemeHandles[_hoverState].Item1.gameObject);
            _lexeme.RemoveLexeme(_hoverState);
            _insertedLexemes.Remove(_hoverState);
            _insertedLexemeHandles.Remove(_hoverState);
            Refresh();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
            return;
        
        _image.color = Settings.current.ayopin.wordBgDragging;
        foreach (var handle in _insertedLexemeHandles)
        {
            handle.Value.Item2.color = Settings.current.ayopin.lexSlotDragging;
        }
        
        transform.position = eventData.position - _dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
            return;

        if (!_isInDropArea)
            _sentenceAssemble.OnWordDroppedOutsideDropArea(this);
        
        _image.color = Settings.current.ayopin.wordBg;
        foreach (var handle in _insertedLexemeHandles)
        {
            handle.Value.Item2.color = Settings.current.ayopin.lexSlot;
        }
        
        _image.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateHoverColor(eventData.position);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        UpdateHoverColor(eventData.position);
    }

    void UpdateHoverColor(Vector2 cursorPos)
    {
        bool hoveringOverHandle = false;
        foreach (KeyValuePair<int, (RectTransform, Image)> handle in _insertedLexemeHandles)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(handle.Value.Item1, cursorPos))
            {
                hoveringOverHandle = true;
                _hoverState = handle.Key;
                handle.Value.Item2.color = Settings.current.ayopin.lexSlotHover;
            }
            else
            {
                handle.Value.Item2.color = Settings.current.ayopin.lexSlot;
            }
        }

        if (hoveringOverHandle)
        {
            _image.color = Settings.current.ayopin.wordBg;
            return;
        }
        
        _hoverState = -1;
        _image.color = Settings.current.ayopin.wordBgHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverState = -1;
        _image.color = Settings.current.ayopin.wordBg;
        foreach (var handle in _insertedLexemeHandles)
        {
            handle.Value.Item2.color = Settings.current.ayopin.lexSlot;
        }
    }
    
    public IEnumerator MoveWord(Vector3 startPos, Vector3 endPos)
    {
        float t = 0;
        float mul = 4;
        while (t <= 0.25f)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, t * mul);
            yield return null;
            t += Time.deltaTime;
        }

        transform.localPosition = endPos;
        SetDraggable(true);
    }
}