namespace UnityEngine;

public class Object
{
    public string name { get; set; } = string.Empty;

    public static void Destroy(Object? obj)
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
                component.gameObject.DestroyInternal();
                break;
        }
    }
}

public class Component : Object
{
    public GameObject gameObject { get; internal set; } = null!;

    public Transform transform => gameObject.transform;
}

public class Behaviour : Component
{
    public bool enabled { get; set; } = true;
}

public class MonoBehaviour : Behaviour
{
    public virtual void Awake()
    {
    }

    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void LateUpdate()
    {
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
    }

    public virtual void OnDestroy()
    {
    }
}

public class GameObject : Object
{
    private static readonly List<GameObject> Registry = new();
    private readonly List<Component> _components = new();
    private bool _destroyed;

    public GameObject()
        : this(string.Empty)
    {
    }

    public GameObject(string name)
    {
        this.name = name;
        activeSelf = true;

        transform = new Transform();
        AddExistingComponent(transform);
        Registry.Add(this);
    }

    public bool activeSelf { get; private set; }

    public bool activeInHierarchy
    {
        get
        {
            if (!activeSelf || _destroyed)
            {
                return false;
            }

            var currentParent = transform.parent;
            while (currentParent is not null)
            {
                if (!currentParent.gameObject.activeSelf)
                {
                    return false;
                }

                currentParent = currentParent.parent;
            }

            return true;
        }
    }

    public Transform transform { get; }

    public static IEnumerable<GameObject> AllObjects => Registry.Where(gameObject => !gameObject._destroyed);

    public static GameObject? Find(string name)
    {
        return AllObjects.FirstOrDefault(gameObject => gameObject.name == name);
    }

    public void SetActive(bool value)
    {
        activeSelf = value;
    }

    public T AddComponent<T>()
        where T : Component, new()
    {
        if (typeof(T) == typeof(Transform))
        {
            return (T)(Component)transform;
        }

        var component = new T();
        AddExistingComponent(component);
        return component;
    }

    public T? GetComponent<T>()
        where T : Component
    {
        foreach (var component in _components)
        {
            if (component is T typedComponent)
            {
                return typedComponent;
            }
        }

        return null;
    }

    public bool TryGetComponent<T>(out T? component)
        where T : Component
    {
        component = GetComponent<T>();
        return component is not null;
    }

    private void AddExistingComponent(Component component)
    {
        component.gameObject = this;
        _components.Add(component);

        if (component is MonoBehaviour monoBehaviour)
        {
            RuntimeLoop.Register(monoBehaviour);
        }
    }

    internal void DestroyInternal()
    {
        if (_destroyed)
        {
            return;
        }

        _destroyed = true;
        transform.SetParent(null);
        Registry.Remove(this);

        foreach (var component in _components.OfType<MonoBehaviour>())
        {
            RuntimeLoop.Unregister(component);
        }
    }

    internal static void ResetRegistryForTests()
    {
        Registry.Clear();
    }
}

public class Transform : Component
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
        set => _localRotation = value;
    }

    public Quaternion localRotation
    {
        get => _localRotation;
        set => _localRotation = value;
    }

    public Vector3 localScale { get; set; } = Vector3.one;

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

    private bool CreatesCycle(Transform candidateParent)
    {
        var current = candidateParent;
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

public class Coroutine
{
}
