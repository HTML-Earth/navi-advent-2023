using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    bool _soundIsEnabled;

    Image _image;
    TextMeshProUGUI _text;

    Sprite _onSprite;
    Sprite _offSprite;

    void Awake()
    {
        _image = transform.GetComponent<Image>();
        _text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _onSprite = Resources.Load<Sprite>("Sprites/speaker_on");
        _offSprite = Resources.Load<Sprite>("Sprites/speaker_off");
        
        UpdateVisuals();
    }

    public void ToggleSound()
    {
        _soundIsEnabled = !_soundIsEnabled;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        _image.sprite = _soundIsEnabled ? _onSprite : _offSprite;
        _text.text = _soundIsEnabled ? $"Sound:\nEnabled" : $"Sound:\nDisabled";
    }
}
