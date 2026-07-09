using UnityEngine;
using Xunit;

namespace U3.Port.Tests;

public class UnityEngineShimTests
{
    public UnityEngineShimTests()
    {
        RuntimeLoop.Reset();
    }

    [Fact]
    public void GameObjectCreatesTransformAutomatically()
    {
        var gameObject = new GameObject();

        Assert.NotNull(gameObject.transform);
        Assert.Same(gameObject, gameObject.transform.gameObject);
    }

    [Fact]
    public void AddComponentAddsComponent()
    {
        var gameObject = new GameObject();

        var component = gameObject.AddComponent<TestComponent>();

        Assert.NotNull(component);
        Assert.Same(gameObject, component.gameObject);
    }

    [Fact]
    public void GetComponentFindsComponent()
    {
        var gameObject = new GameObject();
        var component = gameObject.AddComponent<TestComponent>();

        var found = gameObject.GetComponent<TestComponent>();

        Assert.Same(component, found);
    }

    [Fact]
    public void TryGetComponentFindsExistingComponent()
    {
        var gameObject = new GameObject();
        var component = gameObject.AddComponent<TestComponent>();

        var found = gameObject.TryGetComponent<TestComponent>(out var result);

        Assert.True(found);
        Assert.Same(component, result);
    }

    [Fact]
    public void TryGetComponentReturnsFalseForMissingComponent()
    {
        var gameObject = new GameObject();

        var found = gameObject.TryGetComponent<TestComponent>(out var result);

        Assert.False(found);
        Assert.Null(result);
    }

    [Fact]
    public void ComponentTransformPointsToGameObjectTransform()
    {
        var gameObject = new GameObject();
        var component = gameObject.AddComponent<TestComponent>();

        Assert.Same(gameObject.transform, component.transform);
    }

    [Fact]
    public void Vector3OperatorsWork()
    {
        var left = new Vector3(1f, 2f, 3f);
        var right = new Vector3(4f, 5f, 6f);

        AssertVector3(new Vector3(5f, 7f, 9f), left + right);
        AssertVector3(new Vector3(3f, 3f, 3f), right - left);
        AssertVector3(new Vector3(2f, 4f, 6f), left * 2f);
        AssertVector3(new Vector3(2f, 4f, 6f), 2f * left);
    }

    [Fact]
    public void MathfClampAndLerpWork()
    {
        Assert.Equal(5f, Mathf.Clamp(5f, 0f, 10f));
        Assert.Equal(0f, Mathf.Clamp(-1f, 0f, 10f));
        Assert.Equal(10f, Mathf.Clamp(11f, 0f, 10f));

        Assert.Equal(5f, Mathf.Lerp(0f, 10f, 0.5f));
        Assert.Equal(0f, Mathf.Lerp(0f, 10f, -1f));
        Assert.Equal(10f, Mathf.Lerp(0f, 10f, 2f));
    }

    [Fact]
    public void TransformParentAddsChildToParent()
    {
        var parent = new GameObject("parent");
        var child = new GameObject("child");

        child.transform.parent = parent.transform;

        Assert.Same(parent.transform, child.transform.parent);
        Assert.Equal(1, parent.transform.childCount);
        Assert.Same(child.transform, parent.transform.GetChild(0));
    }

    [Fact]
    public void TransformParentRemovesChildFromOldParent()
    {
        var oldParent = new GameObject("old-parent");
        var newParent = new GameObject("new-parent");
        var child = new GameObject("child");

        child.transform.SetParent(oldParent.transform);
        child.transform.SetParent(newParent.transform);

        Assert.Equal(0, oldParent.transform.childCount);
        Assert.Equal(1, newParent.transform.childCount);
        Assert.Same(child.transform, newParent.transform.GetChild(0));
    }

    [Fact]
    public void TransformRootReturnsTopLevelParent()
    {
        var root = new GameObject("root");
        var middle = new GameObject("middle");
        var leaf = new GameObject("leaf");

        middle.transform.SetParent(root.transform);
        leaf.transform.SetParent(middle.transform);

        Assert.Same(root.transform, leaf.transform.root);
    }

    [Fact]
    public void TransformRootUpdatesAfterParentChange()
    {
        var oldRoot = new GameObject("old-root");
        var newRoot = new GameObject("new-root");
        var child = new GameObject("child");

        child.transform.SetParent(oldRoot.transform);
        child.transform.SetParent(newRoot.transform);

        Assert.Equal(0, oldRoot.transform.childCount);
        Assert.Equal(1, newRoot.transform.childCount);
        Assert.Same(newRoot.transform, child.transform.root);
    }

