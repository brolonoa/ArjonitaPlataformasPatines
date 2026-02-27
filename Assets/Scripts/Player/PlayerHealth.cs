using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageble
{
    [SerializeField] int maxHp;
    [SerializeField] int currentHp;

    private ParrySystem parrySystem;

    private void Start()
    {
        currentHp = maxHp;
        parrySystem = GetComponent<ParrySystem>();
    }
    public void TakeDamage()
    {
        if (!parrySystem.isParryActive)
        {
            currentHp--;
            if (currentHp <= 0)
            {
                Death();
            }
            else
            {
                LevelManager.Instance.OnDamageRecibed();
            }
        }

        
    }
    
    private void Death()
    {
        LevelManager.Instance.OnPlayerDeath();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IEInteractable eInteractable = other.GetComponent<IEInteractable>();
        if(eInteractable != null)
        {
            eInteractable.OnInteract();
        }
    }

}
