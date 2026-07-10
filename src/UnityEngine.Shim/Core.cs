using System.Collections;

namespace UnityEngine;

public class Object
{
    internal bool IsDestroyed { get; private set; }

    public string name { get; set; } = string.Empty;

    public static void Destroy(Object? obj)
    {
        DestroyImmediate(obj);
    }

    public static void Destroy(Object? obj, float t)
    {
        DestroyImmediate(obj);
    }

    public static void DestroyImmediate(Object? obj)
    {
        switch (obj)
        {
            case GameObject gameObject:
                gameObject.DestroyInternal();
                break;
            case Component component:
                component.gameObject.RemoveComponentInternal(component);
                break;
            case not null:
                obj.MarkDestroyed();
                break;
        }
    }

    internal void MarkDestroyed()
    {
        IsDestroyed = true;
    }
}

public class Component : Object
{
    public GameObject gameObject { get; internal set; } = null!;

    public Transform transform => gameObject.transform;

    public T? GetComponent<T>()
    {
        return gameObject.GetComponent<T>();
    }

    public bool TryGetComponent<T>(out T? component)
    {
        return gameObject.TryGetComponent(out component);
    }

    public T? GetComponentInChildren<T>(bool includeInactive = false)
    {
        return gameObject.GetComponentInChildren<T>(includeInactive);
    }

    public T? GetComponentInParent<T>(bool includeInactive = false)
    {
        return gameObject.GetComponentInParent<T>(includeInactive);
    }

    public T[] GetComponents<T>()
    {
        return gameObject.GetComponents<T>();
    }

    public T[] GetComponentsInChildren<T>(bool includeInactive = false)
    {
        return gameObject.GetComponentsInChildren<T>(includeInactive);
    }

    public void GetComponentsInChildren<T>(bool includeInactive, List<T> results)
    {
        gameObject.GetComponentsInChildren(includeInactive, results);
    }

    public T[] GetComponentsInParent<T>(bool includeInactive = false)
    {
        return gameObject.GetComponentsInParent<T>(includeInactive);
    }
}

public class Behaviour : Component
{
    public bool enabled { get; set; } = true;
}

public class MonoBehaviour : Behaviour
{
    public virtual void Awake() { }

    public virtual void Start() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void LateUpdate() { }

    public virtual void OnEnable() { }

    public virtual void OnDisable() { }

    public virtual void OnDestroy() { }
}

public class GameObject : Object
{
    private static readonly List<GameObject> Registry = new();
    private readonly List<Component> _components = new();

    public GameObject()
        : this(string.Empty)
    {
    }

    public GameObject(string name)
    {
        this.name = name;
        tag = "Untagged";
        activeSelf = true;

        transform = new Transform();
        transform.name = name;
        AddExistingComponent(transform);
        Registry.Add(this);
    }

    public bool activeSelf { get; private set; }

    public bool activeInHierarchy
    {
        get
        {
            if (!activeSelf || IsDestroyed)
            {
                return false;
            }

            var currentParent = transform.parent;
            while (currentParent is not null)
            {
                if (!currentParent.gameObject.activeSelf || currentParent.gameObject.IsDestroyed)
                {
                    return false;
                }

                currentParent = currentParent.parent;
            }

            return true;
        }
    }

    public Transform transform { get; }

    public string tag { get; set; }

    public int layer { get; set; }

    public static IEnumerable<GameObject> AllObjects => Registry.Where(gameObject => !gameObject.IsDestroyed);

    public static GameObject? Find(string name)
    {
        return AllObjects.FirstOrDefault(gameObject => gameObject.name == name);
    }

    public static GameObject? FindGameObjectWithTag(string tag)
    {
        return AllObjects.FirstOrDefault(gameObject => gameObject.CompareTag(tag));
    }

    public static GameObject[] FindGameObjectsWithTag(string tag)
    {
        return AllObjects.Where(gameObject => gameObject.CompareTag(tag)).ToArray();
    }

    public bool CompareTag(string tag)
    {
        return string.Equals(this.tag, tag, StringComparison.Ordinal);
    }

    public void SetActive(bool value)
    {
        activeSelf = value;
    }

    public T AddComponent<T>()
        where T : Component
    {
        if (typeof(T) == typeof(Transform))
        {
            return (T)(Component)transform;
        }

        var component = (T)Activator.CreateInstance(typeof(T))!;
        AddExistingComponent(component);
        return component;
    }

