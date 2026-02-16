using UnityEngine;
using System;

public class BeatEventSystem : MonoBehaviour
{
    public static BeatEventSystem Instance { get; private set; }

    public event Action OnBeat;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerBeat()
    {
        OnBeat?.Invoke();
    }
}