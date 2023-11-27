using UnityEngine;

public class Settings: MonoBehaviour
{
    public static Settings current;
    public Ayopin ayopin;

    bool _soundIsEnabled;

    void Awake()
    {
        if (current == null)
            current = this;
        
        _soundIsEnabled = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
    }

    public bool GetSoundEnabled()
    {
        return _soundIsEnabled;
    }

    public void SetSoundEnabled(bool soundEnabled)
    {
        _soundIsEnabled = soundEnabled;
        PlayerPrefs.SetInt("SoundEnabled", _soundIsEnabled ? 1 : 0);
    }
}