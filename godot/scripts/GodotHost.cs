using Godot;
using U3.GodotBridge;
using UnityBoxCollider = UnityEngine.BoxCollider;
using UnityCamera = UnityEngine.Camera;
using UnityCapsuleCollider = UnityEngine.CapsuleCollider;
using UnityColor = UnityEngine.Color;
using UnityDebug = UnityEngine.Debug;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityLight = UnityEngine.Light;
using UnityLightType = UnityEngine.LightType;
using UnityMaterial = UnityEngine.Material;
using UnityMesh = UnityEngine.Mesh;
using UnityMeshFilter = UnityEngine.MeshFilter;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityPhysics = UnityEngine.Physics;
using UnityRigidbody = UnityEngine.Rigidbody;
using UnitySphereCollider = UnityEngine.SphereCollider;
using UnityVector3 = UnityEngine.Vector3;

public partial class GodotHost : Node
{
    private readonly GodotRuntimeBootstrap _runtime = new();
    private readonly GodotSceneBridge _sceneBridge = new();
    private readonly GodotInputBridge _inputBridge = new();
    private Node3D? _worldRoot;

    public override void _Ready()
    {
        _worldRoot = new Node3D { Name = "U3FakeWorld" };
        AddChild(_worldRoot);

        _runtime.Initialize();
        UnityInput.SetBackend(_inputBridge);
        UnityPhysics.SetBackend(new GodotPhysicsBridge(_worldRoot, _sceneBridge));
        CreatePlayableDemo(_worldRoot);
        UnityDebug.Log("GodotHost ready");
    }

    public override void _Process(double delta)
    {
        _inputBridge.UpdateFrame();
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
        UnityPhysics.ResetBackend();
        UnityInput.ResetBackend();
        _runtime.Shutdown();
    }

    private void CreatePlayableDemo(Node parent)
    {
        var floor = CreateBox("Ground", new UnityVector3(0f, -0.25f, 0f), new UnityVector3(14f, 0.5f, 14f), new UnityColor(0.35f, 0.35f, 0.35f));
        var floorCollider = floor.AddComponent<UnityBoxCollider>();
        floorCollider.size = UnityVector3.one;

        var targetA = CreateBox("TargetBox", new UnityVector3(-2.5f, 0.75f, -4f), UnityVector3.one, UnityColor.green);
        targetA.AddComponent<UnityBoxCollider>().size = UnityVector3.one;

        var targetB = CreateSphere("TargetSphere", new UnityVector3(2.5f, 0.75f, -4f), new UnityVector3(1f, 1f, 1f), UnityColor.blue);
        targetB.AddComponent<UnitySphereCollider>().radius = 0.5f;

        var targetC = CreateBox("TargetCapsule", new UnityVector3(0f, 1f, -7f), new UnityVector3(0.7f, 1.8f, 0.7f), new UnityColor(1f, 0.7f, 0.2f));
        var capsule = targetC.AddComponent<UnityCapsuleCollider>();
        capsule.radius = 0.35f;
        capsule.height = 1.8f;

        var player = CreateBox("Player", new UnityVector3(0f, 0.75f, 3f), new UnityVector3(0.65f, 1.5f, 0.65f), new UnityColor(0.9f, 0.2f, 0.2f));
        var playerCollider = player.AddComponent<UnityCapsuleCollider>();
        playerCollider.radius = 0.35f;
        playerCollider.height = 1.5f;
        var playerBody = player.AddComponent<UnityRigidbody>();
        playerBody.isKinematic = true;

        var camera = new UnityGameObject("MainCamera");
        camera.transform.position = player.transform.position + new UnityVector3(0f, 1.4f, 5f);
        camera.transform.LookAt(player.transform.position + new UnityVector3(0f, 1f, 0f));
        var cameraComponent = camera.AddComponent<UnityCamera>();
        cameraComponent.fieldOfView = 65f;
        cameraComponent.nearClipPlane = 0.1f;
        cameraComponent.farClipPlane = 100f;

        var controller = player.AddComponent<PlayerControllerDemo>();
        controller.CameraTransform = camera.transform;

        var sunLight = new UnityGameObject("SunLight");
        sunLight.transform.localEulerAngles = new UnityVector3(45f, -30f, 0f);
        var light = sunLight.AddComponent<UnityLight>();
        light.type = UnityLightType.Directional;
        light.intensity = 1.5f;
        light.color = UnityColor.white;

        foreach (var gameObject in new[] { floor, targetA, targetB, targetC, player, camera, sunLight })
        {
            _sceneBridge.CreateNode(gameObject, parent);
        }

        _sceneBridge.SyncAll();
    }

    private static UnityGameObject CreateBox(string name, UnityVector3 position, UnityVector3 scale, UnityColor color)
    {
        var gameObject = new UnityGameObject(name);
        gameObject.transform.position = position;
        gameObject.transform.localScale = scale;
        ConfigureMesh(gameObject, UnityMesh.CreateBox(), color);
        return gameObject;
    }

    private static UnityGameObject CreateSphere(string name, UnityVector3 position, UnityVector3 scale, UnityColor color)
    {
        var gameObject = new UnityGameObject(name);
        gameObject.transform.position = position;
        gameObject.transform.localScale = scale;
        ConfigureMesh(gameObject, UnityMesh.CreateSphere(), color);
        return gameObject;
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
}