    public T? GetComponent<T>()
    {
        return _components.Where(component => !component.IsDestroyed).OfType<T>().FirstOrDefault();
    }

    public bool TryGetComponent<T>(out T? component)
    {
        component = GetComponent<T>();
        return component is not null;
    }

    public T[] GetComponents<T>()
    {
        return _components.Where(component => !component.IsDestroyed).OfType<T>().ToArray();
    }

    public void GetComponents<T>(List<T> results)
    {
        results.AddRange(GetComponents<T>());
    }

    public T? GetComponentInChildren<T>(bool includeInactive = false)
    {
        return GetComponentsInChildren<T>(includeInactive).FirstOrDefault();
    }

    public T[] GetComponentsInChildren<T>(bool includeInactive = false)
    {
        var results = new List<T>();
        CollectComponentsInChildren(transform, includeInactive, results);
        return results.ToArray();
    }

    public void GetComponentsInChildren<T>(List<T> results)
    {
        GetComponentsInChildren(includeInactive: false, results);
    }

    public void GetComponentsInChildren<T>(bool includeInactive, List<T> results)
    {
        CollectComponentsInChildren(transform, includeInactive, results);
    }

    public T? GetComponentInParent<T>(bool includeInactive = false)
    {
        return GetComponentsInParent<T>(includeInactive).FirstOrDefault();
    }

    public T[] GetComponentsInParent<T>(bool includeInactive = false)
    {
        var results = new List<T>();
        var current = transform;
        while (current is not null)
        {
            if (includeInactive || current.gameObject.activeInHierarchy)
            {
                results.AddRange(current.gameObject.GetComponents<T>());
            }

            current = current.parent;
        }

        return results.ToArray();
    }

    internal void AddExistingComponent(Component component)
    {
        component.gameObject = this;
        _components.Add(component);

        if (component is Camera camera)
        {
            Camera.SetMainIfMissing(camera);
        }

        if (component is Collider collider)
        {
            collider.attachedRigidbody = GetComponent<Rigidbody>();
        }

        if (component is MonoBehaviour monoBehaviour)
        {
            RuntimeLoop.Register(monoBehaviour);
        }
    }

    internal void RemoveComponentInternal(Component component)
    {
        if (ReferenceEquals(component, transform) || component.IsDestroyed)
        {
            return;
        }

        component.MarkDestroyed();
        _components.Remove(component);

        if (component is MonoBehaviour monoBehaviour)
        {
            RuntimeLoop.Unregister(monoBehaviour);
            monoBehaviour.OnDestroy();
        }

        if (component is Camera camera)
        {
            Camera.ClearMainIfSame(camera);
        }
    }

    internal void DestroyInternal()
    {
        if (IsDestroyed)
        {
            return;
        }

        foreach (var child in transform.children.ToArray())
        {
            child.gameObject.DestroyInternal();
        }

        foreach (var component in _components.ToArray())
        {
            component.MarkDestroyed();

            if (component is MonoBehaviour monoBehaviour)
            {
                RuntimeLoop.Unregister(monoBehaviour);
                monoBehaviour.OnDestroy();
            }

            if (component is Camera camera)
            {
                Camera.ClearMainIfSame(camera);
            }
        }

        _components.Clear();
        transform.SetParent(null);
        MarkDestroyed();
        Registry.Remove(this);
    }

    internal static void ResetRegistryForTests()
    {
        Registry.Clear();
        Camera.ResetMain();
    }

    private static void CollectComponentsInChildren<T>(Transform transform, bool includeInactive, List<T> results)
    {
        if (includeInactive || transform.gameObject.activeInHierarchy)
        {
            results.AddRange(transform.gameObject.GetComponents<T>());
        }

        foreach (var child in transform.children)
        {
            CollectComponentsInChildren(child, includeInactive, results);
        }
    }
}

public class Transform : Component, IEnumerable<Transform>
{
    private readonly List<Transform> _children = new();
    private Transform? _parent;
    private Vector3 _localPosition;
    private Quaternion _localRotation = Quaternion.identity;

    // World/local composition is intentionally simplified for this shim layer:
    // position mirrors localPosition and rotation mirrors localRotation.
    public Vector3 position
    {
        get => _localPosition;
        set => _localPosition = value;
    }

    public Vector3 localPosition
    {
        get => _localPosition;
        set => _localPosition = value;
    }

    public Quaternion rotation
    {
        get => _localRotation;
        set => _localRotation = value.normalized;
    }

    public Quaternion localRotation
    {
        get => _localRotation;
        set => _localRotation = value.normalized;
    }

