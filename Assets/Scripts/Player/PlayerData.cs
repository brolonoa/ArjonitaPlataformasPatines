using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/PlayerData")]

public class PlayerData : ScriptableObject 
{
    [Header("Ground Check")]
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    public float groundCheckDistance = 0.55f;

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

}
