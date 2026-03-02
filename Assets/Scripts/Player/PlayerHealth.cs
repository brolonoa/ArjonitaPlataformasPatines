using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerHealth : MonoBehaviour, IDamageble
{
    [SerializeField] int maxHp;
    [SerializeField] int currentHp;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isInvulnerable = false;

    public static event Action<int, int> OnHealthChanged;
    // currentHp, maxHp

    private ParrySystem parrySystem;

    private void Start()
    {
        currentHp = maxHp;
        parrySystem = GetComponent<ParrySystem>();

        OnHealthChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        if (!parrySystem.isParryActive)
        {
            currentHp -= damage;
            currentHp = Mathf.Clamp(currentHp, 0, maxHp);

            OnHealthChanged?.Invoke(currentHp, maxHp);

            if (currentHp <= 0)
            {
                Death();
            }
            else
            {
                StartCoroutine(InvulnerabilityCoroutine());
            }
        }
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        OnHealthChanged?.Invoke(currentHp, maxHp);
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        float elapsed = 0f;

        while (elapsed < invulnerabilityTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true;
        isInvulnerable = false;
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
