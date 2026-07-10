# U3 Physics Ex Slice

## Files Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/RaycastHitEx.cs`
- `external/U3-SDK/Assets/Runtime/UnityEx/BoxColliderEx.cs`
- `external/U3-SDK/Assets/Runtime/UnityEx/SphereColliderEx.cs`
- `external/U3-SDK/Assets/Runtime/UnityEx/CapsuleColliderEx.cs`

## Files Linked

- `RaycastHitEx.cs`
- `BoxColliderEx.cs`
- `SphereColliderEx.cs`
- `CapsuleColliderEx.cs`

## Files Refused

None. All evaluated files depend only on Unity-like math, collider stubs, `Physics.Overlap*NonAlloc`, `Transform.TransformPoint`, and already-linked UnityEx helpers.

## Dependencies Found

- `RaycastHit`, `Collider`, `Rigidbody`, `Transform`
- `BoxCollider`, `SphereCollider`, `CapsuleCollider`
- `Physics.OverlapBoxNonAlloc`, `OverlapSphereNonAlloc`, `OverlapCapsuleNonAlloc`
- `QueryTriggerInteraction`, `Mathf`, `MathfEx`, `Vector3`
- `TransformEx.GetSceneHierarchyPath` through debug formatting/dev branch

## APIs/Stubs Needed

- Physics backend hooks for raycast and overlap queries.
- Simple collider bounds and closest-point behavior.
- Collider registration in the fake runtime.

## Risks and Limitations

Overlap helpers compile and run against shim/backend APIs, but they are not Unity-accurate. Godot overlap support remains passive in this sprint; the primary real bridge path is raycast.

## Next Steps

Evaluate small physics formatting/traversal helpers next. Defer helpers that require real rigidbody simulation, collision events, triggers, or Unity layer metadata.
