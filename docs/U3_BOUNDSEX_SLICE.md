# U3 BoundsEx Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/BoundsEx.cs`

## Inclusion Decision

The real `SDG.Unturned.BoundsEx` was linked into `U3.Runtime`.

## Dependencies Found

- Namespace used: `UnityEngine`
- Unity types required:
  - `Bounds`
  - `Vector3`

No dependencies were found on Physics, real Collider/Rigidbody behavior, AssetBundle, Resources, SceneManager, Animator, UI, Steam or NetTransport.

## Stubs Added

No new UnityEngine.Shim stubs were needed. The current `Bounds` implementation already exposes:

- `center`
- `extents`
- `size`

## APIs Tested

- `BoundsEx.ContainsXZ`
- `BoundsEx.CalculateVolume`

## Limitations

`BoundsEx.cs` only adds simple bounds math helpers. This slice does not add real physics queries, collision tests, layer filtering or scene object lookup behavior.

## Next Step

Evaluate another small UnityEx foundation helper, such as `Matrix4x4Ex.cs`, only after inspecting that it depends on existing math/foundation shim types and does not pull in real physics or asset systems.
