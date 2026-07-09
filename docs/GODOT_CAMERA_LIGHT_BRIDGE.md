# Godot Camera and Light Bridge

## Camera

`UnityEngine.Shim` now includes a minimal `Camera` component. It inherits `Behaviour` and supports:

- `enabled`
- `fieldOfView`
- `nearClipPlane`
- `farClipPlane`
- `Camera.main`

`GameObject.AddComponent<Camera>()` assigns `Camera.main` when no main camera exists yet. Runtime reset and destroying the owning `GameObject` clear the main camera if needed.

`GodotSceneBridge` maps `Camera` to a child `Godot.Camera3D`:

- `enabled && activeInHierarchy` -> `Camera3D.Current`
- `enabled && activeInHierarchy` -> `Camera3D.Visible`
- `fieldOfView` -> `Fov`
- `nearClipPlane` -> `Near`
- `farClipPlane` -> `Far`

## Light

`UnityEngine.Shim` now includes a minimal `Light` component and `LightType` enum:

- `Directional`
- `Point`
- `Spot`

`GodotSceneBridge` maps lights to Godot nodes:

- `Directional` -> `DirectionalLight3D`
- `Point` -> `OmniLight3D`
- `Spot` -> `SpotLight3D`

Synced properties:

- `enabled && activeInHierarchy` -> `Visible`
- `intensity` -> `LightEnergy`
- `color` -> `LightColor`
- `range` -> `OmniRange` or `SpotRange`

If `Light.type` changes, the bridge replaces the Godot light node with the matching type.

## Demo

`GodotHost` creates:

- `MainCamera` with a shim `Camera`
- `SunLight` with a shim directional `Light`
- `ParentObject` and `ChildObject` using the existing mesh bridge

The scene no longer needs manually authored camera or light nodes for the smoke test.

## Limitations

- No clear flags, projection modes, culling masks, target textures, sky, environment, shadows, spot angle or light cookies.
- Camera rotation uses the existing direct quaternion mapping only.
- Directional lights ignore `range`.
- No real assets, physics, UI or networking.

## Manual Test

Open `godot/` in Godot 4 C#/.NET and run `res://scenes/Main.tscn`.

Expected result:

- `ParentObject` remains visible as a green box.
- `ChildObject` remains visible as a blue sphere.
- The view comes from the generated `MainCamera`.
- Lighting comes from the generated `SunLight`.

Build validation:

```bash
dotnet build godot/U3GodotPort.Godot.csproj
```

## Next Step

Add minimal rotation helpers such as `Quaternion.Euler` or `Transform.LookAt` so generated cameras and lights can be aimed without Godot-specific code in the host.
