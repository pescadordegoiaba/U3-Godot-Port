# U3 Unity NetPak Extension Slice

## Files Added

`src/U3.Runtime/U3.Runtime.csproj` now links:

- `external/U3-SDK/Assets/Runtime/SDG.NetPak/UnityNetPakReaderEx.cs`
- `external/U3-SDK/Assets/Runtime/SDG.NetPak/UnityNetPakWriterEx.cs`

The files are linked directly from `external/U3-SDK`; they were not copied or edited.

## APIs Tested

Smoke tests cover:

- `WriteQuaternion` / `ReadQuaternion`
- `WriteNormalVector3` / `ReadNormalVector3`
- `WriteClampedVector3` / `ReadClampedVector3`
- `WriteColor32RGBA` / `ReadColor32RGBA`
- `WriteColor32RGB` / `ReadColor32RGB(out Color)`
- `WriteNormalVector3AsYaw` / `ReadNormalVector3AsYaw`

Quaternion and normal vector comparisons use tolerances because NetPak quantizes values.

## Unity Stubs Added

The shim was expanded with:

- `Color32`
- `Color32 -> Color` conversion
- `Color -> Color32` conversion
- `Vector2.sqrMagnitude`
- `Vector2.magnitude`
- `Vector2.normalized`
- `Vector2.Normalize()`
- `Vector2.Distance(...)`
- `Vector2.Dot(...)`
- `Vector2[int]`
- `Vector3[int]`
- `Quaternion[int]`
- `Mathf.Atan2(...)`
- `Mathf.Approximately(...)`

## Errors Resolved

Expected compile gaps were caused by Unity APIs used by the NetPak extension files:

- `Color32` was required for color read/write helpers.
- `Vector2.normalized` was required by special yaw quaternion serialization.
- Indexers were required for `Vector3` and `Quaternion` component loops.
- `Mathf.Atan2` and `Mathf.Approximately` were required by yaw helpers.

No external U3-SDK files were modified.

## Next Recommended Files

Before enabling exception-specific NetPak paths or broader Unity helpers, evaluate:

- `external/U3-SDK/Assets/Runtime/UnityEx/QuaternionEx.cs`
- `external/U3-SDK/Assets/Runtime/UnityEx/Vector3Ex.cs`

Those add `IsNormalized` helpers referenced by `UnityNetPakReaderEx/WriterEx` inside `WITH_NETPAK_EXCEPTIONS` blocks.
