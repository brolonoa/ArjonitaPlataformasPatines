using UnityEngine;

public class Fog : MonoBehaviour
{
    int damage = 400;
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageble idamagble = other.gameObject.GetComponent<IDamageble>();
        if (idamagble != null)
        {
            idamagble.TakeDamage(damage);
            
        }
        
    }

}
