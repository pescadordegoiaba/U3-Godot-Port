# U3 Runtime Map

This document maps the first pass over `external/U3-SDK/Assets/Runtime` to choose a safe first compile slice for `src/U3.Runtime`.

## Directory Overview

`Assets/Runtime` contains approximately 1,774 C# files.

| Directory | Approx. C# files | Notes |
| --- | ---: | --- |
| `Assembly-CSharp` | 1,657 | Main game/runtime body. Heavy Unity object, scene, Steam, networking, UI, physics, and asset usage. Defer most of it. |
| `SDG.Glazier` | 35 | UI abstraction layer. Depends on UI concepts and should be deferred. |
| `UnturnedDat` | 26 | DAT parser/data model. Mostly non-visual and a good early target, excluding Unity-specific extensions at first. |
| `UnityEx` | 18 | Unity extension helpers for vectors, transforms, colliders, events, paths. Useful later, but depends directly on Unity types. |
| `SystemEx` | 17 | Mostly pure .NET extension/helpers. Best first target. |
| `SDG.NetPak` | 11 | Bit-packing reader/writer core plus Unity and Steam extension files. Core files are a good early target. |
| `SDG.NetTransport` | 5 | Transport interfaces and connection abstractions. Network-facing, defer until after data/utilities compile. |
| `SDG.HostBans` | 3 | Host ban filters/managers. Includes Steam/web/runtime manager concerns. Defer. |
| `LiveConfig` | 2 | Live config data and web manager. Manager depends on `MonoBehaviour` and `UnityWebRequest`; defer. |

`Assembly-CSharp` is especially large. Its largest subdirectory is `Assembly-CSharp/Unturned` with about 1,117 C# files and high usage of Unity, Steam, networking, and UI terms.

## Approximate Term Counts

Counts were gathered with `rg -o '<term>' external/U3-SDK/Assets/Runtime -g '*.cs' | wc -l`.

| Term | Approx. uses |
| --- | ---: |
| `MonoBehaviour` | 194 |
| `GameObject` | 1,423 |
| `Transform` | 3,183 |
| `Rigidbody` | 253 |
| `Collider` | 788 |
| `Physics` | 554 |
| `AssetBundle` | 126 |
| `Resources` | 362 |
| `SceneManager` | 15 |
| `Animator` | 146 |
| `UnityEngine.UI` | 81 |
| `TMPro` | 32 |
| `UnityWebRequest` | 29 |
| `ScriptableObject` | 2 |
| `SerializeField` | 49 |
| `Network` | 529 |
| `Steam` | 6,033 |

## Difficulty Classification

### Easy

- `SystemEx`: mostly pure .NET helpers and extension methods.
- Core `UnturnedDat`: parser, tokenizer, node/value/list/dictionary model, writer, editable nodes, and metadata-preserving writer.
- Core `SDG.NetPak`: `NetPakReader`, `NetPakWriter`, constants, attributes, and length metadata.

### Medium

- `SDG.NetPak/SystemNetPakReaderEx.cs` and `SystemNetPakWriterEx.cs`: use `UnityEngine.Mathf`, but only for math helpers already mostly present in the shim.
- `SDG.NetPak/UnityNetPakReaderEx.cs` and `UnityNetPakWriterEx.cs`: depend on `Vector2`, `Vector3`, `Quaternion`, `Color32`, `Color`, and additional math/quaternion behavior.
- `UnityEx` math-only files such as vector/color/quaternion helpers: useful, but depend on richer Unity math APIs.
- `SDG.NetTransport`: relatively small but networking-oriented, so it should follow the data/utilities base.

### Difficult

- `Assembly-CSharp/Unturned`: large, highly coupled to Unity objects, scenes, assets, Steam, network, UI, and physics.
- `Assembly-CSharp/Framework`: many Unity dependencies and cross-runtime framework assumptions.
- Provider/runtime systems in `Assembly-CSharp/Provider`, `SteamworksProvider`, and networking folders.
- Physics/collider-heavy code due to `Rigidbody`, `Collider`, `Physics`, raycasts, and scene objects.

### Ignore Temporarily

- `Assembly-CSharp/Glazier*`, `SDG.Glazier`, and UI-heavy `ItemStore` code.
- `SteamworksProvider`, Steam transports, and Steam-specific NetPak extension files.
- `LiveConfigManager` and `HostBansManager`, because they combine runtime objects with web requests.
- Asset loading, scene, `Resources`, `AssetBundle`, and animation-heavy code.

## First Slice Recommendation

Start with a small utility/data slice:

- All `SystemEx/*.cs`.
- Core `UnturnedDat` parser/model files, excluding Unity-specific extensions at first.
- Core `SDG.NetPak` reader/writer files, excluding Unity and Steam extension files.

This slice avoids UI, Editor, AssetBundle-heavy systems, Steam/networking transports, scenes, physics, and most visual Unity dependencies. It prioritizes helpers, parsers, data classes, and bitstream utility code that can compile against .NET plus the existing `UnityEngine.Shim`.

## Commands Used

```bash
find external/U3-SDK/Assets/Runtime -maxdepth 2 -type d | sort
find external/U3-SDK/Assets/Runtime -type f -name '*.cs' | wc -l
find external/U3-SDK/Assets/Runtime -mindepth 1 -maxdepth 1 -type d -print0 | while IFS= read -r -d '' d; do printf '%4s  %s\n' "$(find "$d" -type f -name '*.cs' | wc -l)" "$d"; done | sort -nr
rg -o '<term>' external/U3-SDK/Assets/Runtime -g '*.cs' | wc -l
rg -l 'using UnityEngine|UnityEngine\.|Steam|Network|NetTransport|UnityWebRequest|UnityEngine\.UI|TMPro|AssetBundle|Resources|SceneManager|Animator|Rigidbody|Collider|Physics|MonoBehaviour|GameObject|Transform' external/U3-SDK/Assets/Runtime -g '*.cs'
rg --files-without-match 'using UnityEngine|UnityEngine\.|Steam|Network|NetTransport|UnityWebRequest|UnityEngine\.UI|TMPro|AssetBundle|Resources|SceneManager|Animator|Rigidbody|Collider|Physics|MonoBehaviour|GameObject|Transform' external/U3-SDK/Assets/Runtime/SystemEx external/U3-SDK/Assets/Runtime/UnturnedDat external/U3-SDK/Assets/Runtime/SDG.NetPak -g '*.cs'
```
