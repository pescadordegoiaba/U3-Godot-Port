# U3 UnityEx Remaining Map

## Summary

`external/U3-SDK/Assets/Runtime/UnityEx` has 17 `*Ex.cs` helper files. Seven are already linked into `U3.Runtime`, and ten remain.

This pass only analyzes the remaining helpers. No runtime project links, stubs, or external SDK files were changed.

## Already Linked

| File | Dependencies | Risk | Current status |
| --- | --- | --- | --- |
| `BoundsEx.cs` | `UnityEngine.Bounds`, `Vector3` | Low | Linked |
| `ColorEx.cs` | `UnityEngine.Color`, `MathfEx` | Low | Linked |
| `MathfEx.cs` | `Mathf`, `Vector2`, `Vector3`, `Quaternion`, `Color`, `Random`, `Ray` | Low/Medium | Linked |
| `Matrix4x4Ex.cs` | `Matrix4x4`, `Vector3`, `Quaternion` | Low | Linked |
| `QuaternionEx.cs` | `Quaternion`, `MathfEx` | Low | Linked |
| `Vector2Ex.cs` | `Vector2` | Low | Linked |
| `Vector3Ex.cs` | `Vector3`, `MathfEx` | Low | Linked |

## Remaining `*Ex.cs` Files

| File | Dependencies | Risk | Reason | Recommendation |
| --- | --- | --- | --- | --- |
| `RandomEx.cs` | `UnityEngine.Random.value`, `Mathf`, `MathfEx`, `Vector3` | Low | Pure math helper returning a random forward vector inside a cone. No hierarchy, physics, assets, UI or networking. | Include now |
| `ComponentEx.cs` | `Component`, `TransformEx.GetSceneHierarchyPath` | Medium | Small, but depends on `TransformEx` being linked. | Defer until `TransformEx` slice |
| `TransformEx.cs` | `System.Collections.Generic`, `Transform`, `GameObject`, `Component`, `Object.Destroy`, `Rigidbody`, `QuaternionEx`, `Vector3Ex`, `GameObjectEx` | Medium | Useful hierarchy helper, but current shim likely needs `Transform.Find`, `Transform.GetComponent<T>`, possibly component destruction semantics and `Quaternion.Inverse`. | Defer; good candidate after `RandomEx` |
| `GameObjectEx.cs` | `GameObject`, `Component`, `RectTransform`, `TransformEx`, `Transform` enumeration, `CompareTag`, `tag`, `layer` | Medium | Mostly hierarchy/tag helpers, but needs `RectTransform`, `GameObject.tag`, `GameObject.layer`, and `CompareTag` stubs. | Defer until tag/layer/UI-ish shim slice |
| `RaycastHitEx.cs` | `RaycastHit`, `Collider`, `Rigidbody`, `TransformEx.GetSceneHierarchyPath` | Medium | Debug formatting only, but depends on `TransformEx` and collider hit fields. No real raycast call. | Defer until `TransformEx` is linked |
| `ControllerColliderHitEx.cs` | `ControllerColliderHit`, `Collider`, `Rigidbody`, `TransformEx.GetSceneHierarchyPath`, `Vector3` | High | Requires `ControllerColliderHit` and character-controller collision concepts not present in shim. | Block for now |
| `BoxColliderEx.cs` | `BoxCollider`, `Collider[]`, `Transform.TransformPoint`, `Physics.OverlapBoxNonAlloc`, `QueryTriggerInteraction`, `Mathf`, `Vector3` | High | Includes real overlap query helper and collider shape type. | Block until physics/collider shim slice |
| `CapsuleColliderEx.cs` | `CapsuleCollider`, `Collider[]`, `Transform.TransformPoint`, `Physics.OverlapCapsuleNonAlloc`, `QueryTriggerInteraction`, `Mathf`, `Vector3`, optional `GetSceneHierarchyPath` in dev/editor branch | High | Includes real overlap query helper and capsule collider shape behavior. | Block until physics/collider shim slice |
| `SphereColliderEx.cs` | `SphereCollider`, `Collider[]`, `Transform.TransformPoint`, `Physics.OverlapSphereNonAlloc`, `QueryTriggerInteraction`, `Mathf`, `MathfEx`, `Vector3` | High | Includes real overlap query helper and sphere collider shape behavior. | Block until physics/collider shim slice |
| `UnityEventEx.cs` | `UnityEngine.Events.UnityEvent`, `UnityEngine.Object`, `Debug.LogException` | Medium | Small wrapper, but needs `UnityEngine.Events` and `Debug.LogException(Object context)` behavior. | Defer until events/logger shim slice |

## Categories

### Math/Foundation Light

- `RandomEx.cs`

### Likely Need Physics/Collider/Rigidbody

- `BoxColliderEx.cs`
- `CapsuleColliderEx.cs`
- `ControllerColliderHitEx.cs`
- `RaycastHitEx.cs`
- `SphereColliderEx.cs`

### Likely Need Resources/AssetBundle/Assets

- None among remaining `*Ex.cs` files.

### Likely Need UI/TMPro/Glazier

- `GameObjectEx.cs`, because it references `RectTransform`.

### Likely Need SceneManager/Animator

- None among remaining `*Ex.cs` files.

### Likely Need Steam/Networking Heavy

- None among remaining `*Ex.cs` files.

### Uncertain / Needs Small Shim Decision

- `ComponentEx.cs`
- `TransformEx.cs`
- `GameObjectEx.cs`
- `RaycastHitEx.cs`
- `UnityEventEx.cs`

## Recommended Next Slice

Implement `external/U3-SDK/Assets/Runtime/UnityEx/RandomEx.cs` next.

Why it is safe:

- It depends only on math/foundation APIs already present in the shim.
- It uses the already linked real `MathfEx`.
- It does not require hierarchy traversal, component lookup, physics queries, collider shapes, UI, assets, scenes, Steam or networking.

Likely stubs needed:

- None expected.

Suggested smoke tests:

- `RandomEx.GetRandomForwardVectorInCone(0f)` returns approximately `Vector3.forward`.
- For a small cone angle, the returned vector has magnitude approximately `1`.
- For repeated samples, the angle from `Vector3.forward` is less than or equal to `halfAngleRadians` converted to degrees, with a small tolerance.
