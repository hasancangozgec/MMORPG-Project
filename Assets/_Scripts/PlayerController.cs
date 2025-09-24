using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.1f;
    
    [Header("Camera Settings")]
    public Transform cameraTarget;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;
    
    [Header("Ground Check")]
    public LayerMask groundMask = 1;
    
    // Components
    private CharacterController characterController;
    private PlayerInput playerInput;
    private Camera playerCamera;
    
    // Input
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isRunning;
    private bool jumpPressed;
    
    // Movement
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    
    // Camera
    private float xRotation = 0f;
    
    // Character Data
    private CharacterData characterData;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        
        // Find camera if not assigned
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
        }
        
        // Setup camera target if not assigned
        if (cameraTarget == null)
        {
            GameObject cameraTargetObj = new GameObject("CameraTarget");
            cameraTargetObj.transform.SetParent(transform);
            cameraTargetObj.transform.localPosition = new Vector3(0, 1.8f, 0);
            cameraTarget = cameraTargetObj.transform;
        }
    }
    
    private void Start()
    {
        // Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Position camera
        if (playerCamera != null && cameraTarget != null)
        {
            playerCamera.transform.position = cameraTarget.position + Vector3.back * 5f;
            playerCamera.transform.LookAt(cameraTarget);
        }
    }
    
    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleCameraRotation();
        HandleJump();
    }
    
    private void HandleGroundCheck()
    {
        // Check if player is grounded
        Vector3 spherePosition = transform.position - new Vector3(0, characterController.height / 2, 0);
        isGrounded = Physics.CheckSphere(spherePosition, groundCheckDistance, groundMask);
        
        // Reset velocity if grounded and falling
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep grounded
        }
    }
    
    private void HandleMovement()
    {
        // Calculate movement direction
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        // Determine current speed
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Apply movement
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    
    private void HandleCameraRotation()
    {
        if (playerCamera == null || cameraTarget == null) return;
        
        // Rotate player horizontally
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
        
        // Rotate camera vertically
        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        
        // Apply camera rotation and position
        playerCamera.transform.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0);
        playerCamera.transform.position = cameraTarget.position + playerCamera.transform.rotation * Vector3.back * 5f;
    }
    
    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }
    }
    
    // Input System Event Handlers
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformAttack();
        }
    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformInteraction();
        }
    }
    
    // Action Methods
    private void PerformAttack()
    {
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Player attacked!");
        }
        
        // TODO: Implement attack logic
    }
    
    private void PerformInteraction()
    {
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Player interacted!");
        }
        
        // TODO: Implement interaction logic
    }
    
    // Character Data
    public void SetCharacterData(CharacterData data)
    {
        characterData = data;
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Player controller set for character: {data.characterName}");
        }
    }
    
    public CharacterData GetCharacterData()
    {
        return characterData;
    }
    
    // Utility Methods
    public bool IsMoving()
    {
        return moveInput.magnitude > 0.1f;
    }
    
    public bool IsRunning()
    {
        return isRunning && IsMoving();
    }
    
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw ground check sphere
        Vector3 spherePosition = transform.position - new Vector3(0, characterController?.height / 2 ?? 1f, 0);
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(spherePosition, groundCheckDistance);
    }
}