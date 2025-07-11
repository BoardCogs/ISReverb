using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 8f;

    [SerializeField] private float runSpeed = 12f;

    [SerializeField] private float jumpHeight = 2f;

    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float groundDistance = 0.4f;

    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private float jumpBufferTime = 0.15f;

    [SerializeField] private float terminalVelocity = -15f;

    private CharacterController controller;

    private Vector2 moveInput;

    private Vector3 velocity;

    private Transform groundCheck => transform.Find("GroundCheck");

    private float jumpSpeed;

    private bool inputJump;

    private bool isGrounded;

    private float jumpBuffer;

    private bool sprinting;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        jumpSpeed = Mathf.Sqrt(jumpHeight * -2 * gravity);
        //groundCheck = transform.Find("GroundCheck");
    }



    // Update is called once per frame
    void Update()
    {
        // Checks if the player is currently on ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayers);

        // MOVEMENT

        // Getting horizontal input
        float x = moveInput.x;
        float z = moveInput.y;
        Vector3 move = transform.right * x + transform.forward * z;

        // Applying horizontal velocity
        float moveSpeed = sprinting ? runSpeed : walkSpeed;
        controller.Move(moveSpeed * Time.deltaTime * move);



        // JUMP

        if (isGrounded)
        {
            // Makes character jump
            if (inputJump)
            {
                velocity.y = jumpSpeed;
                inputJump = false;
                jumpBuffer = 0;
            }
            // Keeps character grounded
            else if (velocity.y < 0f)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            // Reduce jump buffer
            jumpBuffer -= Time.deltaTime;
            if (jumpBuffer <= 0)
                inputJump = false;
        }

        // Applying gravity
        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, terminalVelocity, float.MaxValue);

        // Applying vertical velocity
        controller.Move(velocity * Time.deltaTime);
    }



    // Called by input system when movement buttons are pressed
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }



    // Called by input system when sprint buttons is pressed
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            sprinting = true;
        }
        else if (context.canceled)
        {
            sprinting = false;
        }
    }



    // Called by the input system when jump button is pressed
    public void OnJump(InputAction.CallbackContext context)
    {
        // Jump is buffered. The character will still jump if the button is pressed slightly before it touches the ground
        if (context.performed)
        {
            inputJump = true;
            jumpBuffer = jumpBufferTime;
        }
    }




    private void OnDrawGizmosSelected()   
    {
        // Draw a green sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }
}
