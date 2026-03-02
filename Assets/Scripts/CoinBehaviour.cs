using System;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour, IEInteractable
{
    private int currentCoins;
    public static event Action<int> OnCoinChange;
    public void OnInteract()
    {
        currentCoins++;
        OnCoinChange?.Invoke(currentCoins);
        Destroy(gameObject);
    }

    
}
