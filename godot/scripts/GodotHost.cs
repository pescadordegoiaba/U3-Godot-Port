using Godot;
using U3.GodotBridge;

public partial class GodotHost : Node
{
    private readonly GodotRuntimeBootstrap _runtime = new();

    public override void _Ready()
    {
        _runtime.Initialize();
        UnityEngine.Debug.Log("GodotHost ready");
    }

    public override void _Process(double delta)
    {
        _runtime.Tick((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _runtime.TickFixed((float)delta);
    }

    public override void _ExitTree()
    {
        _runtime.Shutdown();
    }
}
