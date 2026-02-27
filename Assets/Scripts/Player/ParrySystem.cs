using UnityEngine;
using System.Collections;

public class ParrySystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerData playerData;
    private PlayerController controller;
    private PlayerGroundDetection groundDetection;
    private Rigidbody2D rb;

    [Header("Parry Settings")]
    [SerializeField] private float parryWindow = 0.2f;
    [SerializeField] private float parryCooldown = 0.5f;
    [SerializeField] private float extraUpwardBoost = 2f;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color parryColor = Color.yellow;

    

    public bool isParryActive;
    private float parryTimer;
    private float cooldownTimer;
    private bool canParry = true;

    public event System.Action OnParrySuccess;
    public event System.Action OnParryFailed;

    private Animator playerAnim;
    void Awake()
    {
        controller = GetComponent<PlayerController>();
        groundDetection = GetComponent<PlayerGroundDetection>();
        playerData = controller.playerData;
        rb = GetComponent<Rigidbody2D>();
        playerAnim = controller.playerAnim;
    }

    void Update()
    {
        HandleParryInput();
        UpdateTimers();
    }

    void HandleParryInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryParry();
        }
    }

    void UpdateTimers()
    {
        if (isParryActive)
        {
            parryTimer -= Time.deltaTime;

            if (parryTimer <= 0)
            {
                isParryActive = false;
                OnParryFailed?.Invoke();
            }
        }

        if (!canParry)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
                canParry = true;
        }
    }

    void TryParry()
    {
        if (groundDetection.IsGrounded)
            return;

        if (!canParry)
            return;

        ActivateParry();
    }

    void ActivateParry()
    {
        playerAnim.SetTrigger("performParry");
        isParryActive = true;
        parryTimer = parryWindow;

        if (playerSprite != null)
            StartCoroutine(FlashSprite());
    }

    public void OnParryHit()
    {
        if (!isParryActive)
            return;
        playerAnim.SetTrigger("succesParry");
        PerformParryBoost();
    }

    void PerformParryBoost()
    {
        isParryActive = false;
        canParry = false;
        cooldownTimer = parryCooldown;

        float horizontalInput = Input.GetAxis("Horizontal");

        if (Mathf.Approximately(horizontalInput, 0f))
        {
            horizontalInput = Mathf.Sign(rb.linearVelocity.x);
            if (horizontalInput == 0)
                horizontalInput = 1f;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        Vector2 boost = new Vector2(
            horizontalInput * playerData.parryHorizontalForce,
            playerData.parryVerticalForce + extraUpwardBoost
        );

        rb.AddForce(boost, ForceMode2D.Impulse);

        OnParrySuccess?.Invoke();
    }
    Vector2 CalculateBoostDirection()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Mathf.Approximately(horizontalInput, 0f))
        {
            horizontalInput = Mathf.Sign(rb.linearVelocity.x);
            if (horizontalInput == 0)
                horizontalInput = 1f;
        }

        Vector2 direction = new Vector2(horizontalInput, 1f + extraUpwardBoost).normalized;
        return direction;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isParryActive)
            return;

        IParryable parryable = other.GetComponent<IParryable>();

        if (parryable != null && parryable.CanBeParried)
        {
            parryable.OnParry();
            OnParryHit();
        }
    }

    IEnumerator FlashSprite()
    {
        Color originalColor = playerSprite.color;
        playerSprite.color = parryColor;

        yield return new WaitForSeconds(parryWindow);

        playerSprite.color = originalColor;
    }

    public void ForceParryBoost()
    {
        PerformParryBoost();
    }

    public bool IsParryActive => isParryActive;
    public bool CanParry => canParry && !groundDetection.IsGrounded;
}
