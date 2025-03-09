using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraInputActions cameraActions;
    private InputAction movement;
    private Transform cameraTransform;
    private Camera mainCamera;
    private float originalSize;

    private float speed = 0;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float damping = 10f;

    [SerializeField] private float stepSize = 25f;
    [SerializeField] private float minSize = 50f;
    [SerializeField] private float maxSize = 300f;
    [SerializeField] private float zoomSpeed = 5f;

    private Vector3 moveDir;

    private float zoomSize;

    private Vector3 velocity;
    private Vector3 lastPosition;

    private void Awake()
    {
        cameraActions = new CameraInputActions();
        mainCamera = this.GetComponentInChildren<Camera>();
        cameraTransform = mainCamera.transform;
        originalSize = mainCamera.orthographicSize;
    }

    private void OnEnable()
    {
        zoomSize = originalSize;

        lastPosition = this.transform.position;

        movement = cameraActions.Camera.Movement;
        cameraActions.Camera.Zoom.performed += ZoomCamera;
        cameraActions.Camera.Enable();
    }

    private void OnDisable()
    {
        cameraActions.Camera.Zoom.performed -= ZoomCamera;
        cameraActions.Camera.Disable();
    }

    private void Update()
    {
        //update target position based on input
        GetKeyboardMovement();
        //move objects based on target position
        UpdateVelocity();
        UpdateCameraPosition();
        UpdateCameraZoom();
    }

    private void UpdateVelocity()
    {
        velocity = (this.transform.position - lastPosition) / Time.deltaTime;
        velocity.z = 0f;
        lastPosition = this.transform.position;
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight()
                           + movement.ReadValue<Vector2>().y * GetCameraForward();

        inputValue = inputValue.normalized;
        
        moveDir = inputValue;
    }

    private void UpdateCameraPosition()
    {
        if (moveDir.sqrMagnitude > 0.1f)
        {
            //create a ramp up or acceleration
            speed = Mathf.Lerp(speed, maxSpeed * zoomSize / 10, Time.deltaTime * acceleration);
            transform.position += moveDir * speed * Time.deltaTime;
        }
        else
        {
            //create smooth slow down
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += velocity * Time.deltaTime;
            speed = 0;
        }

        //reset for next frame
        moveDir = Vector3.zero;
    }

    private void ZoomCamera(InputAction.CallbackContext obj)
    {
        float inputValue = -obj.ReadValue<Vector2>().y / 120f;
        if (Mathf.Abs(inputValue) > 0.1f)
        {
            zoomSize = zoomSize + inputValue * stepSize;

            if (zoomSize < minSize)
                zoomSize = minSize;
            else if (zoomSize > maxSize)
                zoomSize = maxSize;
        }
    }

    private void UpdateCameraZoom()
    {
        //set zoom target
        float zoomTarget = zoomSize;
        if (zoomTarget != mainCamera.orthographicSize)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, zoomTarget, Time.deltaTime * zoomSpeed);
        }
    }

    //gets the up vector of the camera
    private Vector3 GetCameraForward()
    {
        Vector3 up = cameraTransform.up;
        up.z = 0f;
        return up;
    }

    //gets the right vector of the camera
    private Vector3 GetCameraRight()
    {
        Vector3 right = cameraTransform.right;
        right.z = 0f;
        return right;
    }
}
