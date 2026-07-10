using SDG.Unturned;
using UnityEngine;

namespace U3.GodotBridge;

public sealed class PlayerControllerDemo : MonoBehaviour
{
    private float _verticalVelocity;
    private float _yaw;
    private float _pitch;

    public Transform? CameraTransform { get; set; }

    public float WalkSpeed { get; set; } = 4f;

    public float SprintSpeed { get; set; } = 7f;

    public float JumpSpeed { get; set; } = 5f;

    public float Gravity { get; set; } = 14f;

    public float MouseSensitivity { get; set; } = 0.08f;

    public float RaycastDistance { get; set; } = 20f;

    public bool IsGrounded { get; private set; }

    public override void Start()
    {
        Debug.Log("PlayerControllerDemo.Start");
    }

    public override void Update()
    {
        UpdateLook();
        UpdateGrounded();
        UpdateMovement();

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            CastInteractionRay();
        }
    }

    private void UpdateLook()
    {
        _yaw += Input.GetAxisRaw("Mouse X") * MouseSensitivity;
        _pitch = Mathf.Clamp(_pitch - (Input.GetAxisRaw("Mouse Y") * MouseSensitivity), -80f, 80f);
        transform.eulerAngles = new Vector3(0f, _yaw, 0f);

        if (CameraTransform is not null)
        {
            CameraTransform.position = transform.position + new Vector3(0f, 1.4f, 0f);
            CameraTransform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
        }
    }

    private void UpdateGrounded()
    {
        IsGrounded = Physics.CheckSphere(transform.position + new Vector3(0f, -0.82f, 0f), 0.28f);
        if (IsGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = -1f;
        }
    }

    private void UpdateMovement()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var planarInput = new Vector3(horizontal, 0f, vertical);
        var move = (transform.right * planarInput.x) + (transform.forward * planarInput.z);
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        var speed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : WalkSpeed;
        var motion = move * speed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            _verticalVelocity = JumpSpeed;
            Debug.Log("PlayerControllerDemo.Jump");
        }

        _verticalVelocity -= Gravity * Time.deltaTime;
        motion.y = _verticalVelocity * Time.deltaTime;
        transform.position += motion;
    }

    private void CastInteractionRay()
    {
        var rayTransform = CameraTransform ?? transform;
        var ray = new Ray(rayTransform.position, rayTransform.forward);
        if (Physics.Raycast(ray, out var hit, RaycastDistance))
        {
            Debug.Log($"PlayerControllerDemo.Raycast hit {hit.ToDebugString()}");
            var interactable = hit.collider?.GetComponent<InteractableDemo>();
            interactable?.Interact();
        }
        else
        {
            Debug.Log("PlayerControllerDemo.Raycast missed");
        }
    }
}