    public Vector3 eulerAngles
    {
        get => rotation.eulerAngles;
        set => rotation = Quaternion.Euler(value);
    }

    public Vector3 localEulerAngles
    {
        get => localRotation.eulerAngles;
        set => localRotation = Quaternion.Euler(value);
    }

    public Vector3 localScale { get; set; } = Vector3.one;

    public Vector3 forward => rotation * Vector3.forward;

    public Vector3 up => rotation * Vector3.up;

    public Vector3 right => rotation * Vector3.right;

    public Transform? parent
    {
        get => _parent;
        set => SetParent(value);
    }

    public int childCount => _children.Count;

    public IEnumerable<Transform> children => _children;

    public Transform root
    {
        get
        {
            var current = this;
            while (current.parent is not null)
            {
                current = current.parent;
            }

            return current;
        }
    }

    public Transform GetChild(int index)
    {
        return _children[index];
    }

    public void SetParent(Transform? parent)
    {
        if (ReferenceEquals(_parent, parent))
        {
            return;
        }

        if (parent is not null && CreatesCycle(parent))
        {
            throw new InvalidOperationException("Cannot set a Transform parent to itself or one of its children.");
        }

        _parent?._children.Remove(this);
        _parent = parent;
        _parent?._children.Add(this);
    }

    public Transform? Find(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        var parts = name.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return Find(parts, 0);
    }

    public void DetachChildren()
    {
        foreach (var child in _children.ToArray())
        {
            child.SetParent(null);
        }
    }

    public int GetSiblingIndex()
    {
        return parent is null ? 0 : parent._children.IndexOf(this);
    }

    public void SetSiblingIndex(int index)
    {
        if (parent is null)
        {
            return;
        }

        parent._children.Remove(this);
        var clampedIndex = Math.Clamp(index, 0, parent._children.Count);
        parent._children.Insert(clampedIndex, this);
    }

    public new T? GetComponent<T>()
    {
        return gameObject.GetComponent<T>();
    }

    public new T[] GetComponentsInChildren<T>(bool includeInactive = false)
    {
        return gameObject.GetComponentsInChildren<T>(includeInactive);
    }

    public new void GetComponentsInChildren<T>(bool includeInactive, List<T> results)
    {
        gameObject.GetComponentsInChildren(includeInactive, results);
    }

    public new T? GetComponentInParent<T>(bool includeInactive = false)
    {
        return gameObject.GetComponentInParent<T>(includeInactive);
    }

    public bool CompareTag(string tag)
    {
        return gameObject.CompareTag(tag);
    }

    public Vector3 TransformPoint(Vector3 point)
    {
        return position + (rotation * Vector3.Scale(point, localScale));
    }

    public Vector3 InverseTransformPoint(Vector3 point)
    {
        var unrotated = Quaternion.Inverse(rotation) * (point - position);
        return new Vector3(
            localScale.x == 0f ? unrotated.x : unrotated.x / localScale.x,
            localScale.y == 0f ? unrotated.y : unrotated.y / localScale.y,
            localScale.z == 0f ? unrotated.z : unrotated.z / localScale.z);
    }

    public Vector3 TransformDirection(Vector3 direction)
    {
        return rotation * direction;
    }

    public Vector3 InverseTransformDirection(Vector3 direction)
    {
        return Quaternion.Inverse(rotation) * direction;
    }

    public void LookAt(Vector3 worldPosition)
    {
        var direction = worldPosition - position;
        rotation = Quaternion.LookRotation(direction);
    }

    public void LookAt(Transform target)
    {
        LookAt(target.position);
    }

    public IEnumerator<Transform> GetEnumerator()
    {
        return _children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private Transform? Find(string[] parts, int index)
    {
        if (index >= parts.Length)
        {
            return this;
        }

        foreach (var child in _children)
        {
            if (child.name == parts[index])
            {
                return child.Find(parts, index + 1);
            }
        }

        return null;
    }

    private bool CreatesCycle(Transform candidateParent)
    {
        Transform? current = candidateParent;
        while (current is not null)
        {
            if (ReferenceEquals(current, this))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}

public class RectTransform : Transform
{
    public Vector2 anchoredPosition { get; set; }

    public Vector2 sizeDelta { get; set; }

    public Vector2 anchorMin { get; set; }

    public Vector2 anchorMax { get; set; } = Vector2.one;

    public Vector2 pivot { get; set; } = new(0.5f, 0.5f);
}

public class Coroutine
{
}
