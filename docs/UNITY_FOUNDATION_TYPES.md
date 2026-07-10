# Unity Foundation Types

## Types Added

`UnityEngine.Shim` now includes foundation stubs for future physics, raycast and UnityEx slices:

- `Bounds`
- `Ray`
- `RaycastHit`
- `Collider`
- `Rigidbody`
- `LayerMask`
- `Plane`
- `Matrix4x4`

## APIs Implemented

`Random` now supports seeded deterministic generation with:

- `value`
- `Range(int, int)`
- `Range(float, float)`
- `insideUnitCircle`
- `insideUnitSphere`
- `onUnitSphere`
- `rotation`
- `InitState(int)`

Math support was expanded with:

- `Vector3.Min`
- `Vector3.Max`
- `Vector3.Scale`
- `Vector3.ClampMagnitude`
- `Mathf.Epsilon`
- `Mathf.Pow`
- `Mathf.Acos`
- `Mathf.Asin`
- `Mathf.Sign`

`Matrix4x4` supports identity/zero matrices, `[row, column]` indexing, point/vector multiplication, translation, scale, rotation and basic TRS composition.

## Limitations

This is not a physics implementation. `Collider` and `Rigidbody` are empty component stubs used only for type compatibility.

`LayerMask` name lookup is intentionally stubbed:

- `GetMask(...)` returns `0`.
- `NameToLayer(...)` returns `-1`.
- `LayerToName(...)` returns `string.Empty`.

`RaycastHit` stores hit data, but no raycast query system exists yet.

`Matrix4x4` is a practical math shim for common TRS usage. It is not yet a complete Unity-compatible matrix implementation.

## Next UnityEx Candidates

These files can be evaluated next, after dependency inspection:

- `UnityEx/BoundsEx.cs`
- `UnityEx/Matrix4x4Ex.cs`
- `UnityEx/RaycastHitEx.cs`

Physics-related helpers should remain blocked until real `Physics`, `Collider` and `Rigidbody` behavior is intentionally scoped.
