using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WordDropArea : MonoBehaviour, IDropHandler, IPointerMoveHandler, IPointerExitHandler
{
    public RectTransform dropVisual;
    public Image dropVisualImage;
    RectTransform _rectT;

    float _screenWidth;
    Lexeme _lexeme;
    TextMeshProUGUI _text;
    List<LexemeInstance> _droppedWords = new();
    List<float> _dropPositions = new();
    List<(int, int)> _dropIndexes = new();
    int _nextDropPosition;

    public List<LexemeInstance> DroppedWords() => _droppedWords;
    
    void Awake()
    {
        var margin = 100;
        _screenWidth = transform.parent.GetComponent<RectTransform>().rect.width - margin * 2;
        _rectT = GetComponent<RectTransform>();
        _dropPositions.Add(-_screenWidth * 0.5f);
        _dropIndexes.Add((-1,0));
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectT, eventData.position, null, out var relativeCursorPoint);
            var lexemeInstance = eventData.pointerDrag.GetComponent<LexemeInstance>();
            
            _nextDropPosition = _dropPositions.Count - 1;
            float lowestDistance = Mathf.Infinity;
            for (int i = 0; i < _dropPositions.Count; i++)
            {
                if (!CanDropWordHere(lexemeInstance.Lexeme, i))
                    continue;
                
                var newDistance = Mathf.Abs(_dropPositions[i] - relativeCursorPoint.x);
                if (newDistance < lowestDistance)
                {
                    lowestDistance = newDistance;
                    _nextDropPosition = i;
                }
            }

            dropVisualImage.enabled = true;
            dropVisual.localPosition = new Vector3(_dropPositions[_nextDropPosition], 25, 0);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            dropVisualImage.enabled = false;
        }
    }

    bool CanDropWordHere(Lexeme word, int index)
    {
        bool droppingOntoEmptySpace = _dropIndexes[index].Item1 == -1;
        if (droppingOntoEmptySpace)
        {
            return word.CanBeStandaloneWord();
        }
        else //dropping onto word
        {
            var wordIndexToDropInto = _dropIndexes[index].Item1;
            var slotIndexToDropInto = _dropIndexes[index].Item2;

            return _droppedWords[wordIndexToDropInto].Lexeme.SlotCanHoldLexeme(slotIndexToDropInto, word)
                && !_droppedWords[wordIndexToDropInto].Lexeme.SlotIsOccupied(slotIndexToDropInto);
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        dropVisualImage.enabled = false;
        var lexemeInstance = eventData.pointerDrag.GetComponent<LexemeInstance>();
        if (lexemeInstance != null)
        {
            if (_dropIndexes[_nextDropPosition].Item1 == -1)
            {
                if (lexemeInstance.Lexeme.CanBeStandaloneWord())
                {
                    _droppedWords.Insert(_dropIndexes[_nextDropPosition].Item2, lexemeInstance);
                
                    lexemeInstance.transform.SetParent(transform, true);
                    lexemeInstance.transform.SetSiblingIndex(0);
                    lexemeInstance.OnDroppedInDropArea(this);
                }
            }
            else
            {
                var wordIndexToDropInto = _dropIndexes[_nextDropPosition].Item1;
                var slotIndexToDropInto = _dropIndexes[_nextDropPosition].Item2;

                if (_droppedWords[wordIndexToDropInto].Lexeme
                    .SlotCanHoldLexeme(slotIndexToDropInto, lexemeInstance.Lexeme))
                {
                    // update inserted lexeme first to render in correct order
                    lexemeInstance.OnDroppedOntoWord();
                    lexemeInstance.Lexeme.OnInserted(_droppedWords[wordIndexToDropInto].Lexeme);
                    _droppedWords[wordIndexToDropInto].Lexeme.InsertLexeme(slotIndexToDropInto, lexemeInstance.Lexeme);
                    _droppedWords[wordIndexToDropInto].OnInsertedLexeme(slotIndexToDropInto, lexemeInstance.Lexeme);
                    
                    Destroy(lexemeInstance.gameObject);
                }
            }
            
            ReflowWords();
        }
    }

    public void RemoveWord(LexemeInstance lexemeInstance)
    {
        _droppedWords.Remove(lexemeInstance);
        ReflowWords();
    }

    void ReflowWords()
    {
        float x = -_screenWidth * 0.5f;
        int wordIndex = 0;

        _dropIndexes.Clear();
        _dropIndexes.Add((-1,0));
        
        _dropPositions.Clear();
        _dropPositions.Add(x);
        
        foreach (LexemeInstance word in _droppedWords)
        {
            int slotCount = word.Lexeme.GetSlotCount();
            var wordWidth = MoveWordAndReturnWidth(word, x);

            if (slotCount > 0)
            {
                var slotSpacing = wordWidth / (slotCount + 1);
                for (int i = 0; i < slotCount; i++)
                {
                    _dropIndexes.Add((wordIndex, i));
                    _dropPositions.Add(x + slotSpacing * (i+1));
                }
            }

            x += wordWidth;
            wordIndex++;
            _dropIndexes.Add((-1, wordIndex));
            _dropPositions.Add(x);
        }
    }

    float MoveWordAndReturnWidth(LexemeInstance word, float x)
    {
        StartCoroutine(word.MoveWord(word.transform.localPosition, new Vector3(x, 0, 0)));
        return word.Width + 20f;
    }
}