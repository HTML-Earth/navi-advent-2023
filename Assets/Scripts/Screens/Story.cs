using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    public StoryExercise storyExercise;
    
    public AudioClip selectSound;
    public AudioClip correctPairSound;
    public AudioClip wrongPairSound;
    public AudioClip victorySound;

    AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
        if (!Settings.current.GetSoundEnabled())
            _source.mute = true;
    }
}
