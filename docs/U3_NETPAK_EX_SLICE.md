# U3 NetPak Extension Slice

Date: 2026-07-09

## Files Added

`src/U3.Runtime/U3.Runtime.csproj` now links these files from the U3-SDK:

```text
external/U3-SDK/Assets/Runtime/SDG.NetPak/SystemNetPakReaderEx.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/SystemNetPakWriterEx.cs
```

Note: the requested `Assembly-CSharp/SDG/NetPak/...` paths do not exist in this checkout. The files are present under `Assets/Runtime/SDG.NetPak`.

## APIs Tested

Smoke tests were added in `tests/U3.Port.Tests/U3RuntimeSmokeTests.cs`:

- `SystemNetPakExtensionsRoundTripPrimitiveValues`
  - `WriteInt32` / `ReadInt32`
  - `WriteUInt64` / `ReadUInt64`
  - `WriteFloat` / `ReadFloat`
  - `WriteString` / `ReadString`
  - `WriteGuid` / `ReadGuid`
  - `WriteDateTime` / `ReadDateTime`
- `SystemNetPakExtensionsRoundTripQuantizedValues`
  - `WriteSignedInt` / `ReadSignedInt`
  - `WriteUnsignedClampedFloat` / `ReadUnsignedClampedFloat`
  - `WriteClampedFloat` / `ReadClampedFloat`
  - `WriteDegrees` / `ReadDegrees`

`dotnet test U3GodotPort.sln` result:

```text
Passed: 27, Failed: 0, Skipped: 0
```

## Stubs Added

No new `UnityEngine.Shim` stubs were required.

The two files only needed existing `Mathf` members already present in the shim:

- `PI`
- `Abs`
- `FloorToInt`

## Next Recommended Files

The next useful NetPak slice is:

```text
external/U3-SDK/Assets/Runtime/SDG.NetPak/UnityNetPakReaderEx.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/UnityNetPakWriterEx.cs
```

Expected shim work before or during that slice:

- `UnityEngine.Color32`
- `Mathf.Atan2`
- `Mathf.Approximately`
- `Quaternion.Euler`
- quaternion-vector multiplication
- `Vector2.normalized`

Do not add Steamworks NetPak extensions yet because they require Steamworks types.
