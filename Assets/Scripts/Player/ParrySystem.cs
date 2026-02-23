using UnityEngine;
using System.Collections;

/// <summary>
/// Sistema de Parry aéreo.
/// Permite "saltar" de nuevo en el aire al hacer parry exitoso.
/// </summary>
public class ParrySystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerData playerData;
    private PlayerController controller;
    private PlayerGroundDetection groundDetection;
    private Rigidbody2D rb;

    [Header("Parry Settings")]
    [Tooltip("Ventana de tiempo en la que el parry está activo")]
    [SerializeField] private float parryWindow = 0.2f;

    [Tooltip("Cooldown entre parries")]
    [SerializeField] private float parryCooldown = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject parryVFX; // Efecto visual opcional

    // === ESTADO ===
    private bool isParryActive;
    private float parryTimer;
    private float cooldownTimer;
    private bool canParry = true;

    // === EVENTOS ===
    public event System.Action OnParrySuccess;
    public event System.Action OnParryFailed;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color parryColor = Color.yellow;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        groundDetection = GetComponent<PlayerGroundDetection>();
        playerData = controller.playerData;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleParryInput();
        UpdateTimers();
    }

    void HandleParryInput()
    {
        // Input de parry (botón izquierdo del mouse o botón de gamepad)
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryParry();
        }
    }

    void UpdateTimers()
    {
        // Countdown de ventana de parry
        if (isParryActive)
        {
            parryTimer -= Time.deltaTime;

            if (parryTimer <= 0)
            {
                isParryActive = false;
                OnParryFailed?.Invoke();
            }
        }

        // Countdown de cooldown
        if (!canParry)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                canParry = true;
            }
        }
    }

    void TryParry()
    {
        // Solo se puede hacer parry en el aire
        if (groundDetection.IsGrounded)
        {
            Debug.Log("Parry: En el suelo, no se puede hacer parry aéreo");
            return;
        }

        // Verificar cooldown
        if (!canParry)
        {
            Debug.Log("Parry: En cooldown");
            return;
        }

        // Activar ventana de parry
        ActivateParry();
    }

    void ActivateParry()
    {
        isParryActive = true;
        parryTimer = parryWindow;

        Debug.Log("Parry activado! Ventana de " + parryWindow + "s");

        if (playerSprite != null)
        {
            StartCoroutine(FlashSprite());
        }
    }

    public void OnParryHit()
    {
        if (!isParryActive)
            return;

        // ¡Parry exitoso!
        PerformParryBoost();
    }

    void PerformParryBoost()
    {
        isParryActive = false;
        canParry = false;
        cooldownTimer = parryCooldown;

        // Calcular dirección del boost
        Vector2 boostDirection = CalculateBoostDirection();

        // Resetear velocidad Y (como un salto)
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        // Aplicar boost
        rb.AddForce(boostDirection * playerData.parryBoostForce, ForceMode2D.Impulse);

        // Disparar evento
        OnParrySuccess?.Invoke();

        Debug.Log($"¡PARRY EXITOSO! Boost aplicado en dirección {boostDirection}");
    }

    Vector2 CalculateBoostDirection()
    {
        // Dirección basada en input del jugador
        float horizontalInput = Input.GetAxis("Horizontal");

        // Si no hay input, usar dirección de facing actual
        if (Mathf.Approximately(horizontalInput, 0f))
        {
            horizontalInput = Mathf.Sign(rb.linearVelocity.x);

            // Si tampoco hay velocidad, usar dirección hacia adelante por defecto
            if (horizontalInput == 0)
                horizontalInput = 1f;
        }

        // Vector normalizado con componente vertical fija
        Vector2 direction = new Vector2(horizontalInput, 1f).normalized;

        return direction;
    }

    /// <summary>
    /// Llamado por OnTriggerEnter2D del jugador cuando detecta proyectil.
    /// </summary>
    void OnTriggerStay2D(Collider2D other)
    {
        // Solo reaccionar durante ventana de parry
        if (!isParryActive)
            return;

        // Verificar si es un proyectil o enemigo parryable
        if (other.CompareTag("Projectile") || other.CompareTag("ParryableEnemy"))
        {
            OnParryHit();

            // Destruir proyectil (opcional)
            if (other.CompareTag("Projectile"))
            {
                Destroy(other.gameObject);
            }
        }
    }
    IEnumerator FlashSprite()
    {
        Color originalColor = playerSprite.color;
        playerSprite.color = parryColor;

        yield return new WaitForSeconds(parryWindow);

        playerSprite.color = originalColor;
    }

    #region PUBLIC API

    /// <summary>
    /// Forzar parry exitoso desde código externo.
    /// Útil para testing o eventos especiales.
    /// </summary>
    public void ForceParryBoost()
    {
        PerformParryBoost();
    }

    /// <summary>
    /// ¿Está la ventana de parry activa?
    /// </summary>
    public bool IsParryActive => isParryActive;

    /// <summary>
    /// ¿Puede hacer parry? (no en cooldown)
    /// </summary>
    public bool CanParry => canParry && !groundDetection.IsGrounded;

    #endregion

    #region DEBUG

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        // Dibujar estado de parry
        if (isParryActive)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
        else if (!canParry)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }


    #endregion
}
