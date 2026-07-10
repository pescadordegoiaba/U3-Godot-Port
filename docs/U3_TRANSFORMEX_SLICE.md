# U3 TransformEx Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/TransformEx.cs`

## Inclusion Decision

The real `SDG.Unturned.TransformEx` was linked into `U3.Runtime`.

`TransformEx.cs` directly calls `GameObjectEx.GetOrAddComponent<T>`, so the real `external/U3-SDK/Assets/Runtime/UnityEx/GameObjectEx.cs` was linked in the same slice as a small justified dependency.

## Dependencies Found

Namespaces used by `TransformEx.cs`:

- `System.Collections.Generic`
- `UnityEngine`

Unity types and APIs required:

- `Transform`
- `GameObject`
- `Component`
- `Rigidbody`
- `Object.Destroy`
- `Transform.parent`
- `Transform.root`
- `Transform.Find`
- `Transform.GetComponent<T>`
- `Transform` child enumeration
- `Transform.rotation`
- `Transform.localScale`
- `Quaternion.Inverse`

Internal SDK helpers required:

- `GameObjectEx.GetOrAddComponent<T>`
- `QuaternionEx.GetRoundedIfNearlyAxisAligned`
- `Vector3Ex.GetRoundedIfNearlyEqualToOne`

No dependency was found on real Physics, real Rigidbody simulation, real Collider behavior, AssetBundle, Resources, SceneManager, Animator, UI, Steam or NetTransport.

## APIs/Stubs Added

- `GameObject.AddComponent<T>()` no longer requires a `new()` generic constraint and uses `Activator.CreateInstance`, matching Unity-style generic usage more closely.

No physics, assets, scene, UI or gameplay behavior was added.

## APIs Tested

- `TransformEx.GetOrAddComponent`
- `TransformEx.FindAllChildrenWithName` for `GameObject` and `Transform` lists
- `TransformEx.GetChildOfParent`
- `TransformEx.GetRootTransform`
- `TransformEx.GetSceneHierarchyPath`
- `TransformEx.DumpChildren`
- `TransformEx.DestroyComponentIfExists`
- `TransformEx.SetRotation_RoundIfNearlyAxisAligned`
- `TransformEx.SetLocalScale_RoundIfNearlyEqualToOne`
- `TransformEx.InverseTransformRotation`
- `TransformEx.TransformRotation`

## Limitations

`TransformEx` now compiles and has smoke coverage against the fake hierarchy model, but the shim still uses simplified world/local transform math. `Destroy` is immediate rather than Unity's deferred end-of-frame behavior.

## Compatibility Risks

- Hierarchy path and dump behavior depend on the shim's simplified `Transform.name` mirroring the GameObject name at construction time.
- Rotation helpers depend on the shim's minimal quaternion math.
- `GameObjectEx.GetRectTransform` can return `null` for ordinary `GameObject` instances because the fake `GameObject` constructor creates a regular `Transform`.

## Next Step

Evaluate `UnityEx/ComponentEx.cs` next. It should be low risk now because it depends mainly on `Component` and the newly linked `TransformEx.GetSceneHierarchyPath`.
