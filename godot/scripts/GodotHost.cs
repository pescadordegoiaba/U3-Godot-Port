using Godot;
using U3.GodotBridge;

public partial class GodotHost : Node
{
    private readonly GodotRuntimeBootstrap _runtime = new();
    private readonly GodotSceneBridge _sceneBridge = new();
    private UnityEngine.GameObject? _parentObject;
    private UnityEngine.GameObject? _childObject;

    public override void _Ready()
    {
        _runtime.Initialize();
        CreateHierarchyDemo();
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

    private void CreateHierarchyDemo()
    {
        _parentObject = new UnityEngine.GameObject("ParentObject");
        _parentObject.transform.position = new UnityEngine.Vector3(0f, 1f, 0f);
        _parentObject.transform.localScale = new UnityEngine.Vector3(0.8f, 0.8f, 0.8f);
        _parentObject.AddComponent<MovingParentBehaviour>();

        _childObject = new UnityEngine.GameObject("ChildObject");
        _childObject.transform.SetParent(_parentObject.transform);
        _childObject.transform.localPosition = new UnityEngine.Vector3(0f, 1.25f, 0f);
        _childObject.transform.localScale = new UnityEngine.Vector3(0.45f, 0.45f, 0.45f);

        var parentNode = _sceneBridge.CreateNode(_parentObject, this);
        parentNode.AddChild(CreateBoxMesh("ParentMesh"));

        var childNode = _sceneBridge.CreateNode(_childObject, this);
        childNode.AddChild(CreateBoxMesh("ChildMesh"));

        _sceneBridge.SyncAll();
    }

    private static MeshInstance3D CreateBoxMesh(string name)
    {
        return new MeshInstance3D
        {
            Name = name,
            Mesh = new BoxMesh()
        };
    }

    private sealed class MovingParentBehaviour : UnityEngine.MonoBehaviour
    {
        private bool _loggedStart;

        public override void Start()
        {
            if (!_loggedStart)
            {
                UnityEngine.Debug.Log("MovingParentBehaviour.Start");
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
