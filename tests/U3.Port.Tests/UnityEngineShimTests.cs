using UnityEngine;
using Xunit;

namespace U3.Port.Tests;

public class UnityEngineShimTests
{
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

    private static void AssertVector3(Vector3 expected, Vector3 actual)
    {
        Assert.Equal(expected.x, actual.x);
        Assert.Equal(expected.y, actual.y);
        Assert.Equal(expected.z, actual.z);
    }

    private sealed class TestComponent : Component
    {
    }
}
