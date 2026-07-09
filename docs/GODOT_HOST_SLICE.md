# Godot Host Slice

Date: 2026-07-09

## Files Created

```text
godot/project.godot
godot/U3GodotPort.Godot.csproj
godot/scenes/Main.tscn
godot/scripts/GodotHost.cs
godot/icon.svg
src/U3.GodotBridge/GodotRuntimeBootstrap.cs
```

`src/U3.GodotBridge/U3GodotBridgeMarker.cs` was removed and replaced by the real bootstrap class.

## How To Open And Test

Open the `godot/` folder with a Godot 4 .NET/C# editor build.

The main scene is:

```text
res://scenes/Main.tscn
```

Run the scene/project. Expected logs:

```text
GodotHost ready
GodotRuntimeBootstrap initialized
BridgeTestBehaviour.Awake
BridgeTestBehaviour.Start
BridgeTestBehaviour.Update frame=...
BridgeTestBehaviour.FixedUpdate fixedDelta=...
```

The bridge/runtime logs use `UnityEngine.Debug.Log`, which currently writes to `Console`. Depending on how Godot is launched, those lines may appear in the terminal rather than the editor Output dock.

## What Works

- `GodotHost` is a C# script inheriting from `Godot.Node`.
- `_Ready()` initializes `GodotRuntimeBootstrap`.
- `_Process(double delta)` calls `UnityEngine.RuntimeLoop.Tick(float deltaTime)`.
- `_PhysicsProcess(double delta)` calls `UnityEngine.RuntimeLoop.TickFixed(float deltaTime)`.
- The bridge creates a fake `UnityEngine.GameObject`, attaches a test `MonoBehaviour`, and exercises `Awake`, `Start`, `Update`, and `FixedUpdate`.
- `dotnet build godot/U3GodotPort.Godot.csproj` passes.

## Local Validation

These commands passed:

```bash
dotnet build U3GodotPort.sln
dotnet test U3GodotPort.sln
dotnet build godot/U3GodotPort.Godot.csproj
```

This command was attempted:

```bash
godot --headless --path godot --quit
```

It exited successfully but reported that no loader was available for `res://scripts/GodotHost.cs`. No `godot-mono`, `godot-dotnet`, or equivalent executable was found. This indicates the installed `godot` binary is not a .NET/C# build even though the C# project compiles with `dotnet`.

## Limitations

- No Godot rendering, physics, UI, assets, scenes beyond the single host scene, or real Unity object mapping yet.
- The bridge is one-way: Godot calls the fake Unity runtime loop, but Unity shim objects do not map to Godot nodes.
- Logs from `UnityEngine.Debug` are console-based, not routed into Godot's logging API yet.
- `RuntimeLoop.Reset()` is called during bootstrap initialization to ensure a clean fake runtime state.

## Next Step

Add a small log bridge so `UnityEngine.Debug.Log`, `LogWarning`, and `LogError` can be routed to Godot `GD.Print`, `GD.PushWarning`, and `GD.PushError` when running inside Godot, while preserving console logging for tests and non-Godot hosts.
