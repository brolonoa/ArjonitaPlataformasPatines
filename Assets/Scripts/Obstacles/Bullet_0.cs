using UnityEngine;

public class Bullet_0 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _velocidad = -10f;
    [SerializeField] private float _tiempoDeVida = 10f;

    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb == null)
        {
            Debug.LogError($"[{gameObject.name}] No tiene Rigidbody2D!");
            Destroy(gameObject);
            return;
        }

        _rb.linearVelocity = transform.right * _velocidad;

        Destroy(gameObject, _tiempoDeVida);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Destroy(gameObject);
    //}
}