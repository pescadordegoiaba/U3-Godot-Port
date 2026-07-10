using SDG.Unturned;
using U3.GodotBridge;
using UnityEngine;
using Xunit;

namespace U3.Port.Tests;

public class PhysicsGameplaySprintTests
{
    public PhysicsGameplaySprintTests()
    {
        RuntimeLoop.Reset();
    }

    [Fact]
    public void PhysicsRaycastCallsBackendAndFillsHit()
    {
        var gameObject = new GameObject("target");
        var collider = gameObject.AddComponent<BoxCollider>();
        var backend = new FakePhysicsBackend
        {
            RaycastResult = true,
            Hit = new RaycastHit
            {
                collider = collider,
                point = new Vector3(0f, 0f, 3f),
                normal = Vector3.back,
                distance = 3f
            }
        };
        Physics.SetBackend(backend);

        var result = Physics.Raycast(new Ray(Vector3.zero, Vector3.forward), out var hit, 10f);

        Assert.True(result);
        Assert.True(backend.RaycastCalled);
        Assert.Same(collider, hit.collider);
        Assert.Same(gameObject.transform, hit.transform);
        AssertVector3(new Vector3(0f, 0f, 3f), hit.point);
        AssertVector3(Vector3.back, hit.normal);
        Assert.Equal(3f, hit.distance);
    }

    [Fact]
    public void PhysicsRaycastReturnsFalseWithoutBackendOrColliders()
    {
        Physics.ResetBackend();

        Assert.False(Physics.Raycast(new Ray(Vector3.zero, Vector3.forward), out _));
    }

    [Fact]
    public void PhysicsResetBackendRestoresFallback()
    {
        Physics.SetBackend(new FakePhysicsBackend { RaycastResult = true });
        Physics.ResetBackend();

        Assert.False(Physics.Raycast(new Ray(Vector3.zero, Vector3.forward), out _));
    }

    [Fact]
    public void PhysicsFallbackRaycastHitsRegisteredBoxCollider()
    {
        var gameObject = new GameObject("box");
        gameObject.transform.position = new Vector3(0f, 0f, 5f);
        var collider = gameObject.AddComponent<BoxCollider>();
        collider.size = Vector3.one;

        var hit = Physics.Raycast(new Ray(Vector3.zero, Vector3.forward), out var hitInfo, 10f);

        Assert.True(hit);
        Assert.Same(collider, hitInfo.collider);
        Assert.Equal(4.5f, hitInfo.distance, 3);
    }

    [Fact]
    public void ColliderRegistryRemovesDestroyedColliders()
    {
        var gameObject = new GameObject("collider");
        var collider = gameObject.AddComponent<BoxCollider>();

        Assert.Contains(collider, Physics.AllColliders);

        UnityEngine.Object.Destroy(collider);

        Assert.DoesNotContain(collider, Physics.AllColliders);
    }

    [Fact]
    public void BoxSphereAndCapsuleBoundsWork()
    {
        var boxObject = new GameObject("box");
        boxObject.transform.position = new Vector3(1f, 2f, 3f);
        boxObject.transform.localScale = new Vector3(2f, 2f, 2f);
        var box = boxObject.AddComponent<BoxCollider>();
        box.center = new Vector3(1f, 0f, 0f);
        box.size = Vector3.one;

        AssertVector3(new Vector3(3f, 2f, 3f), box.bounds.center);
        AssertVector3(new Vector3(2f, 2f, 2f), box.bounds.size);

        var sphereObject = new GameObject("sphere");
        sphereObject.transform.position = new Vector3(0f, 1f, 0f);
        var sphere = sphereObject.AddComponent<SphereCollider>();
        sphere.radius = 2f;
        AssertVector3(new Vector3(4f, 4f, 4f), sphere.bounds.size);

        var capsuleObject = new GameObject("capsule");
        var capsule = capsuleObject.AddComponent<CapsuleCollider>();
        capsule.radius = 0.5f;
        capsule.height = 3f;
        AssertVector3(new Vector3(1f, 3f, 1f), capsule.bounds.size);
    }

    [Fact]
    public void ColliderClosestPointUsesSimpleShapeBounds()
    {
        var gameObject = new GameObject("box");
        var collider = gameObject.AddComponent<BoxCollider>();
        collider.size = Vector3.one;

        AssertVector3(new Vector3(0.5f, 0f, 0f), collider.ClosestPoint(new Vector3(10f, 0f, 0f)));
    }

