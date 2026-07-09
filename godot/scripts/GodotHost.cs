using Godot;
using U3.GodotBridge;

public partial class GodotHost : Node
{
    private readonly GodotRuntimeBootstrap _runtime = new();
    private readonly GodotSceneBridge _sceneBridge = new();
    private UnityEngine.GameObject? _movingCube;

    public override void _Ready()
    {
        _runtime.Initialize();
        CreateMovingCube();
        UnityEngine.Debug.Log("GodotHost ready");
    }

    public override void _Process(double delta)
    {
        _runtime.Tick((float)delta);
        _sceneBridge.SyncAll();
    }

    public override void _PhysicsProcess(double delta)
    {
        _runtime.TickFixed((float)delta);
    }

    public override void _ExitTree()
    {
        _sceneBridge.Clear();
        _runtime.Shutdown();
    }

    private void CreateMovingCube()
    {
        _movingCube = new UnityEngine.GameObject("MovingCube");
        _movingCube.transform.position = new UnityEngine.Vector3(0f, 1f, 0f);
        _movingCube.transform.localScale = new UnityEngine.Vector3(0.75f, 0.75f, 0.75f);
        _movingCube.AddComponent<MovingCubeBehaviour>();

        var node = _sceneBridge.CreateNode(_movingCube, this);
        var mesh = new MeshInstance3D
        {
            Name = "MovingCubeMesh",
            Mesh = new BoxMesh()
        };

        node.AddChild(mesh);
    }

    private sealed class MovingCubeBehaviour : UnityEngine.MonoBehaviour
    {
        private bool _loggedStart;

        public override void Start()
        {
            if (!_loggedStart)
            {
                UnityEngine.Debug.Log("MovingCubeBehaviour.Start");
                _loggedStart = true;
            }
        }

        public override void Update()
        {
            transform.position = new UnityEngine.Vector3(
                UnityEngine.Mathf.Sin(UnityEngine.Time.time) * 2f,
                1f,
                0f);
        }
    }
}
