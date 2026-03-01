using UnityEngine;

public class Bullet_0 : MonoBehaviour, IParryable
{
    [Header("Settings")]
    [SerializeField] private float _velocidad = 10f;
    [SerializeField] private float _tiempoDeVida = 10f;
    [SerializeField] float damage;

    private Rigidbody2D _rb;
    private Vector2 _direction = Vector2.right;

    [SerializeField] private bool canBeParried;
    public bool CanBeParried => canBeParried;

    public void SetDirection(Vector2 dir)
    {
        _direction = dir.normalized;
    }

    public void OnParry()
    {
        if (canBeParried)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb == null)
        {
            Destroy(gameObject);
            return;
        }

        _rb.linearVelocity = _direction * _velocidad;

        Destroy(gameObject, _tiempoDeVida);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        IDamageble idamagble = other.gameObject.GetComponent<IDamageble>();

        if (idamagble != null)
        {
            idamagble.TakeDamage();
        }

        Destroy(gameObject);
    }
}