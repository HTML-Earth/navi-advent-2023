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

    void Awake()
    {
        _buttonPrefab = Resources.Load<GameObject>("Prefabs/ExerciseButton");
        
        foreach (var scene in scenes)
        {
            var button = Instantiate(_buttonPrefab, parent);
            button.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = scene;
            button.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(scene));
        }
    }
}