# Godot Overlap Queries

## What Works

`UnityEngine.Physics` now exposes `OverlapSphere`, `OverlapBox`, `OverlapCapsule`, `CheckSphere`, `CheckBox`, `CheckCapsule` and their non-alloc variants. In Godot, `GodotPhysicsBridge` implements these queries with `World3D.DirectSpaceState.IntersectShape`.

## Mapping

- Sphere queries use `SphereShape3D`.
- Box queries use `BoxShape3D`.
- Capsule queries use `CapsuleShape3D` with approximate orientation.
- Results are mapped back to fake `UnityEngine.Collider` through `GodotSceneBridge`.

## Limitations

Layer masks are numeric and only partially match Unity semantics. Capsule orientation is approximate. Outside Godot, fallback queries use collider bounds and closest-point checks rather than real physics.

## Manual Test

Run the Godot demo and interact with objects. The player grounded check and interaction raycast now use the same physics bridge path as the overlap/raycast foundation.
