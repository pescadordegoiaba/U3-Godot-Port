using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xunit;

namespace U3.Port.Tests;

public class UnityFakeRuntimeCompletionTests
{
    public UnityFakeRuntimeCompletionTests()
    {
        RuntimeLoop.Reset();
        Resources.ClearRegistered();
        PlayerPrefs.DeleteAll();
    }

    [Fact]
    public void ComponentQueriesDelegateToGameObject()
    {
        var gameObject = new GameObject("root");
        var first = gameObject.AddComponent<TestComponent>();
        var second = gameObject.AddComponent<TestComponent>();

        Assert.Same(first, gameObject.GetComponent<TestComponent>());
        Assert.Same(first, gameObject.transform.GetComponent<TestComponent>());
        Assert.Same(first, second.GetComponent<TestComponent>());
        Assert.Equal(2, gameObject.GetComponents<TestComponent>().Length);
    }

    [Fact]
    public void ComponentQueriesFindChildrenAndParents()
    {
        var parent = new GameObject("parent");
        var child = new GameObject("child");
        child.transform.SetParent(parent.transform);
        var parentComponent = parent.AddComponent<TestComponent>();
        var childComponent = child.AddComponent<ChildComponent>();

        Assert.Same(childComponent, parent.GetComponentInChildren<ChildComponent>());
        Assert.Same(parentComponent, child.GetComponentInParent<TestComponent>());
        Assert.Contains(childComponent, parent.GetComponentsInChildren<Component>());
        Assert.Contains(parentComponent, child.GetComponentsInParent<Component>());
    }

    [Fact]
    public void TransformFindSiblingAndDetachChildrenWork()
    {
        var root = new GameObject("root");
        var first = new GameObject("first");
        var second = new GameObject("second");
        var grandchild = new GameObject("grandchild");
        first.transform.SetParent(root.transform);
        second.transform.SetParent(root.transform);
        grandchild.transform.SetParent(first.transform);

        Assert.Same(grandchild.transform, root.transform.Find("first/grandchild"));

        second.transform.SetSiblingIndex(0);
        Assert.Equal(0, second.transform.GetSiblingIndex());
        Assert.Same(second.transform, root.transform.GetChild(0));

        root.transform.DetachChildren();
        Assert.Equal(0, root.transform.childCount);
        Assert.Null(first.transform.parent);
        Assert.Null(second.transform.parent);
    }

    [Fact]
    public void TagsLayersAndTagSearchWork()
    {
        var player = new GameObject("player") { tag = "Player", layer = 7 };
        var agent = new GameObject("agent") { tag = "Agent" };

        Assert.True(player.CompareTag("Player"));
        Assert.Equal(7, player.layer);
        Assert.Same(player, GameObject.FindGameObjectWithTag("Player"));
        Assert.Equal(new[] { agent }, GameObject.FindGameObjectsWithTag("Agent"));
    }

    [Fact]
    public void DestroyComponentRemovesOnlyComponent()
    {
        var gameObject = new GameObject("object");
        var component = gameObject.AddComponent<TestComponent>();

        UnityEngine.Object.Destroy(component);

        Assert.True(gameObject.activeInHierarchy);
        Assert.Null(gameObject.GetComponent<TestComponent>());
        Assert.NotNull(gameObject.transform);
    }

    [Fact]
    public void DestroyGameObjectRemovesFromRegistryAndRuntimeLoop()
    {
        var gameObject = new GameObject("object");
        var behaviour = gameObject.AddComponent<CountingBehaviour>();

        RuntimeLoop.Tick(0.1f);
        UnityEngine.Object.Destroy(gameObject);
        RuntimeLoop.Tick(0.1f);

        Assert.Null(GameObject.Find("object"));
        Assert.Equal(1, behaviour.updateCount);
        Assert.Equal(1, behaviour.destroyCount);
    }

    [Fact]
    public void RuntimeLoopIgnoresDestroyedComponents()
    {
        var gameObject = new GameObject("object");
        var behaviour = gameObject.AddComponent<CountingBehaviour>();

        RuntimeLoop.Tick(0.1f);
        UnityEngine.Object.Destroy(behaviour);
        RuntimeLoop.Tick(0.1f);

        Assert.Equal(1, behaviour.updateCount);
        Assert.Equal(1, behaviour.destroyCount);
    }

    [Fact]
    public void ScriptableObjectCreateInstanceWorks()
    {
        Assert.IsType<TestAsset>(ScriptableObject.CreateInstance<TestAsset>());
        Assert.IsType<TestAsset>(ScriptableObject.CreateInstance(typeof(TestAsset)));
    }

    [Fact]
    public void TextAssetStoresTextAndBytes()
    {
        var asset = new TextAsset("hello");

        Assert.Equal("hello", asset.text);
        Assert.Equal("hello", System.Text.Encoding.UTF8.GetString(asset.bytes));
    }

    [Fact]
    public void ResourcesRegisterLoadLoadAllAndClearWork()
    {
        var first = new TextAsset("a") { name = "first" };
        var second = new TextAsset("b") { name = "second" };

        Resources.Register("texts", first);
        Resources.Register("texts", second);

        Assert.Same(first, Resources.Load<TextAsset>("texts"));
        Assert.Equal(2, Resources.LoadAll<TextAsset>("texts").Length);

        Resources.ClearRegistered();

        Assert.Null(Resources.Load<TextAsset>("texts"));
    }

