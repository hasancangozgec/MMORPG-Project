using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float mouseSensitivity = 2f;
    public float scrollSensitivity = 2f;
    
    [Header("Camera Limits")]
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float minHeight = 0.5f;
    public float maxHeight = 5f;
    public float maxLookAngle = 80f;
    public float minLookAngle = -60f;
    
    [Header("Camera Smoothing")]
    public float rotationSmoothTime = 0.1f;
    public float positionSmoothTime = 0.1f;
    
    // Input
    private Vector2 lookInput;
    private float scrollInput;
    
    // Camera rotation
    private float yaw = 0f;
    private float pitch = 0f;
    
    // Smoothing
    private Vector3 currentVelocity;
    private float yawVelocity;
    private float pitchVelocity;
    
    // Camera collision
    private Camera cam;
    private float originalDistance;
    
    private void Awake()
    {
        cam = GetComponent<Camera>();
        originalDistance = distance;
    }
    
    private void Start()
    {
        // Initialize camera position if target is set
        if (target != null)
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
            
            UpdateCameraPosition();
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        HandleCameraInput();
        UpdateCameraPosition();
        HandleCameraCollision();
    }
    
    private void HandleCameraInput()
    {
        // Handle mouse look
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minLookAngle, maxLookAngle);
        
        // Handle scroll wheel for distance
        if (Mathf.Abs(scrollInput) > 0.1f)
        {
            distance -= scrollInput * scrollSensitivity;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }
    
    private void UpdateCameraPosition()
    {
        // Calculate desired rotation
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);
        
        // Smooth rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
            rotationSmoothTime > 0 ? Time.deltaTime / rotationSmoothTime : 1f);
        
        // Calculate desired position
        Vector3 targetPosition = target.position - transform.forward * distance + Vector3.up * height;
        
        // Smooth position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, 
            ref currentVelocity, positionSmoothTime);
    }
    
    private void HandleCameraCollision()
    {
        // Raycast from target to camera to check for obstacles
        Vector3 directionToCamera = (transform.position - target.position).normalized;
        float desiredDistance = Vector3.Distance(target.position, transform.position);
        
        RaycastHit hit;
        if (Physics.Raycast(target.position, directionToCamera, out hit, desiredDistance))
        {
            // Move camera closer if there's an obstacle
            float safeDistance = hit.distance - 0.1f;
            Vector3 safePosition = target.position + directionToCamera * safeDistance;
            transform.position = safePosition;
        }
    }
    
    // Input System Event Handlers
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void OnScroll(InputAction.CallbackContext context)
    {
        scrollInput = context.ReadValue<Vector2>().y;
    }
    
    // Public Methods
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        if (target != null && GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Camera target set to: {target.name}");
        }
    }
    
    public void SetCameraDistance(float newDistance)
    {
        distance = Mathf.Clamp(newDistance, minDistance, maxDistance);
    }
    
    public void ResetCamera()
    {
        distance = originalDistance;
        yaw = 0f;
        pitch = 0f;
        
        if (target != null)
        {
            UpdateCameraPosition();
        }
    }
    
    public void SetCameraMode(bool isFreeLook)
    {
        // Toggle between free look and locked camera modes
        if (isFreeLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}