# Player Controller Demo V2

## Controls

- `W/A/S/D`: move
- `LeftShift`: sprint
- mouse: look
- `Space`: jump when grounded
- `E` or left mouse: interact/raycast
- `Escape`: release mouse

## What Works

The demo player uses only fake Unity APIs: `Input`, `Physics`, `Transform`, `Debug` and `MonoBehaviour`. Movement is kinematic, gravity is manual, grounded is checked with physics queries, and interaction raycasts call `InteractableDemo.Interact`.

## Limitations

This is not Unturned gameplay. There is no real character controller, no networking, no inventory, no real assets and no animation controller.
