using UnityEngine;

namespace U3.GodotBridge;

public static class DemoAssetRegistry
{
    public static readonly string[] SpawnPaths =
    {
        "demo/spawns/red_box",
        "demo/spawns/blue_sphere",
        "demo/spawns/gold_capsule",
        "demo/spawns/green_box",
        "demo/spawns/cyan_sphere"
    };

    public static void RegisterDefaults()
    {
        Resources.Register("demo/spawns/red_box", new TextAsset("""
            Id 1
            Name RedBox
            Shape Box
            Color 0.9,0.15,0.15
            InteractPrompt Inspect
            Position -3,0.75,-4
            """));

        Resources.Register("demo/spawns/blue_sphere", new TextAsset("""
            Id 2
            Name BlueSphere
            Shape Sphere
            Color 0.1,0.25,1
            InteractPrompt Inspect
            Position 0,0.75,-5
            """));

        Resources.Register("demo/spawns/gold_capsule", new TextAsset("""
            Id 3
            Name GoldCapsule
            Shape Capsule
            Color 1,0.7,0.1
            InteractPrompt Use
            Position 3,1,-4
            """));

        Resources.Register("demo/spawns/green_box", new TextAsset("""
            Id 4
            Name GreenBox
            Shape Box
            Color 0.1,0.8,0.25
            InteractPrompt Open
            Position -1.5,0.75,-8
            """));

        Resources.Register("demo/spawns/cyan_sphere", new TextAsset("""
            Id 5
            Name CyanSphere
            Shape Sphere
            Color 0,0.8,0.9
            InteractPrompt Scan
            Position 1.5,0.75,-8
            """));
    }
}