    [Fact]
    public void RigidbodyMovePositionAndRotationUpdateTransform()
    {
        var gameObject = new GameObject("body");
        var rigidbody = gameObject.AddComponent<Rigidbody>();
        var rotation = Quaternion.Euler(0f, 90f, 0f);

        rigidbody.MovePosition(new Vector3(1f, 2f, 3f));
        rigidbody.MoveRotation(rotation);

        AssertVector3(new Vector3(1f, 2f, 3f), gameObject.transform.position);
        AssertVector3(Vector3.right, gameObject.transform.forward, tolerance: 0.001f);
    }

    [Fact]
    public void ColliderAttachedRigidbodyUpdatesWhenRigidbodyAdded()
    {
        var gameObject = new GameObject("body");
        var collider = gameObject.AddComponent<BoxCollider>();
        var rigidbody = gameObject.AddComponent<Rigidbody>();

        Assert.Same(rigidbody, collider.attachedRigidbody);
    }

    [Fact]
    public void InputFallbackReturnsDefaultValues()
    {
        Input.ResetBackend();

        Assert.False(Input.GetKey(KeyCode.W));
        Assert.False(Input.GetKeyDown(KeyCode.W));
        Assert.False(Input.GetButton("Jump"));
        Assert.Equal(0f, Input.GetAxis("Horizontal"));
        AssertVector2(Vector2.zero, Input.mouseScrollDelta);
    }

    [Fact]
    public void InputBackendReturnsConfiguredValues()
    {
        var backend = new FakeInputBackend();
        backend.Keys[KeyCode.W] = true;
        backend.KeysDown[KeyCode.Space] = true;
        backend.Axes["Vertical"] = 1f;
        backend.Buttons["Jump"] = true;
        Input.SetBackend(backend);

        Assert.True(Input.GetKey(KeyCode.W));
        Assert.True(Input.GetKeyDown(KeyCode.Space));
        Assert.True(Input.GetButton("Jump"));
        Assert.Equal(1f, Input.GetAxis("Vertical"));
        Assert.True(Input.anyKey);
    }

    [Fact]
    public void InputResetBackendRestoresDefaultValues()
    {
        var backend = new FakeInputBackend();
        backend.Keys[KeyCode.W] = true;
        Input.SetBackend(backend);

        Input.ResetBackend();

        Assert.False(Input.GetKey(KeyCode.W));
    }

    [Fact]
    public void ColliderExVolumeAndCenterHelpersWork()
    {
        var boxObject = new GameObject("box");
        boxObject.transform.position = new Vector3(1f, 0f, 0f);
        var box = boxObject.AddComponent<BoxCollider>();
        box.size = new Vector3(2f, 3f, 4f);
        box.center = new Vector3(1f, 0f, 0f);
        Assert.Equal(24f, box.GetBoxVolume());
        AssertVector3(new Vector3(2f, 0f, 0f), box.TransformBoxCenter());

        var sphere = new GameObject("sphere").AddComponent<SphereCollider>();
        sphere.radius = 1f;
        Assert.Equal(4f / 3f * Mathf.PI, sphere.GetSphereVolume(), 4);

        var capsule = new GameObject("capsule").AddComponent<CapsuleCollider>();
        capsule.radius = 1f;
        capsule.height = 2f;
        Assert.Equal((Mathf.PI * 2f) + (4f / 3f * Mathf.PI), capsule.GetCapsuleVolume(), 4);
    }

    [Fact]
    public void PhysicsFallbackOverlapAndCheckQueriesWork()
    {
        var nearSphere = new GameObject("near").AddComponent<SphereCollider>();
        nearSphere.transform.position = new Vector3(0f, 0f, 0f);
        nearSphere.radius = 1f;

        var farSphere = new GameObject("far").AddComponent<SphereCollider>();
        farSphere.transform.position = new Vector3(10f, 0f, 0f);
        farSphere.radius = 1f;

        Assert.Contains(nearSphere, Physics.OverlapSphere(Vector3.zero, 2f));
        Assert.DoesNotContain(farSphere, Physics.OverlapSphere(Vector3.zero, 2f));
        Assert.True(Physics.CheckSphere(Vector3.zero, 2f));
        Assert.False(Physics.CheckSphere(new Vector3(30f, 0f, 0f), 1f));

        var buffer = new Collider[1];
        Assert.Equal(1, Physics.OverlapSphereNonAlloc(Vector3.zero, 20f, buffer));
        Assert.NotNull(buffer[0]);
    }

    [Fact]
    public void PhysicsFallbackOverlapBoxAndCapsuleWork()
    {
        var box = new GameObject("box").AddComponent<BoxCollider>();
        box.transform.position = Vector3.zero;
        box.size = Vector3.one;

        Assert.Contains(box, Physics.OverlapBox(Vector3.zero, Vector3.one));
        Assert.True(Physics.CheckBox(Vector3.zero, Vector3.one));
        Assert.Contains(box, Physics.OverlapCapsule(Vector3.down, Vector3.up, 1f));
        Assert.True(Physics.CheckCapsule(Vector3.down, Vector3.up, 1f));
    }

