using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchPairs : MonoBehaviour
{
    public List<string> wordPairs;
    public Transform leftParent;
    public Transform rightParent;

    List<(string, string)> _wordPairs;

    GameObject _wordButton;

    void Awake()
    {
        _wordButton = Resources.Load<GameObject>("Prefabs/WordPairButton");

        _wordPairs = new List<(string, string)>();
        foreach (var pair in wordPairs)
        {
            var split = pair.Split(',');
            _wordPairs.Add((split[0], split[1]));
        }

        foreach (var wordPair in _wordPairs)
        {
            var leftWord = Instantiate(_wordButton, leftParent);
            var rightWord = Instantiate(_wordButton, rightParent);
            leftWord.transform.Find("pamrel").GetComponent<TextMeshProUGUI>().text = wordPair.Item1;
            rightWord.transform.Find("pamrel").GetComponent<TextMeshProUGUI>().text = wordPair.Item2;
        }
    }
}
