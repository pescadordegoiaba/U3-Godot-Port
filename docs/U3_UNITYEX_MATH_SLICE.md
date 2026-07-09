# U3 UnityEx Math Slice

## Files Added

`src/U3.Runtime/U3.Runtime.csproj` links these SDK files:

- `external/U3-SDK/Assets/Runtime/UnityEx/QuaternionEx.cs`
- `external/U3-SDK/Assets/Runtime/UnityEx/Vector3Ex.cs`

The files are linked directly from `external/U3-SDK`; they were not copied or edited.

## Dependencies Found

Both files are math-only and depend on `UnityEngine` math structs plus `SDG.Unturned.MathfEx`.

`external/U3-SDK/Assets/Runtime/UnityEx/MathfEx.cs` was not linked in this slice because it includes helpers using `UnityEngine.Random.insideUnitCircle`. Pulling it whole would require expanding the shim outside the current math-helper scope.

## Stubs Added

`U3.Runtime` now includes a local minimal `SDG.Unturned.MathfEx` with only the members needed by `QuaternionEx.cs` and `Vector3Ex.cs`:

- `Square`
- `IsNearlyEqual`
- `IsAngleDegreesNearlyEqual`
- `IsNearlyZero`
- `Min(float, float, float)`
- `Max(float, float, float)`

`UnityEngine.Shim` added:

- `Mathf.Repeat`
- `Mathf.DeltaAngle`

## APIs Tested

Smoke tests cover:

- `QuaternionEx.IsNormalized`
- `QuaternionEx.GetRoundedIfNearlyAxisAligned`
- `Vector3Ex.IsNormalized`
- `Vector3Ex.ContainsNaN`
- `Vector3Ex.ContainsInfinity`
- `Vector3Ex.IsFinite`
- `Vector3Ex.IsNearlyZero`
- `Vector3Ex.IsNearlyEqual`
- `Vector3Ex.AreComponentsNearlyEqual`
- `Vector3Ex.GetRoundedIfNearlyEqualToOne`
- `Vector3Ex.GetAbs`
- `Vector3Ex.GetMin`
- `Vector3Ex.GetMax`
- `Vector3Ex.GetHorizontal`
- `Vector3Ex.GetHorizontalMagnitude`
- `Vector3Ex.GetHorizontalSqrMagnitude`
- `Vector3Ex.ClampHorizontalMagnitude`
- `Vector3Ex.ClampMagnitude`
- `Vector3Ex.TryParseVector3`

## Limitations

The local `MathfEx` is intentionally incomplete. It should be replaced by the real SDK `MathfEx.cs` only when the shim is ready for its extra dependencies, including `UnityEngine.Random`.

Quaternion rounding depends on the current shim's Euler conversion, which is approximate and already documented as not fully Unity-compatible.

## Next Recommended Files

Evaluate `external/U3-SDK/Assets/Runtime/UnityEx/MathfEx.cs` as its own slice. Expected prerequisites include a minimal `UnityEngine.Random` and possibly `Mathf.InverseLerp` and `Vector3.Lerp` if broader methods are tested.
