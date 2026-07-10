# U3 Vector2Ex Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/Vector2Ex.cs`

The file was linked directly from `external/U3-SDK`; it was not copied or edited.

## Decision

The real `UnityEngine.Vector2Ex` was linked into `U3.Runtime`.

## Dependencies Found

Namespaces:

- `UnityEngine`

Unity types/APIs required:

- `Vector2`
- `Vector2(float, float)`

No Physics, Rigidbody, Collider, AssetBundle, Resources, SceneManager, Animator, UI, Steam or NetTransport dependencies were found.

## Stubs Added

No new `UnityEngine.Shim` stubs were required. The existing `Vector2` fields and constructor are enough for this slice.

## Tests Created

Tests cover:

- `new Vector2(1, 0).Cross() -> new Vector2(0, -1)`
- `new Vector2(0, 1).Cross() -> new Vector2(1, 0)`
- `new Vector2(3, 4).Cross() -> new Vector2(4, -3)`

## Limitations

`Vector2Ex.cs` only provides the `Cross` extension method, so this slice does not expand broader `Vector2` math behavior.

## Next Step

Evaluate `external/U3-SDK/Assets/Runtime/UnityEx/ColorEx.cs` as the next small UnityEx data/math helper slice after inspecting dependencies.