    [Fact]
    public void PlayerPrefsSetGetDeleteAndDefaultsWork()
    {
        PlayerPrefs.SetString("s", "value");
        PlayerPrefs.SetInt("i", 42);
        PlayerPrefs.SetFloat("f", 1.5f);

        Assert.True(PlayerPrefs.HasKey("s"));
        Assert.Equal("value", PlayerPrefs.GetString("s"));
        Assert.Equal(42, PlayerPrefs.GetInt("i"));
        Assert.Equal(1.5f, PlayerPrefs.GetFloat("f"));

        PlayerPrefs.DeleteKey("s");

        Assert.False(PlayerPrefs.HasKey("s"));
        Assert.Equal("fallback", PlayerPrefs.GetString("s", "fallback"));
    }

    [Fact]
    public void PhysicsStubsReturnNoHits()
    {
        Assert.False(Physics.Raycast(new Ray(Vector3.zero, Vector3.forward), out _));
        Assert.Empty(Physics.RaycastAll(new Ray(Vector3.zero, Vector3.forward)));
        Assert.Empty(Physics.OverlapSphere(Vector3.zero, 1f));
        Assert.False(Physics.CheckSphere(Vector3.zero, 1f));
    }

    [Fact]
    public void ColliderAndRigidbodyPropertiesWork()
    {
        var gameObject = new GameObject("physics");
        var box = gameObject.AddComponent<BoxCollider>();
        var sphere = gameObject.AddComponent<SphereCollider>();
        var capsule = gameObject.AddComponent<CapsuleCollider>();
        var rigidbody = gameObject.AddComponent<Rigidbody>();

        box.center = Vector3.one;
        box.size = new Vector3(2f, 3f, 4f);
        sphere.radius = 5f;
        capsule.height = 6f;
        rigidbody.MovePosition(new Vector3(7f, 8f, 9f));
        rigidbody.MoveRotation(Quaternion.Euler(0f, 90f, 0f));

        Assert.Equal(Vector3.one, box.center);
        Assert.Equal(new Vector3(2f, 3f, 4f), box.size);
        Assert.Equal(5f, sphere.radius);
        Assert.Equal(6f, capsule.height);
        Assert.Equal(new Vector3(7f, 8f, 9f), rigidbody.position);
        Assert.Equal(new Vector3(7f, 8f, 9f), gameObject.transform.position);
    }

    [Fact]
    public void SceneManagerFakeLoadsScenes()
    {
        var loaded = false;
        SceneManager.sceneLoaded += (_, _) => loaded = true;

        SceneManager.LoadScene("TestScene");

        var scene = SceneManager.GetActiveScene();
        Assert.True(scene.IsValid());
        Assert.Equal("TestScene", scene.name);
        Assert.True(scene.isLoaded);
        Assert.True(loaded);
        Assert.True(SceneManager.LoadSceneAsync("AsyncScene").isDone);
    }

    [Fact]
    public void AnimatorParametersAndTriggersWork()
    {
        var animator = new GameObject("animator").AddComponent<Animator>();

        animator.SetBool("alive", true);
        animator.SetInteger("state", 3);
        animator.SetFloat("speed", 4.5f);
        animator.SetTrigger("fire");

        Assert.True(animator.GetBool("alive"));
        Assert.Equal(3, animator.GetInteger("state"));
        Assert.Equal(4.5f, animator.GetFloat("speed"));
        Assert.True(animator.HasTrigger("fire"));

        animator.ResetTrigger("fire");
        Assert.False(animator.HasTrigger("fire"));
    }

    [Fact]
    public void AudioSourcePlayStopPauseWork()
    {
        var audio = new GameObject("audio").AddComponent<AudioSource>();

        audio.Play();
        Assert.True(audio.isPlaying);

        audio.Pause();
        Assert.False(audio.isPlaying);

        audio.Play();
        audio.Stop();
        Assert.False(audio.isPlaying);
    }

    [Fact]
    public void UnityEventAddInvokeAndRemoveAllWork()
    {
        var calls = 0;
        var unityEvent = new UnityEvent();

        unityEvent.AddListener(() => calls++);
        unityEvent.Invoke();
        unityEvent.RemoveAllListeners();
        unityEvent.Invoke();

        Assert.Equal(1, calls);
    }

    [Fact]
    public void ButtonOnClickInvokesListener()
    {
        var button = new GameObject("button").AddComponent<Button>();
        var clicked = false;

        button.onClick.AddListener(() => clicked = true);
        button.onClick.Invoke();

        Assert.True(clicked);
    }

    [Fact]
    public void TmpTextStoresText()
    {
        var text = new GameObject("tmp").AddComponent<TextMeshProUGUI>();

        text.text = "hello tmp";
        text.fontSize = 24f;

        Assert.Equal("hello tmp", text.text);
        Assert.Equal(24f, text.fontSize);
    }

    private sealed class TestComponent : Component
    {
    }

    private sealed class ChildComponent : Component
    {
    }

    private sealed class CountingBehaviour : MonoBehaviour
    {
        public int updateCount;
        public int destroyCount;

        public override void Update()
        {
            updateCount++;
        }

        public override void OnDestroy()
        {
            destroyCount++;
        }
    }

    private sealed class TestAsset : ScriptableObject
    {
    }
}
