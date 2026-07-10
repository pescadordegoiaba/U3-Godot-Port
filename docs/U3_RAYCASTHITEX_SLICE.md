# U3 RaycastHitEx Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/RaycastHitEx.cs`

## Inclusion Decision

The real `SDG.Unturned.RaycastHitEx` was linked into `U3.Runtime`.

## Dependencies Found

Namespaces used:

- `UnityEngine`

Unity types and APIs required:

- `RaycastHit`
- `RaycastHit.collider`
- `RaycastHit.rigidbody`
- `RaycastHit.transform`
- `RaycastHit.point`
- `RaycastHit.normal`
- `TransformEx.GetSceneHierarchyPath`

Internal SDK dependencies:

- `SDG.Unturned.TransformEx`

No dependency was found on real Physics simulation, Rigidbody simulation, Collider physics behavior, AssetBundle, Resources, SceneManager, Animator, UI, Steam, NetTransport or gameplay systems.

## APIs/Stubs Added

No new UnityEngine.Shim APIs were needed for this slice.

## Tests Created

- `RaycastHitExToDebugStringUsesHitRelationsAndHierarchyPath`
- `RaycastHitExToDebugStringHandlesDefaultHit`

## Limitations

`RaycastHitEx.ToDebugString` now works for fake hits, but the shim still does not perform real raycasts. `RaycastHit.collider`, `transform` and `rigidbody` are only meaningful when tests or future bridge code populate the fake hit data.

## Compatibility Risks

The string output depends on `ToString()` behavior for shim objects and vectors, which may not exactly match Unity. The hierarchy path is backed by the simplified fake `Transform` model.

## Next Step

Evaluate another small UnityEx helper that only formats or traverses already-supported fake Unity types. Avoid files that invoke real `Physics`, scene loading, assets, UI or networking until the corresponding subsystem has a deliberate fake implementation.
