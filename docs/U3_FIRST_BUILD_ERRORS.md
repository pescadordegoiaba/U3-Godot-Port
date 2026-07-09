# U3 First Build Errors

Date: 2026-07-09

Command:

```bash
dotnet build U3GodotPort.sln
```

Result:

```text
Build succeeded.
127 Warning(s)
0 Error(s)
```

## Included Files

`src/U3.Runtime/U3.Runtime.csproj` now links the first compile slice from `external/U3-SDK` without copying or modifying SDK files.

### SystemEx

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

### UnturnedDat Core

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

### SDG.NetPak Core

```text
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetEnumAttribute.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetMaxValue.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetPakConst.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetPakReader.cs
external/U3-SDK/Assets/Runtime/SDG.NetPak/NetPakWriter.cs
```

## Current Errors

No compiler errors remain for this first slice.

## Current Warnings

The remaining 127 warnings are from unchanged external source files. They are grouped as:

- Nullable reference warnings such as `CS8625`, `CS8600`, `CS8601`, `CS8603`, `CS8604`, `CS8618`, and `CS8602`.
- Generic nullability constraint warnings such as `CS8714` in `SystemEx/DictionaryEx.cs`.
- Override nullability mismatch warnings such as `CS8765` in IPv4 value types.

These warnings are expected because the linked U3-SDK source was not authored for this project's nullable configuration. They do not block the first compile slice.

## Stubs Needed

No new Unity shim stubs were required for this slice.

The existing `UnityEngine.Mathf.Max` was enough for `UnturnedDat/MetadataPreservingDatWriter.cs`, and no heavy Unity APIs were included.

## Files Deferred From First Slice

Keep these out of the first slice:

- `external/U3-SDK/Assets/Runtime/UnturnedDat/UnityDatEx.cs`
- `external/U3-SDK/Assets/Runtime/UnturnedDat/UnityDatColorEx.cs`
- `external/U3-SDK/Assets/Runtime/SDG.NetPak/SystemNetPakReaderEx.cs`
- `external/U3-SDK/Assets/Runtime/SDG.NetPak/SystemNetPakWriterEx.cs`
- `external/U3-SDK/Assets/Runtime/SDG.NetPak/UnityNetPakReaderEx.cs`
- `external/U3-SDK/Assets/Runtime/SDG.NetPak/UnityNetPakWriterEx.cs`
- `external/U3-SDK/Assets/Runtime/SDG.NetPak/SteamworksNetPakReaderEx.cs`
- `external/U3-SDK/Assets/Runtime/SDG.NetPak/SteamworksNetPakWriterEx.cs`
- `Assembly-CSharp`, `NetGen`, UI, scene, AssetBundle, Steam, physics, and networking-heavy folders.

## Next Step

Add focused smoke tests around:

- `SDG.Unturned.DatParser` and `DatWriter`
- `SDG.NetPak.NetPakReader` and `NetPakWriter`
- selected `Unturned.SystemEx` helpers

After those tests pass, consider adding `SystemNetPakReaderEx.cs` and `SystemNetPakWriterEx.cs` as the next small slice.
