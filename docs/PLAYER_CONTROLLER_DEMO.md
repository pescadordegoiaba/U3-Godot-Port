# Player Controller Demo

## Controls

- `W/A/S/D` or arrows: move
- `LeftShift`: sprint
- `Space`: jump request log
- `E` or left mouse button: raycast from the camera

## How to Run

Open `godot/project.godot` with Godot 4 .NET/C# and run the main scene `res://scenes/Main.tscn`.

## What Works

The demo creates a fake Unity scene from `GodotHost`: ground, target objects, a player object, camera and sunlight. Meshes and colliders are represented by Godot nodes. `PlayerControllerDemo` uses `UnityEngine.Input`, `UnityEngine.Physics`, `UnityEngine.Transform` and `UnityEngine.Debug`.

## What Is Still Fake

- Movement is kinematic, not real rigidbody simulation.
- Jump only logs intent.
- Raycast uses Godot physics when running in Godot, but overlap queries are still passive.
- No real Unturned gameplay or assets are loaded.

## Next Step

Add mouse-look and a simple grounded check using Godot raycasts.
