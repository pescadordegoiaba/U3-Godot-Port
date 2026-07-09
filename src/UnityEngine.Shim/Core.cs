namespace UnityEngine;

public class Object
{
    public string name { get; set; } = string.Empty;
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
    private readonly List<Component> _components = new();

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
    }

    public bool activeSelf { get; private set; }

    public Transform transform { get; }

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
    }
}

public class Transform : Component
{
    public Vector3 position { get; set; }

    public Vector3 localPosition { get; set; }

    public Quaternion rotation { get; set; } = Quaternion.identity;

    public Quaternion localRotation { get; set; } = Quaternion.identity;

    public Vector3 localScale { get; set; } = Vector3.one;

    public Transform? parent { get; set; }
}

public class Coroutine
{
}
