using SDG.Unturned;
using UnityEngine;

namespace U3.GodotBridge;

public sealed class PlayerControllerDemo : MonoBehaviour
{
    private float _jumpLogCooldown;

    public Transform? CameraTransform { get; set; }

    public float WalkSpeed { get; set; } = 4f;

    public float SprintSpeed { get; set; } = 7f;

    public float RaycastDistance { get; set; } = 20f;

    public override void Start()
    {
        Debug.Log("PlayerControllerDemo.Start");
    }

    public override void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var move = new Vector3(horizontal, 0f, -vertical);
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        var speed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : WalkSpeed;
        transform.position += move * speed * Time.deltaTime;

        if (CameraTransform is not null)
        {
            CameraTransform.position = transform.position + new Vector3(0f, 1.4f, 5f);
            CameraTransform.LookAt(transform.position + new Vector3(0f, 1f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.Space) && _jumpLogCooldown <= 0f)
        {
            Debug.Log("PlayerControllerDemo.Jump requested");
            _jumpLogCooldown = 0.25f;
        }

        _jumpLogCooldown = Mathf.Max(0f, _jumpLogCooldown - Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            CastInteractionRay();
        }
    }

    private void CastInteractionRay()
    {
        var rayTransform = CameraTransform ?? transform;
        var ray = new Ray(rayTransform.position, rayTransform.forward);
        if (Physics.Raycast(ray, out var hit, RaycastDistance))
        {
            Debug.Log($"PlayerControllerDemo.Raycast hit {hit.ToDebugString()}");
        }
        else
        {
            Debug.Log("PlayerControllerDemo.Raycast missed");
        }
    }
}
