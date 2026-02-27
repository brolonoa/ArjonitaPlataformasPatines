using UnityEngine;

public class DronEnemy : MonoBehaviour, IParryable
{
    

    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private float deactivatedTime;

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private bool canBeParried;
    public bool CanBeParried => canBeParried;
    private void Start()
    {
        currentHealth = maxHealth;
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider.enabled = true;
    }
    

    public void OnParry()
    {
        currentHealth--;
        if (currentHealth <= 0)
        {
            Death();
        }
        else
        {
            //cambiar el color 
        }
    }

    private void Death()
    {
        boxCollider.enabled = false;
        spriteRenderer.enabled = false;
        Invoke("Activate", deactivatedTime);
    }
    private void Activate()
    {
        boxCollider.enabled = true;
        spriteRenderer.enabled = true;
    }
}
