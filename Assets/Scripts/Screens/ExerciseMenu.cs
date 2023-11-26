using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExerciseMenu : MonoBehaviour
{
    public List<string> scenes;
    public Transform parent;
    GameObject _buttonPrefab;
    
    const int HighestUnlockedIndex = 2;

    void Awake()
    {
        _buttonPrefab = Resources.Load<GameObject>("Prefabs/ExerciseButton");

        var i = 0;
        foreach (var scene in scenes)
        {
            var button = Instantiate(_buttonPrefab, parent);
            button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = scene;
            
            if (i > HighestUnlockedIndex)
                button.GetComponent<Button>().interactable = false;
            else
                button.GetComponent<Button>().onClick.AddListener(() => OpenExercise(scene));

            i++;
        }
    }

    public void OpenExercise(string exerciseName)
    {
        SceneManager.LoadScene(exerciseName);
    }
}