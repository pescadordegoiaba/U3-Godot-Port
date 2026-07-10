# Godot Physics Bridge

## Architecture

`UnityEngine.Physics` now supports an injectable `IPhysicsBackend`. Outside Godot it uses a small fake fallback. In Godot, `GodotPhysicsBridge` is installed by `GodotHost` and uses `World3D.DirectSpaceState.IntersectRay`.

`GodotSceneBridge` maps fake Unity colliders to Godot collision nodes:

- `BoxCollider` -> `BoxShape3D`
- `SphereCollider` -> `SphereShape3D`
- `CapsuleCollider` -> `CapsuleShape3D`
- normal colliders -> `StaticBody3D`
- trigger colliders -> `Area3D`
- colliders with `Rigidbody` -> `AnimatableBody3D`

## Raycast

`Physics.Raycast` calls the installed Godot backend, converts Unity vectors to Godot vectors, calls `IntersectRay`, and maps the returned `CollisionObject3D` back to the fake Unity `Collider` when possible.

## Limitations

- No real Unity rigidbody simulation.
- `OverlapSphere` and non-alloc overlap queries are passive in the Godot backend for now.
- Capsule direction is not fully mapped to Godot shape orientation.
- Layer names are still fake; numeric layer masks are mapped directly.

## Manual Test

Open `godot/project.godot` in Godot 4 .NET and run `scenes/Main.tscn`. Use `E` or left mouse button to raycast from the camera. Hits are logged through `UnityEngine.Debug`.

## Next Step

Add Godot `IntersectShape` support for `OverlapSphere` and then box/capsule overlaps.
