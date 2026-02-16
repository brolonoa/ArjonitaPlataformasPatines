using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Slide();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {

        }
    }

    private void Jump()
    {

    }
    private void Slide()
    {

    }
}