    [Fact]
    public void PhysicsOverlapQueriesCallBackend()
    {
        var collider = new GameObject("backend").AddComponent<BoxCollider>();
        var backend = new FakePhysicsBackend
        {
            OverlapResults = new[] { collider },
            CheckResult = true
        };
        Physics.SetBackend(backend);

        Assert.Same(collider, Assert.Single(Physics.OverlapSphere(Vector3.zero, 1f)));
        Assert.Same(collider, Assert.Single(Physics.OverlapBox(Vector3.zero, Vector3.one)));
        Assert.Same(collider, Assert.Single(Physics.OverlapCapsule(Vector3.down, Vector3.up, 1f)));
        Assert.True(Physics.CheckSphere(Vector3.zero, 1f));
        Assert.True(Physics.CheckBox(Vector3.zero, Vector3.one));
        Assert.True(Physics.CheckCapsule(Vector3.down, Vector3.up, 1f));
        Assert.True(backend.OverlapSphereCalled);
        Assert.True(backend.OverlapBoxCalled);
        Assert.True(backend.OverlapCapsuleCalled);
    }

    [Fact]
    public void RigidbodyAddForceAndSleepStubsWork()
    {
        var rigidbody = new GameObject("body").AddComponent<Rigidbody>();
        rigidbody.mass = 2f;

        rigidbody.AddForce(new Vector3(2f, 0f, 0f), ForceMode.Impulse);

        AssertVector3(new Vector3(1f, 0f, 0f), rigidbody.velocity);
        rigidbody.Sleep();
        Assert.True(rigidbody.IsSleeping());
        rigidbody.WakeUp();
        Assert.False(rigidbody.IsSleeping());
    }

    [Fact]
    public void CharacterControllerMoveUpdatesTransformAndVelocity()
    {
        Time.deltaTime = 0.5f;
        var controller = new GameObject("controller").AddComponent<CharacterController>();

        controller.Move(new Vector3(2f, 0f, 0f));

        AssertVector3(new Vector3(2f, 0f, 0f), controller.transform.position);
        AssertVector3(new Vector3(4f, 0f, 0f), controller.velocity);
    }

    [Fact]
    public void ColliderMaterialStoresPhysicMaterial()
    {
        var collider = new GameObject("collider").AddComponent<BoxCollider>();
        var material = new PhysicMaterial { bounciness = 0.5f };

        collider.material = material;

        Assert.Same(material, collider.sharedMaterial);
        Assert.Equal(0.5f, collider.material!.bounciness);
    }

    [Fact]
    public void InputMouseAndCursorBackendWork()
    {
        var backend = new FakeInputBackend();
        backend.Axes["Mouse X"] = 3f;
        backend.mouseDelta = new Vector2(3f, -2f);
        Input.SetBackend(backend);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Assert.Equal(3f, Input.GetAxisRaw("Mouse X"));
        AssertVector2(new Vector2(3f, -2f), Input.mouseDelta);
        Assert.Equal(CursorLockMode.Locked, backend.cursorLockState);
        Assert.False(backend.cursorVisible);
    }

    [Fact]
    public void PlayerControllerDemoMovesJumpsAndInteractsWithFakeBackends()
    {
        var player = new GameObject("player");
        var camera = new GameObject("camera");
        var controller = player.AddComponent<PlayerControllerDemo>();
        controller.CameraTransform = camera.transform;

        var target = new GameObject("target");
        var collider = target.AddComponent<BoxCollider>();
        var interactable = target.AddComponent<InteractableDemo>();
        var physicsBackend = new FakePhysicsBackend
        {
            CheckResult = true,
            RaycastResult = true,
            Hit = new RaycastHit { collider = collider, point = Vector3.forward, normal = Vector3.back, distance = 1f }
        };
        var inputBackend = new FakeInputBackend();
        inputBackend.Axes["Vertical"] = 1f;
        inputBackend.KeysDown[KeyCode.Space] = true;
        inputBackend.KeysDown[KeyCode.E] = true;
        Physics.SetBackend(physicsBackend);
        Input.SetBackend(inputBackend);

        RuntimeLoop.Tick(0.1f);

        Assert.NotEqual(Vector3.zero.z, player.transform.position.z);
        Assert.True(interactable.WasInteracted);
    }

