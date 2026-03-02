using System;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour, IEInteractable
{
    public static event Action OnCoinCollected;

    public void OnInteract()
    {
        OnCoinCollected?.Invoke();
        Destroy(gameObject);
    }


}
