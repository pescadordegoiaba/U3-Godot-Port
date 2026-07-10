# U3 Matrix4x4Ex Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/Matrix4x4Ex.cs`

## Inclusion Decision

The real `SDG.Unturned.Matrix4x4Ex` was linked into `U3.Runtime`.

## Dependencies Found

- Namespace used: `UnityEngine`
- Unity types required:
  - `Matrix4x4`
  - `Vector3`
  - `Quaternion`

No dependencies were found on Physics, real Collider/Rigidbody behavior, AssetBundle, Resources, SceneManager, Animator, UI, Steam or NetTransport.

## Stubs Added

No new UnityEngine.Shim stubs were needed. The current shim already exposes the required matrix fields and `Quaternion.LookRotation`.

## APIs Tested

- `Matrix4x4Ex.GetPosition`
- `Matrix4x4Ex.GetRotation`

Existing shim tests continue to cover:

- `Matrix4x4.Translate`
- `Matrix4x4.Scale`
- `Matrix4x4.TRS`

## Limitations

`Matrix4x4Ex.GetRotation` depends on the shim's current minimal `Quaternion.LookRotation` implementation. It is sufficient for simple orthonormal transform matrices, but it is not yet a full Unity-compatible matrix decomposition system.

## Next Step

Evaluate another small UnityEx foundation helper that depends only on current math/foundation shim types. Avoid pulling any helper that requires real Physics, scene queries, assets or networking.
