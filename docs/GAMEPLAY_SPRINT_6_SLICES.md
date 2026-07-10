# Gameplay Sprint 6 Slices

## Summary

This sprint advances the custom playable demo without porting real Unturned gameplay. It adds overlap queries, better fake physics state, mouse input, player jump/look/interact, and data-driven demo spawns.

## Completed Slices

1. Godot overlap queries through `IntersectShape`.
2. Rigidbody/collider sync improvements and fake material/controller stubs.
3. Mouse input and cursor bridge.
4. Player controller V2 with grounded, jump, look and interaction.
5. Demo data spawns using `Resources`, `TextAsset` and `DatParser`.
6. README and docs refresh.

## What Is Fake

Rigidbodies are not simulated. AssetBundles are not implemented. UI, Steam, NetTransport, multiplayer and real Unturned systems are untouched.

## Recommended Next Steps

1. Improve capsule orientation in Godot overlap queries.
2. Add collision debug visualization.
3. Add configurable input bindings.
4. Add a minimal health/damage demo component.
5. Evaluate another safe UnityEx helper.
6. Map non-gameplay U3 data classes.
7. Add a small scene reset/reload flow.
8. Improve fake `CharacterController` grounding.
9. Add material/color changes driven by dat values.
10. Document the next real U3 compile slice.
