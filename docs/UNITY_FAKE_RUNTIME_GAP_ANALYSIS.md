# Unity Fake Runtime Gap Analysis

## Source Scan

Analyzed `external/U3-SDK/Assets/Runtime` with `rg` before editing shim code.

Most frequent Unity-related terms found:

| Term | Approx. count |
| --- | ---: |
| `Text` | 1398 |
| `Transform` | 1226 |
| `GameObject` | 823 |
| `Physics` | 207 |
| `MonoBehaviour` | 192 |
| `Collider` | 167 |
| `Resources` | 153 |
| `AudioSource` | 115 |
| `Rigidbody` | 106 |
| `UnityEvent` | 74 |
| `Image` | 72 |
| `RectTransform` | 62 |
| `SerializeField` | 49 |
| `Animation` | 36 |
| `Application` | 35 |
| `Button` | 29 |
| `TextMeshProUGUI` | 26 |
| `TMP_InputField` | 23 |
| `SceneManager` | 15 |
| `PlayerPrefs` | 9 |
| `AssetBundle` | 8 |
| `Animator` | 5 |
| `ScriptableObject` | 2 |

Frequent namespaces:

| Namespace | Approx. count |
| --- | ---: |
| `using UnityEngine;` | 878 |
| `using UnityEngine.UI;` | 22 |
| `using UnityEngine.Events;` | 20 |
| `using TMPro;` | 20 |
| `using UnityEngine.SceneManagement;` | 1 |
| `using UnityEngine.Serialization;` | 1 |

## Already Present

Core/math/foundation currently include `Object`, `GameObject`, `Component`, `Behaviour`, `MonoBehaviour`, `Transform`, `Coroutine`, `Vector2`, `Vector3`, `Quaternion`, `Color`, `Color32`, `Mathf`, `Random`, `Bounds`, `Ray`, `RaycastHit`, `LayerMask`, `Plane`, `Matrix4x4`, minimal `Collider`/`Rigidbody`, `Mesh`, `MeshFilter`, `Renderer`, `MeshRenderer`, `Material`, `Camera`, `Light`, `RuntimeLoop`, `Time`, and injectable `Debug`.

## Most Important Gaps

### Core

- Component query APIs are incomplete.
- `Destroy(component)` currently needs separate component removal semantics.
- `GameObject.tag`, `GameObject.layer`, tag search, parent/child component traversal, and transform search are needed for many SDK files.

### Math

- Existing math coverage is enough for this sprint. Future additions can be driven by compile errors.

### Physics Stubs

- SDK uses `Physics`, `Collider`, `Rigidbody`, `BoxCollider`, `SphereCollider`, `CapsuleCollider`, raycasts and overlaps frequently.
- This sprint should add passive stubs only, returning false/empty results.

### Assets Stubs

- `Resources`, `TextAsset`, `ScriptableObject`, `Application`, `PlayerPrefs`, and `AssetBundle` are frequent enough to stub now.
- Real asset loading remains out of scope.

### Scene Stubs

- `SceneManager` is present but not dominant. Fake in-memory scene APIs are safe and useful.

### Animation/Audio Stubs

- `AudioSource` is frequent. `Animation` and `Animator` are lower but safe to stub.

### UI Stubs

- `UnityEngine.UI`, `Button`, `Image`, `Text`, and `RectTransform` are common. Add compile-oriented UI controls and events only.

### TMPro Stubs

- `TextMeshProUGUI`, `TMP_InputField`, and `TMP_FontAsset` are used by UI code. Add basic text/value properties.

### Attributes

- `SerializeField` is common. Other Unity attributes should exist as empty compile-time stubs.

## Implemented In This Sprint

- Core component traversal and destruction semantics.
- Common Unity attributes.
- Fake Resources/Application/PlayerPrefs/AssetBundle data APIs.
- Passive Physics/collider/rigidbody stubs.
- Fake SceneManager/AsyncOperation.
- Minimal animation/audio/render expansions.
- Minimal UnityEvent, UnityEngine.UI, and TMPro shims.

## Left For Later

- Real physics integration.
- Real AssetBundle/Resources file loading.
- Real UI rendering, event system, UI Toolkit, Glazier, or TMPro layout behavior.
- Unity networking, Steam, `UnityWebRequest`, `UnityEngine.Profiling`, `UnityEngine.Rendering`, and `UnityEngine.PlayerLoop`.
- Gameplay systems and broad U3-SDK linking.