    [Fact]
    public void ActiveInHierarchyRespectsInactiveParent()
    {
        var parent = new GameObject("parent");
        var child = new GameObject("child");
        child.transform.SetParent(parent.transform);

        parent.SetActive(false);

        Assert.False(parent.activeInHierarchy);
        Assert.False(child.activeInHierarchy);

        parent.SetActive(true);

        Assert.True(parent.activeInHierarchy);
        Assert.True(child.activeInHierarchy);
    }

    [Fact]
    public void GameObjectFindUsesRegistry()
    {
        var gameObject = new GameObject("tracked");

        Assert.Same(gameObject, GameObject.Find("tracked"));

        UnityEngine.Object.Destroy(gameObject);

        Assert.Null(GameObject.Find("tracked"));
    }

    [Fact]
    public void RuntimeLoopCallsAwakeOnce()
    {
        var behaviour = new GameObject().AddComponent<CountingBehaviour>();

        RuntimeLoop.Tick(0.1f);
        RuntimeLoop.Tick(0.1f);

        Assert.Equal(1, behaviour.AwakeCount);
    }

    [Fact]
    public void RuntimeLoopCallsStartOnce()
    {
        var behaviour = new GameObject().AddComponent<CountingBehaviour>();

        RuntimeLoop.Tick(0.1f);
        RuntimeLoop.Tick(0.1f);

        Assert.Equal(1, behaviour.StartCount);
    }

    [Fact]
    public void RuntimeLoopCallsUpdateOnEachTick()
    {
        var behaviour = new GameObject().AddComponent<CountingBehaviour>();

        RuntimeLoop.Tick(0.1f);
        RuntimeLoop.Tick(0.2f);

        Assert.Equal(2, behaviour.UpdateCount);
        Assert.Equal(2, behaviour.LateUpdateCount);
        Assert.Equal(2, Time.frameCount);
        Assert.Equal(0.2f, Time.deltaTime);
    }

    [Fact]
    public void RuntimeLoopCallsFixedUpdateOnTickFixed()
    {
        var behaviour = new GameObject().AddComponent<CountingBehaviour>();

        RuntimeLoop.TickFixed(0.05f);
        RuntimeLoop.TickFixed();

        Assert.Equal(2, behaviour.FixedUpdateCount);
        Assert.Equal(0.05f, Time.fixedDeltaTime);
    }

    [Fact]
    public void RuntimeLoopDoesNotCallUpdateWhenBehaviourDisabled()
    {
        var behaviour = new GameObject().AddComponent<CountingBehaviour>();
        behaviour.enabled = false;

        RuntimeLoop.Tick(0.1f);

        Assert.Equal(1, behaviour.AwakeCount);
        Assert.Equal(1, behaviour.StartCount);
        Assert.Equal(0, behaviour.UpdateCount);
    }

    [Fact]
    public void RuntimeLoopDoesNotCallUpdateWhenGameObjectInactive()
    {
        var gameObject = new GameObject();
        var behaviour = gameObject.AddComponent<CountingBehaviour>();
        gameObject.SetActive(false);

        RuntimeLoop.Tick(0.1f);

        Assert.Equal(1, behaviour.AwakeCount);
        Assert.Equal(1, behaviour.StartCount);
        Assert.Equal(0, behaviour.UpdateCount);
    }

    [Fact]
    public void RuntimeLoopCanMoveTransformFromMonoBehaviour()
    {
        var gameObject = new GameObject("moving");
        gameObject.AddComponent<MovingBehaviour>();

        RuntimeLoop.Tick(0.25f);

        AssertVector3(new Vector3(0.25f, 1f, 0f), gameObject.transform.position);
    }

    [Fact]
    public void AddComponentAddsMeshFilter()
    {
        var gameObject = new GameObject("mesh-filter");

        var meshFilter = gameObject.AddComponent<MeshFilter>();

        Assert.Same(gameObject, meshFilter.gameObject);
        Assert.Same(meshFilter, gameObject.GetComponent<MeshFilter>());
    }

    [Fact]
    public void MeshFilterSharedMeshAcceptsBoxMesh()
    {
        var meshFilter = new GameObject("mesh").AddComponent<MeshFilter>();
        var mesh = Mesh.CreateBox();

        meshFilter.sharedMesh = mesh;

        Assert.Same(mesh, meshFilter.sharedMesh);
        Assert.Same(mesh, meshFilter.mesh);
        Assert.Equal(Mesh.PrimitiveKind.Box, meshFilter.sharedMesh.primitiveKind);
    }

