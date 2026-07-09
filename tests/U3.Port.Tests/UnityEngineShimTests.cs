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

    private static void AssertVector3(Vector3 expected, Vector3 actual)
    {
        Assert.Equal(expected.x, actual.x);
        Assert.Equal(expected.y, actual.y);
        Assert.Equal(expected.z, actual.z);
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
