using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera camera;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float flySpeed = 4.0f;
    private float gravityValue = -9.81f;
    private float mouseSensitivity = 200f;

    private string gameMode = "creative";
    private float xRotation = 0f;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move(){
        Turn();
        Walk();
        Jump();
    }

    void Turn(){
        // Get the mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the player body left and right (yaw) based on Mouse X input
        transform.Rotate(Vector3.up * mouseX);

        // Calculate the up/down rotation (pitch) and clamp it so the camera doesn't flip
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Limit the up/down rotation to prevent over-rotation

        // Apply the rotation to the camera around the local X axis
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    void Walk(){
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // Transform the movement direction from local space to world space based on the player's current rotation
        Vector3 transformedMove = transform.TransformDirection(move);
        controller.Move(transformedMove * Time.deltaTime * playerSpeed);
    }

    void Jump(){
        if (gameMode == "creative") {
            Vector3 heightVelocity = new Vector3();
            
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                heightVelocity += Vector3.down;
            }

            if (Input.GetButton("Jump")) {
                heightVelocity += Vector3.up;
            }

            playerVelocity.y = heightVelocity.y;

        } else if(gameMode == "survival") {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            // Changes the height position of the player..
            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
