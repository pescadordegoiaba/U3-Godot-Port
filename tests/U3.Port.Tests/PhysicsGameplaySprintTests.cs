using SDG.Unturned;
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

        public bool RaycastResult { get; set; }

        public RaycastHit Hit { get; set; }

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
            return Array.Empty<Collider>();
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
            return false;
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

        public Vector2 mouseScrollDelta { get; set; }

        public bool anyKey => Keys.Values.Any(value => value);

        public bool anyKeyDown => KeysDown.Values.Any(value => value);

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
