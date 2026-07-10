# Godot Rigidbody Collider Sync

## What Changed

The fake `Rigidbody` now tracks more Unity-like state: `detectCollisions`, `freezeRotation`, interpolation, sleep/wake stubs and simple `AddForce` velocity changes for non-kinematic bodies.

`Collider` has `PhysicMaterial` storage, enabled-state filtering, and bounds that follow the fake `Transform`. `CharacterController` exists as a kinematic stub with `Move` and `SimpleMove`.

## Godot Sync

`GodotSceneBridge` continues to sync `Transform` into `Node3D` and keeps collision shapes associated with fake colliders. Shape data is updated from collider properties without intentionally recreating the fake Unity object.

## Limitations

There is no real rigidbody simulation. Non-kinematic `AddForce` updates velocity only; it does not integrate position automatically. Capsule shape orientation is still approximate.
