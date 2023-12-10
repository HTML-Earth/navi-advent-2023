using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LexemeInstance : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    RectTransform _rectTransform;
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
    Dictionary<int, LexemeHandle> _insertedLexemeHandles = new();

    struct LexemeHandle
    {
        public RectTransform t;
        public Image image;
        public Color bg;
        public Color hover;
        public Color dragging;
    }

    int _hoverState = -1; // -1 = word itself, 0,1,2... are inserted lexes
    
    bool _isDraggable;
    bool _isInDropArea;

    const int SlotOffsetY = 50;
    
    public Lexeme Lexeme => _lexeme;
    public float Width => _width;

    Color _bgColor;
    Color _bgHoverColor;
    Color _bgDraggingColor;

    void Awake()
    {
        _wordPrefab = Resources.Load<GameObject>("Prefabs/Word");
        _slotPrefab = Resources.Load<GameObject>("Prefabs/slot");
        _text = transform.Find("text").GetComponent<TextMeshProUGUI>();
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetLexeme(Lexeme lexeme, SentenceAssemble sentenceAssemble)
    {
        _sentenceAssemble = sentenceAssemble;
        _lexeme = Instantiate(lexeme);
        _slotCount = _lexeme.GetSlotCount();
        
        _text.text = _lexeme.CanBeStandaloneWord() ? _lexeme.Render() : _lexeme.Root();

        _bgColor = Settings.current.ayopin.DefaultColorFromLexeme(lexeme);
        _bgHoverColor = Settings.current.ayopin.HoverColorFromLexeme(lexeme);
        _bgDraggingColor = Settings.current.ayopin.DraggingColorFromLexeme(lexeme);

        _image.color = _bgColor;
    }

    public void OnInsertedLexeme(int slot, Lexeme lex)
    {
        _insertedLexemes.Add(slot, lex);
        Refresh();
    }

    public void Refresh()
    {
        _text.text = _lexeme.CanBeStandaloneWord() ? _lexeme.Render() : _lexeme.Root();
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
                    if (_insertedLexemeHandles.TryGetValue(i, out var handle))
                    {
                        handle.t.localPosition = new Vector3(slotPos, SlotOffsetY, 0);
                    }
                    continue;
                }
                
                var slot = Instantiate(_slotPrefab, transform);
                slot.transform.localPosition = new Vector3(slotPos, SlotOffsetY, 0);
                
                var handleColor = Settings.current.ayopin.DefaultColorFromLexeme(_insertedLexemes[i]);
                var handleHoverColor = Settings.current.ayopin.HoverColorFromLexeme(_insertedLexemes[i]);
                var handleDraggingColor = Settings.current.ayopin.DraggingColorFromLexeme(_insertedLexemes[i]);
                
                _insertedLexemeHandles.Add(i,
                    new LexemeHandle
                    {
                        t = slot.GetComponent<RectTransform>(),
                        image = slot.GetComponent<Image>(),
                        bg = handleColor,
                        hover = handleHoverColor,
                        dragging = handleDraggingColor
                    });

                _insertedLexemeHandles[i].image.color = handleColor;
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
        transform.SetParent(null);
        _sentenceAssemble.ReflowWords();
        _sentenceAssemble.OnClick();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
            return;

        if (_hoverState == -1)
        {
            _sentenceAssemble.OnStartDrag();
            
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
            _sentenceAssemble.OnUnClick();
            
            GameObject draggedOutWord = Instantiate(_wordPrefab, _sentenceAssemble.wordBankArea);
            draggedOutWord.GetComponent<Image>().raycastTarget = false;
            var lex = draggedOutWord.GetComponent<LexemeInstance>();
            lex.SetLexeme(_insertedLexemes[_hoverState], _sentenceAssemble);
            lex.SetDraggable(true);
            lex.Refresh();
            
            draggedOutWord.transform.position = transform.position;
            eventData.pointerDrag = draggedOutWord;

            Destroy(_insertedLexemeHandles[_hoverState].t.gameObject);
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
        
        _image.color = _bgDraggingColor;
        foreach (var handle in _insertedLexemeHandles)
        {
            handle.Value.image.color = handle.Value.dragging;
        }
        
        transform.position = eventData.position - _dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDraggable)
            return;

        if (!_isInDropArea)
            _sentenceAssemble.OnWordDroppedOutsideDropArea(this);
        else
        {
            _sentenceAssemble.OnDrop();
        }
        
        _image.color = _bgColor;
        foreach (var handle in _insertedLexemeHandles)
        {
            handle.Value.image.color = handle.Value.bg;
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
        foreach (var handle in _insertedLexemeHandles)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(handle.Value.t, cursorPos))
            {
                hoveringOverHandle = true;
                _hoverState = handle.Key;
                handle.Value.image.color = handle.Value.hover;
            }
            else
            {
                handle.Value.image.color = handle.Value.bg;
            }
        }

        if (hoveringOverHandle)
        {
            _image.color = _bgColor;
            return;
        }
        
        _hoverState = -1;
        _image.color = _bgHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverState = -1;
        _image.color = _bgColor;
        foreach (var handle in _insertedLexemeHandles)
        {
            handle.Value.image.color = handle.Value.bg;
        }
    }

    public void SetWordPos(Vector3 pos)
    {
        _rectTransform.anchoredPosition = pos;
    }
    
    public Vector2 GetWordPos()
    {
        return _rectTransform.anchoredPosition;
    }
    
    public IEnumerator MoveWord(Vector3 startPos, Vector3 endPos)
    {
        float t = 0;
        float mul = 4;
        while (t <= 0.25f)
        {
            _rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t * mul);
            yield return null;
            t += Time.deltaTime;
        }

        _rectTransform.anchoredPosition = endPos;
        SetDraggable(true);
    }
}