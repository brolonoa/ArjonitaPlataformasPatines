using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<Image> healthImages;
    [SerializeField] private TextMeshProUGUI coinText;

    private int currentCoins; 

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHp;
        CoinBehaviour.OnCoinCollected += AddCoin;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHp;
        CoinBehaviour.OnCoinCollected -= AddCoin;
    }

    private void AddCoin()
    {
        currentCoins++;
        coinText.text = currentCoins.ToString();
    }

    private void UpdateHp(int current, int max)
    {
        for (int i = 0; i < healthImages.Count; i++)
        {
            healthImages[i].enabled = i < current;
        }
    }
}
