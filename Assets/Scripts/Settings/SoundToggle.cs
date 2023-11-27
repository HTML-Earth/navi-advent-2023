using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
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
        var soundIsEnabled = Settings.current.GetSoundEnabled();
        Settings.current.SetSoundEnabled(!soundIsEnabled);
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        var soundIsEnabled = Settings.current.GetSoundEnabled();
        _image.sprite = soundIsEnabled ? _onSprite : _offSprite;
        _text.text = soundIsEnabled ? $"Sound:\nEnabled" : $"Sound:\nDisabled";
    }
}