    [Fact]
    public void DemoDatLoaderParsesAndSpawnsDefinition()
    {
        var definition = DemoDatLoader.ParseDefinition("""
            Id 42
            Name TestBox
            Shape Box
            Color 0.1,0.2,0.3
            InteractPrompt Inspect
            Position 1,2,3
            """);

        var gameObject = definition.CreateGameObject();

        Assert.Equal(42, definition.Id);
        Assert.Equal("TestBox", gameObject.name);
        Assert.NotNull(gameObject.GetComponent<MeshRenderer>());
        Assert.NotNull(gameObject.GetComponent<BoxCollider>());
        Assert.NotNull(gameObject.GetComponent<InteractableDemo>());
    }

    [Fact]
    public void DemoDatLoaderUsesResourcesTextAssetAndRejectsInvalidData()
    {
        Resources.Register("demo/test", new TextAsset("""
            Id 7
            Name ResourceSphere
            Shape Sphere
            Position 0,1,2
            """));

        var definition = DemoDatLoader.LoadDefinition("demo/test");

        Assert.Equal("ResourceSphere", definition.Name);
        Assert.Equal(DemoSpawnShape.Sphere, definition.Shape);
        Assert.Throws<FormatException>(() => DemoDatLoader.ParseDefinition("Shape Box"));
    }

    private static void AssertVector3(Vector3 expected, Vector3 actual, float tolerance = 0.0001f)
    {
        Assert.Equal(expected.x, actual.x, tolerance);
        Assert.Equal(expected.y, actual.y, tolerance);
        Assert.Equal(expected.z, actual.z, tolerance);
    }

    private static void AssertVector2(Vector2 expected, Vector2 actual, float tolerance = 0.0001f)
    {
        Assert.Equal(expected.x, actual.x, tolerance);
        Assert.Equal(expected.y, actual.y, tolerance);
    }

    private sealed class FakePhysicsBackend : IPhysicsBackend
    {
        public bool RaycastCalled { get; private set; }
        public bool OverlapSphereCalled { get; private set; }
        public bool OverlapBoxCalled { get; private set; }
        public bool OverlapCapsuleCalled { get; private set; }

        public bool RaycastResult { get; set; }
        public bool CheckResult { get; set; }

        public RaycastHit Hit { get; set; }
        public Collider[] OverlapResults { get; set; } = Array.Empty<Collider>();

        public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            RaycastCalled = true;
            hitInfo = Hit;
            return RaycastResult;
        }

        public RaycastHit[] RaycastAll(Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return RaycastResult ? new[] { Hit } : Array.Empty<RaycastHit>();
        }

        public Collider[] OverlapSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            OverlapSphereCalled = true;
            return OverlapResults;
        }

        public Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            OverlapBoxCalled = true;
            return OverlapResults;
        }

        public Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            OverlapCapsuleCalled = true;
            return OverlapResults;
        }

        public int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return 0;
        }

        public int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return 0;
        }

        public int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return 0;
        }

        public bool CheckSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return CheckResult;
        }

        public bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return CheckResult;
        }

        public bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return CheckResult;
        }
    }

    private sealed class FakeInputBackend : IInputBackend
    {
        public Dictionary<KeyCode, bool> Keys { get; } = new();

        public Dictionary<KeyCode, bool> KeysDown { get; } = new();

        public Dictionary<KeyCode, bool> KeysUp { get; } = new();

        public Dictionary<string, float> Axes { get; } = new(StringComparer.Ordinal);

        public Dictionary<string, bool> Buttons { get; } = new(StringComparer.Ordinal);

        public Vector3 mousePosition { get; set; }

        public Vector2 mouseDelta { get; set; }

        public Vector2 mouseScrollDelta { get; set; }

        public bool anyKey => Keys.Values.Any(value => value);

        public bool anyKeyDown => KeysDown.Values.Any(value => value);

        public CursorLockMode cursorLockState { get; set; }

        public bool cursorVisible { get; set; } = true;

        public bool GetKey(KeyCode key) => Keys.GetValueOrDefault(key);

        public bool GetKeyDown(KeyCode key) => KeysDown.GetValueOrDefault(key);

        public bool GetKeyUp(KeyCode key) => KeysUp.GetValueOrDefault(key);

        public float GetAxis(string axisName) => Axes.GetValueOrDefault(axisName);

        public float GetAxisRaw(string axisName) => Axes.GetValueOrDefault(axisName);

        public bool GetButton(string buttonName) => Buttons.GetValueOrDefault(buttonName);

        public bool GetButtonDown(string buttonName) => Buttons.GetValueOrDefault(buttonName);

        public bool GetButtonUp(string buttonName) => false;

        public void ResetInputAxes()
        {
            Keys.Clear();
            KeysDown.Clear();
            KeysUp.Clear();
            Axes.Clear();
            Buttons.Clear();
        }
    }
}
