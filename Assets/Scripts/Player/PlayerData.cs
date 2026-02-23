using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/PlayerData")]

public class PlayerData : ScriptableObject 
{
    [Header("Ground Check")]
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);
    public float groundCheckRadius = 0.25f;
    public float groundCheckDistance = 0.1f;

    [Header("Slope Settings")]
    public float maxGroundAngle = 60f;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float airControl = 0.7f;

    public float groundFriction;
    public float airFriction;

    public float idleFriction = 20f;   // freno al soltar input
    public float turnFriction = 10f;

    [Header("Jump")]
    public float gravityScale;
    public float jumpForce = 14f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Speed System")]
    public float baseMaxSpeed = 12f;
    public float slopeSpeedBonus = 6f;
    public float crouchSlopeBonus = 8f;
    public float momentumDecayRate = 6f;
    public float softCapAccelerationMultiplier = 0.5f;
    public float crouchFlatBonus = 1.5f;
    [Header("Crouch System")]
    [Tooltip("Fuerza del boost inicial al agacharse")]
    public float crouchBoostForce = 5f;

    [Tooltip("Multiplicador de aceleración mientras está agachado")]
    [Range(0.3f, 1f)]
    public float crouchAccelerationMultiplier = 0.6f;

    [Header("Parry System")]
 
    public float parryBoostForce = 18f;

}
