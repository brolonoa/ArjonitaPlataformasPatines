using UnityEngine;

public class PlayerGroundDetection : MonoBehaviour
{
   private PlayerController controller;
   private PlayerData playerData;

    public bool IsGrounded { get; private set; }
    public float LastGroundedTime { get; private set; }
    private bool wasGrounded;

    [SerializeField] LayerMask goroundLayer;
    private void Start()
    {
        controller = GetComponent<PlayerController>();
        playerData = controller.playerData;
    }

    void Update()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        wasGrounded = IsGrounded;

        Vector2 rayOrigin = (Vector2)transform.position + playerData.groundCheckOffset;
        RaycastHit2D hit =  Physics2D.Raycast(rayOrigin, Vector3.down, playerData.groundCheckDistance, goroundLayer);

        IsGrounded = hit.collider != null;

        if (IsGrounded)
        {
            LastGroundedTime = Time.time;
        }

        if(IsGrounded && !wasGrounded)
        {
            //Land
        }
        else if(!IsGrounded && wasGrounded)
        {
            //dejo el suelo
        }
    }
}
