using Godot;
using U3.GodotBridge;
using UnityColor = UnityEngine.Color;
using UnityDebug = UnityEngine.Debug;
using UnityGameObject = UnityEngine.GameObject;
using UnityLight = UnityEngine.Light;
using UnityLightType = UnityEngine.LightType;
using UnityMaterial = UnityEngine.Material;
using UnityMathf = UnityEngine.Mathf;
using UnityMesh = UnityEngine.Mesh;
using UnityMeshFilter = UnityEngine.MeshFilter;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityQuaternion = UnityEngine.Quaternion;
using UnityCamera = UnityEngine.Camera;
using UnityTime = UnityEngine.Time;
using UnityVector3 = UnityEngine.Vector3;

public partial class GodotHost : Node
{
    private readonly GodotRuntimeBootstrap _runtime = new();
    private readonly GodotSceneBridge _sceneBridge = new();
    private UnityGameObject? _parentObject;
    private UnityGameObject? _childObject;
    private UnityGameObject? _mainCamera;
    private UnityGameObject? _sunLight;

    public override void _Ready()
    {
        _runtime.Initialize();
        CreateHierarchyDemo();
        UnityDebug.Log("GodotHost ready");
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
        _parentObject = new UnityGameObject("ParentObject");
        _parentObject.transform.position = new UnityVector3(0f, 1f, 0f);
        _parentObject.transform.localScale = new UnityVector3(0.8f, 0.8f, 0.8f);
        ConfigureMesh(_parentObject, UnityMesh.CreateBox(), UnityColor.green);
        _parentObject.AddComponent<MovingParentBehaviour>();

        _childObject = new UnityGameObject("ChildObject");
        _childObject.transform.SetParent(_parentObject.transform);
        _childObject.transform.localPosition = new UnityVector3(0f, 1.25f, 0f);
        _childObject.transform.localScale = new UnityVector3(0.45f, 0.45f, 0.45f);
        ConfigureMesh(_childObject, UnityMesh.CreateSphere(), UnityColor.blue);

        _sceneBridge.CreateNode(_parentObject, this);
        _sceneBridge.CreateNode(_childObject, this);
        CreateCameraAndLight();

        _sceneBridge.SyncAll();
    }

    private void CreateCameraAndLight()
    {
        _mainCamera = new UnityGameObject("MainCamera");
        _mainCamera.transform.position = new UnityVector3(0f, 1f, 7f);
        _mainCamera.transform.LookAt(new UnityVector3(0f, 1f, 0f));
        var camera = _mainCamera.AddComponent<UnityCamera>();
        camera.fieldOfView = 60f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 100f;

        _sunLight = new UnityGameObject("SunLight");
        _sunLight.transform.localRotation = UnityQuaternion.Euler(45f, -30f, 0f);
        var light = _sunLight.AddComponent<UnityLight>();
        light.type = UnityLightType.Directional;
        light.intensity = 1.5f;
        light.color = UnityColor.white;

        _sceneBridge.CreateNode(_mainCamera, this);
        _sceneBridge.CreateNode(_sunLight, this);
    }

    private static void ConfigureMesh(UnityGameObject gameObject, UnityMesh mesh, UnityColor color)
    {
        var meshFilter = gameObject.AddComponent<UnityMeshFilter>();
        meshFilter.sharedMesh = mesh;

        var meshRenderer = gameObject.AddComponent<UnityMeshRenderer>();
        meshRenderer.material = new UnityMaterial
        {
            color = color
        };
    }

    private sealed class MovingParentBehaviour : UnityMonoBehaviour
    {
        private bool _loggedStart;

        public override void Start()
        {
            if (!_loggedStart)
            {
                UnityDebug.Log("MovingParentBehaviour.Start");
                _loggedStart = true;
            }
        }

        public override void Update()
        {
            transform.position = new UnityVector3(
                UnityMathf.Sin(UnityTime.time) * 2f,
                1f,
                0f);
        }
    }
}
