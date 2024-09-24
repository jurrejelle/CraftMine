using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera playerCamera;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float survivalWalkSpeed = 5.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private float creativeWalkSpeed = 10.0f;
    private float flySpeed = 4.0f;
    private float mouseSensitivity = 500f;

    private string gameMode = "survival";
    private float xRotation = 0f;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
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
        CheckGamemode();
        DestroyBlock();
        PlaceBlock();
    }

    void DestroyBlock(){
        if(Input.GetButtonDown("Fire1")) {
            // Create a ray that starts from the camera and points forward
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            // Perform the raycast from the camera's position forward
            if (Physics.Raycast(ray, out hit, 10f))
            {
                // Print the position where the ray hit
                Vector3Int in_block = Vector3Int.FloorToInt(hit.point + 0.1f * Camera.main.transform.forward);
                World world = WorldManager.GetManager().world;
                Debug.Log($"Hit position inside block: {in_block}, type {world.GetBlock(in_block)}");
                world.DestroyBlock(in_block);
            }
        }
    }

    void PlaceBlock(){
        if(Input.GetButtonDown("Fire2")) {
            // Create a ray that starts from the camera and points forward
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            // Perform the raycast from the camera's position forward
            if (Physics.Raycast(ray, out hit, 10f))
            {
                // Print the position where the ray hit
                Vector3Int in_block = (hit.point - 0.1f * Camera.main.transform.forward).floor();
                World world = WorldManager.GetManager().world;
                Debug.Log($"Hit position inside block: {in_block}, type {world.GetBlock(in_block)}");
                world.PlaceBlock(in_block, 3);
            }
        }

    }

    void CheckGamemode(){
        if(Input.GetKeyDown(KeyCode.C)){
            gameMode = gameMode == "survival" ? "creative" : "survival";
        }
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
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    void Walk(){
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // Transform the movement direction from local space to world space based on the player's current rotation
        Vector3 transformedMove = transform.TransformDirection(move);
        float movementSpeed = gameMode == "creative" ? creativeWalkSpeed : survivalWalkSpeed;
        controller.Move(transformedMove * Time.deltaTime * movementSpeed);
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

            playerVelocity.y = heightVelocity.y * flySpeed;

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

            playerVelocity.y += gravityValue * Time.deltaTime; // TODO: maybe remove this deltatime bcs square???
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
