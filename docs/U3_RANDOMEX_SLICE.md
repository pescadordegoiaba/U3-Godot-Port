# U3 RandomEx Slice

## File Evaluated

- `external/U3-SDK/Assets/Runtime/UnityEx/RandomEx.cs`

## Inclusion Decision

The real `SDG.Unturned.RandomEx` was linked into `U3.Runtime`.

## Dependencies Found

- Namespace used: `UnityEngine`
- Unity/runtime types required:
  - `UnityEngine.Random.value`
  - `Mathf`
  - `SDG.Unturned.MathfEx`
  - `Vector3`

No dependencies were found on Physics, real Collider/Rigidbody behavior, AssetBundle, Resources, SceneManager, Animator, UI/TMPro/Glazier, Steam or NetTransport.

## Stubs Added

No new UnityEngine.Shim stubs were needed. The current shim already exposes the required `Random.value`, `Mathf` helpers and `Vector3` math.

## APIs Tested

- `RandomEx.GetRandomForwardVectorInCone`

The tests cover:

- zero-angle cone returns `Vector3.forward`;
- generated vectors are finite;
- generated vectors remain approximately normalized;
- generated vectors stay within the requested cone angle.

## Limitations

The helper depends on the shim's pseudo-random generator, so exact sequences are not intended to match Unity. The tests validate shape/range invariants rather than Unity-specific random values.

## Next Step

Evaluate `TransformEx.cs` as a deferred medium-risk slice, or split out a small hierarchy/tag shim prerequisite first. Avoid the collider helpers until a deliberate physics/collider shim slice exists.
