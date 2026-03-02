using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<Image> healthImages;
    [SerializeField] TextMeshProUGUI coinText;
    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateHp;
        CoinBehaviour.OnCoinChange += UpdateCoins;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateHp;
        CoinBehaviour.OnCoinChange -= UpdateCoins;
    }

    private void UpdateCoins(int currentCoins)
    {
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
