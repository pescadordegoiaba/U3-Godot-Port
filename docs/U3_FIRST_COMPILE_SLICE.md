# U3 First Compile Slice

This is the recommended first set of external U3-SDK files to include in `src/U3.Runtime` in the next implementation step. Do not modify these files in `external/U3-SDK`; reference them from the project file.

## Exact File List

### System helpers

Include all files from `external/U3-SDK/Assets/Runtime/SystemEx`:

```text
external/U3-SDK/Assets/Runtime/SystemEx/ArrayEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/ByteDisplay.cs
external/U3-SDK/Assets/Runtime/SystemEx/ConvertEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/DateTimeEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/DictionaryEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/DirectoryInfoEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/EnumerableEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/FileInfoEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/GuidEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/HashSetEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/IPv4Address.cs
external/U3-SDK/Assets/Runtime/SystemEx/IPv4Filter.cs
external/U3-SDK/Assets/Runtime/SystemEx/IPv4SubnetMask.cs
external/U3-SDK/Assets/Runtime/SystemEx/ListEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/PathEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/StringEx.cs
external/U3-SDK/Assets/Runtime/SystemEx/TypeEx.cs
```

Reason: these are mostly pure .NET helpers and should establish low-risk shared utility code.

### DAT parser and model

Include these `external/U3-SDK/Assets/Runtime/UnturnedDat` files:

```text
external/U3-SDK/Assets/Runtime/UnturnedDat/DatComment.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatDictionary.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatDictionaryEx.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatDictionaryWithMetadata.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatList.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatListEx.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatListValueEnumerator.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatListWithMetadata.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatNode.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatNodeEx.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatNodeWithMetadata.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatParseable.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatParser.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatTokenizer.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatValue.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatValueEx.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatValueWithMetadata.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/DatWriter.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/EditableDatDictionary.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/EditableDatList.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/EditableDatNode.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/EditableDatValue.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/IDatSerializable.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/MetadataPreservingDatWriter.cs
```

Reason: this group provides data parsing/writing and editable metadata-preserving model behavior. It has small Unity surface area: `MetadataPreservingDatWriter.cs` uses `Mathf.Max`, and `EditableDatList.cs` imports `UnityEngine` but does not appear to rely on visual Unity APIs.

Exclude for the first slice:

```text
external/U3-SDK/Assets/Runtime/UnturnedDat/UnityDatColorEx.cs
external/U3-SDK/Assets/Runtime/UnturnedDat/UnityDatEx.cs
```

Reason: these are Unity type parsing extensions for `Color`, `Color32`, `Vector2`, and `Vector3`; they are good follow-up candidates after the first slice compiles.

### NetPak core

Include these `external/U3-SDK/Assets/Runtime/SDG.NetPak` files:

```text
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetEnumAttribute.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetMaxValue.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetPakConst.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetPakReader.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetPakWriter.cs
```

Reason: these are the core bitstream primitives and metadata. They avoid Steam and Unity-specific serialization extensions.

Exclude for the first slice:

```text
external/U3-SDK/Assets/Runtime/SDG.NetPak/SteamworksNetPakReaderEx.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/SteamworksNetPakWriterEx.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/SystemNetPakReaderEx.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/SystemNetPakWriterEx.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/UnityNetPakReaderEx.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/UnityNetPakWriterEx.cs
```

Reason: `Steamworks*` requires Steamworks types; `UnityNetPak*` requires richer Unity math/color APIs; `SystemNetPak*` is low risk but uses `Mathf` and can be the first follow-up after the core compiles.

## Expected Risks

- `NetPakReader.cs`, `NetPakWriter.cs`, and later `SystemNetPak*` use `unsafe`; `U3.Runtime.csproj` will probably need `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>`.
- External files may produce nullable warnings because they were not authored for this project configuration.
- `MetadataPreservingDatWriter.cs` uses `Mathf.Max`; the current shim already has `Mathf.Max`.
- Some files have namespace splits (`Unturned.SystemEx`, `SDG.Unturned`, `SDG.NetPak`) that should be preserved exactly by linking original source files.

## Stubs Probably Needed Soon

Likely follow-up stubs for the second slice:

- `UnityEngine.Color32`
- `Mathf.Atan2`
- `Mathf.Approximately`
- `Quaternion.Euler`
- quaternion-vector multiplication
- `Vector2.normalized`

Stubs to avoid until later:

- Unity networking and `UnityWebRequest`
- Steamworks types
- `Resources` and `AssetBundle`
- scenes and `SceneManager`
- UI/TMPro
- colliders, rigidbodies, raycasts, and physics

## Next Step

Update `src/U3.Runtime/U3.Runtime.csproj` to link only the files listed in this document, then run:

```bash
dotnet build U3GodotPort.sln
```

Use the resulting compiler errors to decide whether the next change is project configuration, a tiny shim addition, or a smaller first slice.
