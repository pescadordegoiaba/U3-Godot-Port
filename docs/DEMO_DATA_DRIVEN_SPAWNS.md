# Demo Data Driven Spawns

## Goal

Prove that a tiny data-driven loop can work without real AssetBundles: `Resources` registers fake `TextAsset` files, `DatParser` reads them, and the bridge spawns fake Unity `GameObject`s into Godot.

## Format

Demo `.dat` files support:

- `Id`
- `Name`
- `Shape`: `Box`, `Sphere`, or `Capsule`
- `Color`: `r,g,b` or `r,g,b,a`
- `InteractPrompt`
- `Position`: `x,y,z`

## What Works

`DemoAssetRegistry` registers default in-memory text assets. `DemoDatLoader` parses a definition. `DemoSpawnDefinition.CreateGameObject()` creates mesh, material, collider and `InteractableDemo`.

## Limitations

The schema is intentionally tiny and unrelated to real U3 asset formats. Invalid data fails fast with a clear exception.
