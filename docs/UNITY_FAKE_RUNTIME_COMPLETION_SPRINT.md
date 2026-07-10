# Unity Fake Runtime Completion Sprint

## Implemented

This sprint expanded the fake Unity runtime without changing `external/U3-SDK` and without linking new U3-SDK files.

Core runtime additions:

- Component and GameObject query APIs:
  - `GetComponent<T>`
  - `TryGetComponent<T>`
  - `GetComponents<T>`
  - `GetComponentInChildren<T>`
  - `GetComponentsInChildren<T>`
  - `GetComponentInParent<T>`
  - `GetComponentsInParent<T>`
- Interface-friendly component queries.
- `GameObject.tag`, `GameObject.layer`, `CompareTag`, `FindGameObjectWithTag`, and `FindGameObjectsWithTag`.
- Separate `Destroy(component)` and `Destroy(gameObject)` behavior.
- `RuntimeLoop` ignores destroyed components and destroyed/inactive GameObjects.
- `Transform.Find`, `DetachChildren`, sibling index helpers, child enumeration, and simple transform point/direction helpers.

Data and asset additions:

- `ScriptableObject.CreateInstance`
- `TextAsset`
- `Resources` fake registry with `Register`, `Load`, `LoadAll`, `UnloadAsset`, and `ClearRegistered`
- `Application`
- `RuntimePlatform`
- in-memory `PlayerPrefs`
- passive `AssetBundle`

Physics additions:

- `Physics` passive stubs
- `QueryTriggerInteraction`
- `ForceMode`
- `CollisionDetectionMode`
- `RigidbodyConstraints`
- expanded `Collider`
- `BoxCollider`
- `SphereCollider`
- `CapsuleCollider`
- expanded `Rigidbody`

Scene additions:

- `UnityEngine.SceneManagement.Scene`
- `LoadSceneMode`
- fake `SceneManager`
- `AsyncOperation`

Animation/audio/render additions:

- `Animator`
- `Animation`
- `AnimationClip`
- `RuntimeAnimatorController`
- `AudioClip`
- `AudioSource`
- `Texture`
- `Texture2D`
- `Sprite`
- `Shader`
- expanded `Renderer`
- `SkinnedMeshRenderer`
- `LineRenderer`
- `ParticleSystem`
- expanded `Mesh.bounds`

UI/events/TMPro additions:

- `UnityEngine.Events.UnityEvent`
- `UnityEvent<T>`
- `UnityEvent<T0,T1>`
- `UnityAction` delegates
- `UnityEngine.UI` controls: `Graphic`, `MaskableGraphic`, `Text`, `Image`, `RawImage`, `Button`, `Toggle`, `Slider`, `ScrollRect`, `Selectable`, `Canvas`, `CanvasGroup`, `LayoutElement`, `ContentSizeFitter`, layout groups, and `InputField`
- `UnityEngine.RectTransform`
- `TMPro.TMP_Text`, `TextMeshProUGUI`, `TextMeshPro`, `TMP_FontAsset`, `TMP_Dropdown`, and `TMP_InputField`

Attributes:

- common Unity attributes such as `SerializeField`, `HideInInspector`, `HeaderAttribute`, `TooltipAttribute`, `RangeAttribute`, `CreateAssetMenuAttribute`, and runtime initialization attributes
- `UnityEngine.Serialization.FormerlySerializedAsAttribute`

## Functional APIs

The following APIs have useful fake behavior:

- GameObject/component lookup and hierarchy traversal
- GameObject registry and tag search
- Transform child management and simple local/world mapping
- RuntimeLoop lifecycle filtering
- Resources in-memory registration and loading
- PlayerPrefs in-memory storage
- SceneManager in-memory active scene tracking
- Animator parameter dictionaries
- AudioSource play state
- UnityEvent listener invocation
- Button click event invocation
- TMP text storage

## Passive Stubs

These APIs intentionally do not implement real engine behavior:

- Physics raycasts and overlaps return no hits.
- Collider and Rigidbody do not simulate collisions or motion.
- AssetBundle does not load files.
- Resources does not load files from disk.
- UI controls do not render or process real input.
- TMPro does not perform layout/rendering.
- Renderer, Shader, Texture, Sprite, LineRenderer, SkinnedMeshRenderer, and ParticleSystem do not render.

## Still Missing

- Real physics integration.
- Terrain, NavMesh, character controller, and advanced collider behavior.
- Unity UI EventSystem, UI Toolkit, Glazier, and layout behavior.
- Real asset loading, AssetBundle decoding, and Resources folder scanning.
- Unity networking, Steamworks, `UnityWebRequest`, profiling, rendering pipeline, player loop, and audio mixer APIs.
- Full Unity object lifetime semantics such as delayed destroy queues and native null behavior.

## Compatibility Risks

- Destroy is immediate; Unity normally defers `Destroy` until end-of-frame.
- Transform world/local math is intentionally simplified.
- Component queries are broad and interface-friendly, but do not model Unity's exact native lookup ordering in every edge case.
- Physics APIs are no-hit stubs, so gameplay relying on collisions/raycast results will not behave correctly yet.
- UI/TMPro APIs are compile/test shims, not rendering systems.

## U3-SDK Files Closer To Compiling

This sprint moves these helper families closer:

- `UnityEx/TransformEx.cs`
- `UnityEx/ComponentEx.cs`
- `UnityEx/GameObjectEx.cs`
- `UnityEx/RaycastHitEx.cs`
- `UnityEx/UnityEventEx.cs`
- low-risk code using `Resources`, `TextAsset`, `PlayerPrefs`, `Application`, `AudioSource`, `Animator`, or passive physics signatures

Collider helpers such as `BoxColliderEx.cs`, `SphereColliderEx.cs`, and `CapsuleColliderEx.cs` are closer to compiling, but should still be treated carefully because their overlap helpers depend on physics semantics.

## Recommended Next Slice

Evaluate `UnityEx/TransformEx.cs` next as a focused compile slice. If it requires one more shim pass, keep it limited to `Quaternion.Inverse`, transform hierarchy helpers, and component lookup behavior rather than physics or rendering.