    [Fact]
    public void MeshRendererMaterialAcceptsMaterial()
    {
        var meshRenderer = new GameObject("renderer").AddComponent<MeshRenderer>();
        var material = new Material
        {
            name = "test-material",
            color = Color.red
        };

        meshRenderer.material = material;

        Assert.Same(material, meshRenderer.material);
        Assert.Same(material, meshRenderer.sharedMaterial);
        Assert.Equal(Color.red.r, meshRenderer.material.color.r);
        Assert.Equal(Color.red.g, meshRenderer.material.color.g);
        Assert.Equal(Color.red.b, meshRenderer.material.color.b);
        Assert.Equal(Color.red.a, meshRenderer.material.color.a);
    }

    [Fact]
    public void RendererEnabledCanBeToggled()
    {
        var meshRenderer = new GameObject("renderer").AddComponent<MeshRenderer>();

        Assert.True(meshRenderer.enabled);

        meshRenderer.enabled = false;

        Assert.False(meshRenderer.enabled);
    }

    [Fact]
    public void AddComponentAddsCamera()
    {
        var gameObject = new GameObject("camera");

        var camera = gameObject.AddComponent<Camera>();

        Assert.Same(gameObject, camera.gameObject);
        Assert.Same(camera, gameObject.GetComponent<Camera>());
        Assert.True(camera.enabled);
        Assert.Equal(60f, camera.fieldOfView);
    }

    [Fact]
    public void FirstCameraBecomesMainCamera()
    {
        var firstCamera = new GameObject("first-camera").AddComponent<Camera>();
        var secondCamera = new GameObject("second-camera").AddComponent<Camera>();

        Assert.Same(firstCamera, Camera.main);
        Assert.NotSame(secondCamera, Camera.main);
    }

    [Fact]
    public void RuntimeLoopResetClearsMainCamera()
    {
        new GameObject("camera").AddComponent<Camera>();

        RuntimeLoop.Reset();

        Assert.Null(Camera.main);
    }

    [Fact]
    public void AddComponentAddsLight()
    {
        var gameObject = new GameObject("light");

        var light = gameObject.AddComponent<Light>();

        Assert.Same(gameObject, light.gameObject);
        Assert.Same(light, gameObject.GetComponent<Light>());
        Assert.True(light.enabled);
        Assert.Equal(LightType.Directional, light.type);
    }

    [Fact]
    public void LightPropertiesCanBeConfigured()
    {
        var light = new GameObject("light").AddComponent<Light>();

        light.type = LightType.Point;
        light.intensity = 2.5f;
        light.color = Color.blue;
        light.range = 42f;

        Assert.Equal(LightType.Point, light.type);
        Assert.Equal(2.5f, light.intensity);
        Assert.Equal(Color.blue.r, light.color.r);
        Assert.Equal(Color.blue.g, light.color.g);
        Assert.Equal(Color.blue.b, light.color.b);
        Assert.Equal(Color.blue.a, light.color.a);
        Assert.Equal(42f, light.range);
    }

    [Fact]
    public void QuaternionEulerReturnsNonIdentityForNonZeroRotation()
    {
        var rotation = Quaternion.Euler(0f, 90f, 0f);

        Assert.NotEqual(Quaternion.identity.y, rotation.y);
        Assert.NotEqual(Quaternion.identity.w, rotation.w);
    }

    [Fact]
    public void QuaternionRotatesForwardVector()
    {
        var rotated = Quaternion.Euler(0f, 90f, 0f) * Vector3.forward;

        AssertVector3Approx(Vector3.right, rotated);
    }

    [Fact]
    public void QuaternionMultiplicationCombinesRotations()
    {
        var rotation = Quaternion.Euler(0f, 90f, 0f) * Quaternion.Euler(0f, -90f, 0f);

        AssertVector3Approx(Vector3.forward, rotation * Vector3.forward);
    }

    [Fact]
    public void TransformForwardChangesAfterEulerAngles()
    {
        var transform = new GameObject("rotated").transform;

        transform.eulerAngles = new Vector3(0f, 90f, 0f);

        AssertVector3Approx(Vector3.right, transform.forward);
    }

    [Fact]
    public void TransformLookAtPointsForwardAtTarget()
    {
        var transform = new GameObject("observer").transform;
        transform.position = Vector3.zero;

        transform.LookAt(new Vector3(0f, 0f, 5f));

        AssertVector3Approx(Vector3.forward, transform.forward);
    }

    [Fact]
    public void Color32KeepsBytesAndConvertsToColor()
    {
        var color32 = new Color32(10, 20, 30, 40);

        Color color = color32;

        Assert.Equal(10, color32.r);
        Assert.Equal(20, color32.g);
        Assert.Equal(30, color32.b);
        Assert.Equal(40, color32.a);
        Assert.Equal(10f / 255f, color.r, 0.0001f);
        Assert.Equal(20f / 255f, color.g, 0.0001f);
        Assert.Equal(30f / 255f, color.b, 0.0001f);
        Assert.Equal(40f / 255f, color.a, 0.0001f);
    }

