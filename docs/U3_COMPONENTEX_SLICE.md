# U3 ComponentEx Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/ComponentEx.cs`

## Inclusion Decision

The real `SDG.Unturned.ComponentExtension` was linked into `U3.Runtime`.

## Dependencies Found

Namespaces used:

- `UnityEngine`

Unity types and APIs required:

- `Component`
- `Component.transform`

Internal SDK dependencies:

- `TransformEx.GetSceneHierarchyPath`

No dependency was found on real Physics, Rigidbody simulation, Collider behavior, AssetBundle, Resources, SceneManager, Animator, UI, Steam or NetTransport.

## APIs/Stubs Added

No new UnityEngine.Shim APIs were needed for this slice.

## APIs Tested

- `ComponentExtension.GetSceneHierarchyPath(Component)`
- null component handling through the static extension method call

## Limitations

The path result depends on the shim's simplified transform hierarchy and name handling. It is sufficient for fake runtime diagnostics, but it is not a full Unity scene object identity model.

## Compatibility Risks

Unity's special destroyed-object null semantics are not fully emulated. This slice only validates ordinary live components and true null references.

## Next Step

Evaluate `UnityEx/RaycastHitEx.cs` next. It should be small now because it depends on `RaycastHit`, passive collider/rigidbody fields, and the linked `TransformEx.GetSceneHierarchyPath`.
