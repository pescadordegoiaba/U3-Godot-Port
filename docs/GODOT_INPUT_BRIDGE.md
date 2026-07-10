# Godot Input Bridge

## Architecture

`UnityEngine.Input` now delegates to an optional `IInputBackend`. Without a backend it returns safe defaults. `GodotInputBridge` implements the backend and is installed by `GodotHost`.

## Supported Input

- Keys: `W`, `A`, `S`, `D`, arrows, `Space`, `LeftShift`, `LeftControl`, `Escape`, `E`, `Q`, `R`, `F`
- Mouse buttons: `Mouse0`, `Mouse1`
- Axes: `Horizontal`, `Vertical`
- Buttons: `Jump`, `Sprint`, `Interact`, `Fire1`, `Fire2`

`GodotInputBridge.UpdateFrame()` snapshots state each frame so `GetKeyDown` and `GetKeyUp` work.

## Godot Actions

`project.godot` defines action names for movement, jump, sprint, interact, fire and aim. The bridge also reads raw keys directly, so the demo works even before action events are customized in the editor.

## Limitations

- Mouse look is not implemented yet.
- `mousePosition` is minimal and not intended as full Unity compatibility.
- Axis smoothing is not implemented; `GetAxis` and `GetAxisRaw` currently match.

## Next Step

Add mouse-look deltas and configurable key bindings once the player movement loop stabilizes.
