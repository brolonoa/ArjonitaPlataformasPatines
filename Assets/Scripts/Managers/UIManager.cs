using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<Image> healthImages;

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateUI;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateUI;
    }

    private void UpdateUI(int current, int max)
    {
        for (int i = 0; i < healthImages.Count; i++)
        {
            healthImages[i].enabled = i < current;
        }
    }
}
