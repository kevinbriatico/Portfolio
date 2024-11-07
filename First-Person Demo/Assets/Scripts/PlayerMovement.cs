using UnityEngine;

/// <summary>
/// Player body & camera movement: WASD + mouse input.
/// Includes coyote time and jump buffering
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings")]

    [SerializeField] float speed = 7.5f;
    [SerializeField] float jump = 1.75f;
    [SerializeField] float gravity = -45f;
    [SerializeField] float mouseSensitivity = 200f;
    public Vector3 velocity;
    public float xRotation = 0f;
    [Tooltip("Max distance for player to be considered grounded. Set to controller radius at start.")]
    [SerializeField] float checkDistance;
    [Tooltip("Layers assigned to collidable objects (ground & ceiling)")]
    [SerializeField] LayerMask checkMask;
    [SerializeField] float coyoteTime = .1f;
    float coyoteTimeCounter;
    [SerializeField] float jumpBufferTime = .1f;
    public float jumpBufferCounter;
    // Latest fall speed registered. Used to inflict bounce effect.
    public float lastFallSpeed;

    // Debug Settings
    [Header("Debug Settings")]

    [SerializeField] bool isGrounded;
    [SerializeField] bool canMove = true;
    [SerializeField] bool canWalk = true;
    [SerializeField] bool canLook = true;

    // References
    [Header("References")]

    // Used to check for collision w/ ceiling & ground
    [SerializeField] Transform headCheck;
    [SerializeField] Transform groundCheck;
    CharacterController controller;
    GameObject playerCamera;

    // Init references
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main.gameObject;
        // Same as character controller radius to avoid unseeing slopes
        // 'groundCheck' and 'headCheck' are positioned slightly outwards to detect collisions with ground and ceiling but not walls
        checkDistance = controller.GetComponent<CharacterController>().radius;
    }

    // Debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkDistance);
        Gizmos.DrawWireSphere(headCheck.position, checkDistance);
    }


    void Update()
    {
        if (canLook)
        {
            // Look
            float lookX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float lookY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= lookY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * lookX);
        }

        if (canMove)
        {
            // Ground collisions
            isGrounded = Physics.CheckSphere(groundCheck.position, checkDistance, checkMask);

            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;

                if (velocity.y < 0)
                {
                    velocity.y = -2f;
                }
            }
            else
            {
                lastFallSpeed = velocity.y; // Register latest fall speed

                coyoteTimeCounter -= Time.deltaTime;

                //Head collisions
                if (Physics.CheckSphere(headCheck.position, checkDistance, checkMask))
                {
                    velocity.y = -2f;
                }
            }

            // Jump
            if (Input.GetButtonDown("Jump"))
            {
                coyoteTimeCounter = 0f;
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
            {
                velocity.y = Mathf.Sqrt(jump * -2f * gravity);
                jumpBufferCounter = 0f;
            }

            // Movement
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            velocity.y += gravity * Time.deltaTime; // Gravity

            controller.Move(velocity * Time.deltaTime);

            if (canWalk)
            {
                controller.Move(move * speed * Time.deltaTime);
            }
        }
    }
}