    [Fact]
    public void ColorConvertsToColor32WithClampAndRounding()
    {
        Color32 color32 = new Color(1f, 0.5f, -1f, 2f);

        Assert.Equal(byte.MaxValue, color32.r);
        Assert.Equal(128, color32.g);
        Assert.Equal(0, color32.b);
        Assert.Equal(byte.MaxValue, color32.a);
    }

    [Fact]
    public void Vector2NormalizedWorks()
    {
        var value = new Vector2(3f, 4f);

        AssertVector2Approx(new Vector2(0.6f, 0.8f), value.normalized);
    }

    [Fact]
    public void Vector2DistanceAndDotWork()
    {
        Assert.Equal(5f, Vector2.Distance(new Vector2(0f, 0f), new Vector2(3f, 4f)));
        Assert.Equal(11f, Vector2.Dot(new Vector2(1f, 2f), new Vector2(3f, 4f)));
    }

    [Fact]
    public void MathfApproximatelyWorks()
    {
        Assert.True(Mathf.Approximately(1f, 1f + 0.0000001f));
        Assert.False(Mathf.Approximately(1f, 1.01f));
    }

    [Fact]
    public void MathfAtan2MatchesMathF()
    {
        Assert.Equal(MathF.Atan2(1f, 2f), Mathf.Atan2(1f, 2f));
    }

    [Fact]
    public void MathfRepeatWrapsIntoRange()
    {
        Assert.Equal(10f, Mathf.Repeat(370f, 360f));
        Assert.Equal(350f, Mathf.Repeat(-10f, 360f));
    }

    [Fact]
    public void MathfDeltaAngleReturnsShortestSignedAngle()
    {
        Assert.Equal(20f, Mathf.DeltaAngle(350f, 10f));
        Assert.Equal(-20f, Mathf.DeltaAngle(10f, 350f));
    }

    [Fact]
    public void Vector3IndexerReadsAndWritesComponents()
    {
        var value = Vector3.zero;

        value[0] = 1f;
        value[1] = 2f;
        value[2] = 3f;

        Assert.Equal(1f, value[0]);
        Assert.Equal(2f, value[1]);
        Assert.Equal(3f, value[2]);
    }

    [Fact]
    public void QuaternionIndexerReadsAndWritesComponents()
    {
        var value = Quaternion.identity;

        value[0] = 1f;
        value[1] = 2f;
        value[2] = 3f;
        value[3] = 4f;

        Assert.Equal(1f, value[0]);
        Assert.Equal(2f, value[1]);
        Assert.Equal(3f, value[2]);
        Assert.Equal(4f, value[3]);
    }

    private static void AssertVector3(Vector3 expected, Vector3 actual)
    {
        Assert.Equal(expected.x, actual.x);
        Assert.Equal(expected.y, actual.y);
        Assert.Equal(expected.z, actual.z);
    }

    private static void AssertVector3Approx(Vector3 expected, Vector3 actual, float tolerance = 0.001f)
    {
        Assert.True(MathF.Abs(expected.x - actual.x) <= tolerance, $"Expected x {expected.x}, got {actual.x}");
        Assert.True(MathF.Abs(expected.y - actual.y) <= tolerance, $"Expected y {expected.y}, got {actual.y}");
        Assert.True(MathF.Abs(expected.z - actual.z) <= tolerance, $"Expected z {expected.z}, got {actual.z}");
    }

    private static void AssertVector2Approx(Vector2 expected, Vector2 actual, float tolerance = 0.001f)
    {
        Assert.True(MathF.Abs(expected.x - actual.x) <= tolerance, $"Expected x {expected.x}, got {actual.x}");
        Assert.True(MathF.Abs(expected.y - actual.y) <= tolerance, $"Expected y {expected.y}, got {actual.y}");
    }

    private sealed class TestComponent : Component
    {
    }

    private sealed class CountingBehaviour : MonoBehaviour
    {
        public int AwakeCount { get; private set; }

        public int StartCount { get; private set; }

        public int UpdateCount { get; private set; }

        public int FixedUpdateCount { get; private set; }

        public int LateUpdateCount { get; private set; }

        public override void Awake()
        {
            AwakeCount++;
        }

        public override void Start()
        {
            StartCount++;
        }

        public override void Update()
        {
            UpdateCount++;
        }

        public override void FixedUpdate()
        {
            FixedUpdateCount++;
        }

        public override void LateUpdate()
        {
            LateUpdateCount++;
        }
    }

    private sealed class MovingBehaviour : MonoBehaviour
    {
        public override void Update()
        {
            transform.position = new Vector3(Time.time, 1f, 0f);
        }
    }
}
