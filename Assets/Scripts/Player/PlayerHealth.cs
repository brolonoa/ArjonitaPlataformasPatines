using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageble
{
    [SerializeField] int maxHp;
    [SerializeField] int currentHp;

    public static event Action<int, int> OnHealthChanged;
    // currentHp, maxHp

    private ParrySystem parrySystem;

    private void Start()
    {
        currentHp = maxHp;
        parrySystem = GetComponent<ParrySystem>();

        OnHealthChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage()
    {
        if (!parrySystem.isParryActive)
        {
            currentHp--;
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);

            OnHealthChanged?.Invoke(currentHp, maxHp);

            if (currentHp <= 0)
            {
                Death();
            }
        }
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        OnHealthChanged?.Invoke(currentHp, maxHp);
    }

    private void Death()
    {
        LevelManager.Instance.OnPlayerDeath();

        currentHp = maxHp;
        OnHealthChanged?.Invoke(currentHp, maxHp);
    }
    private void OnTriggerEnter2D(Collider2D other) 
    { 
        IEInteractable eInteractable = other.GetComponent<IEInteractable>();

        if (eInteractable != null) 
        { 
            eInteractable.OnInteract(); 
        } 
    }

}
