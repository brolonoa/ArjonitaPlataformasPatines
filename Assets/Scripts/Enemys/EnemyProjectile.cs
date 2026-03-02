using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IParryable
{

    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] int damage =1;

    private Vector2 direction;
    [SerializeField] private bool canBeParried;
    public bool CanBeParried => canBeParried;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        IDamageble idamagble = other.gameObject.GetComponent<IDamageble>();
        if (idamagble != null)
        {
            idamagble.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public void OnParry()
    {
        if (canBeParried)
        {
            Destroy(gameObject);
        }
    }
}
