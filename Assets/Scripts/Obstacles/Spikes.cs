using UnityEngine;

public class Spikes : MonoBehaviour, IParryable
{
    [SerializeField] bool canParry;

    [SerializeField] SpriteRenderer spriteRenderer;

    public bool CanBeParried => canParry;

    public void OnParry()
    {
        //
    }

    private void Awake()
    {
        UpdateColor();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageble idamagble = other.gameObject.GetComponent<IDamageble>();
        if (idamagble != null)
        {
            idamagble.TakeDamage();
        }
    }
      
    

   
    //public void SetCanParry(bool value)
    //{
    //    canParry = value;
    //    UpdateColor();
    //}

    private void UpdateColor()
    {
        if (canParry)
            spriteRenderer.color = Color.blue;
        else
            spriteRenderer.color = Color.red;
    }
}
