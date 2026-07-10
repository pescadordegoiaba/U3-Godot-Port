# Godot Mouse Input Bridge

## What Works

`UnityEngine.Input` now exposes mouse delta through `mouseDelta`, `GetAxis("Mouse X")` and `GetAxis("Mouse Y")`. `GodotInputBridge` accumulates `InputEventMouseMotion` during the frame and clears the delta after the runtime tick.

`Cursor.lockState` and `Cursor.visible` are backed by the input backend. In Godot, locked cursor maps to captured mouse mode, and `Escape` releases capture.

## Supported Controls

- `W/A/S/D` and arrows for movement axes.
- Mouse motion for look.
- `Space`, `LeftShift`, `E`, `Mouse0`, `Mouse1`, `Mouse2`.
- Extra key stubs: tab, return, number keys, alt/shift/control variants.

## Limitations

There is no configurable binding system yet. `GetAxis` and `GetAxisRaw` are currently equivalent.
