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
            Debug.Log("BeatEventSystem inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerBeat()
    {
        Debug.Log("TriggerBeat llamado!");
        OnBeat?.Invoke();
    }
}