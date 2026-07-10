# U3 ColorEx Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/ColorEx.cs`

The file was linked directly from `external/U3-SDK`; it was not copied or edited.

## Decision

The real `SDG.Unturned.ColorEx` was linked into `U3.Runtime`.

## Dependencies Found

Namespaces:

- `UnityEngine`

Unity types/APIs required:

- `Color`
- `Color(float, float, float, float)`
- `Mathf.Abs`

No Texture, Material, Shader, AssetBundle, Resources, SceneManager, Animator, UI, Steam, NetTransport or Physics dependencies were found.

## Stubs Added

No new `UnityEngine.Shim` stubs were required. Existing `Color` and `Mathf.Abs` support this slice.

## Tests Created

Tests cover:

- `ColorEx.BlackZeroAlpha`
- `ColorEx.WhiteZeroAlpha`
- `Color.IsNearlyBlack`
- `Color.IsNearlyWhite`
- `Color.WithAlpha`

## Limitations

`ColorEx.cs` only provides simple color constants and extension helpers. This slice does not add HTML color parsing, HSV conversion, texture, material or shader behavior.

## Next Step

Evaluate another small UnityEx helper only after dependency inspection. Good candidates are files that remain pure math/data helpers and do not introduce physics, rendering assets or scene systems.
