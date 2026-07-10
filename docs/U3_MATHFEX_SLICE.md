# U3 MathfEx Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/MathfEx.cs`

The file was linked directly from `external/U3-SDK`; it was not copied or edited.

## Decision

The real `SDG.Unturned.MathfEx` was linked into `U3.Runtime`.

The previous local minimal `src/U3.Runtime/MathfEx.cs` was removed to avoid a duplicate `SDG.Unturned.MathfEx` type.

## Dependencies Found

Namespaces:

- `System.Runtime.CompilerServices`
- `UnityEngine`

Unity types/APIs required:

- `Mathf`
- `Vector2`
- `Vector3`
- `Quaternion`
- `Color`
- `Random.insideUnitCircle`

No Physics, Rigidbody, Collider, AssetBundle, Resources, SceneManager, Animator, UI, Steam or NetTransport dependencies were found.

## Stubs Added

`UnityEngine.Shim` was expanded with:

- `Vector3.Lerp`
- `Mathf.InverseLerp`
- `Random.insideUnitCircle`

The `Random` shim is intentionally minimal and only supports the API required by this slice.

## Tests Created

Tests cover:

- `Vector3.Lerp`
- `Mathf.InverseLerp`
- `Random.insideUnitCircle`
- `MathfEx` constants and basic math helpers
- `MathfEx` nearly-equal overloads
- clamp, min, max and integer clamp helpers
- round/truncate helpers
- geometry helpers
- ray distance/projection helpers
- random position helpers
- smooth step helpers

Existing `QuaternionEx` and `Vector3Ex` tests continue to validate compatibility with the real `MathfEx`.

## Next Steps

The next safe slice is likely another UnityEx math/data helper that depends only on existing math structs, such as `ColorEx.cs` or `Vector2Ex.cs`, after inspecting dependencies.
