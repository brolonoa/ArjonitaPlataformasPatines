using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/PlayerData")]

public class PlayerData : ScriptableObject
{

    [Header("Fricción")]
    [Tooltip("Fricción cuando no hay input")]
    public float idleFriction = 35f;

    [Tooltip("Fricción extra al cambiar de dirección")]
    public float turnFriction = 45f;

    [Header("Gravedad")]
    [Tooltip("Multiplicador de gravedad")]
    public float gravityScale = 4f;


    public float parryHorizontalForce = 10f;
    public float parryVerticalForce = 15f;
    [Header("Rampas - CLAVE PARA EL SLIDE")]
    [Tooltip("Fuerza que mantiene al jugador pegado a la rampa")]
    public float slopeStickForce = 25f;

    [Tooltip("Multiplicador de velocidad en bajada")]
    public float slopeDownSpeedMultiplier = 1.3f;

    [Tooltip("Multiplicador de velocidad en subida")]
    public float slopeUpSpeedMultiplier = 0.7f;

    [Tooltip("Ángulo mínimo para considerar rampa (grados)")]
    public float slopeMinAngle = 5f;

    [Tooltip("Ángulo máximo que puede subir (grados)")]
    public float slopeMaxAngle = 60f;

    [Header("Sistema de Soft Cap (Control de velocidad)")]
    [Tooltip("Velocidad base máxima (soft cap)")]
    public float baseMaxSpeed = 16f;

   

    

    [Header("Parry System")]
 
    public float parryBoostForce = 18f;



    [Header("Movimiento")]
    public float maxSpeed = 10f;
    public float acceleration = 10f;
    public float airControl = 0.5f;

    [Header("Salto")]
    public float jumpForce = 15f;
    public float jumpBufferTime = 0.2f;
    public float coyoteTime = 0.15f;
    public float jumpCutMultiplier = 0.5f;

    [Header("NUEVO: Deslizamiento")]
    public float slideSpeedMultiplier = 1.5f; // Multiplicador de velocidad al deslizar
    public float slideSlopeBoost = 2f; // Boost adicional en pendientes

}
