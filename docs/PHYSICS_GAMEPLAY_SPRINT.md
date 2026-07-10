# Physics Gameplay Sprint

## Summary

This sprint adds the first playable fake-runtime base. Unity-like physics and input now have injectable backends, Godot can provide raycasts, and the Godot host builds a small player demo without using real Unturned gameplay, assets, UI, Steam or networking.

## U3-SDK Files Linked

- `UnityEx/RaycastHitEx.cs`
- `UnityEx/BoxColliderEx.cs`
- `UnityEx/SphereColliderEx.cs`
- `UnityEx/CapsuleColliderEx.cs`

No evaluated files were refused in this sprint.

## UnityEngine APIs Added

- `IPhysicsBackend`
- `Physics.SetBackend`, `Physics.ResetBackend`
- `Physics.Raycast` overloads with backend/fallback routing
- `Physics.RaycastAll`, `OverlapSphere`, `OverlapSphereNonAlloc`, `OverlapBoxNonAlloc`, `OverlapCapsuleNonAlloc`, `CheckSphere`
- collider registry through `Physics.AllColliders`
- simple `BoxCollider`, `SphereCollider`, `CapsuleCollider` bounds and closest-point behavior
- `IInputBackend`, `Input`, `KeyCode`

## Godot Bridge

`GodotSceneBridge` creates collision nodes for fake Unity colliders. `GodotPhysicsBridge` implements raycasts using `World3D.DirectSpaceState.IntersectRay`. `GodotInputBridge` maps keyboard/mouse state into `UnityEngine.Input`.

## Player Demo

`GodotHost` now creates a small scene with a floor, targets, player, camera and light. `PlayerControllerDemo` moves with `W/A/S/D`, sprints with `LeftShift`, logs jump requests with `Space`, and raycasts with `E` or left mouse button.

## How to Test

Run:

```bash
dotnet build U3GodotPort.sln
dotnet test U3GodotPort.sln
dotnet build godot/U3GodotPort.Godot.csproj
```

Manual Godot test: open `godot/project.godot` in Godot 4 .NET/C# and run `scenes/Main.tscn`.

## Limitations

- No real rigidbody simulation.
- Godot overlap queries are not implemented yet.
- Capsule orientation is approximate.
- Input has no mouse-look delta yet.
- The demo is not Unturned gameplay.

## Recommended Next Steps

1. Implement Godot `OverlapSphere` with `IntersectShape`.
2. Add a simple grounded check and real jump impulse for the demo player.
3. Add mouse-look support to `GodotInputBridge`.
4. Evaluate the next small UnityEx helper that depends only on passive physics data.
5. Add collision debug visualization toggles in the Godot demo.
