using UnityEngine;

public class Settings: MonoBehaviour
{
    public static Settings current;
    public Ayopin ayopin;

    void Awake()
    {
        if (current == null)
            current = this;
    }
}