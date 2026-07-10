namespace UnityEngine.Events;

public delegate void UnityAction();

public delegate void UnityAction<in T0>(T0 arg0);

public delegate void UnityAction<in T0, in T1>(T0 arg0, T1 arg1);

public class UnityEvent
{
    private readonly List<UnityAction> _listeners = new();

    public void AddListener(UnityAction call)
    {
        _listeners.Add(call);
    }

    public void RemoveListener(UnityAction call)
    {
        _listeners.Remove(call);
    }

    public void RemoveAllListeners()
    {
        _listeners.Clear();
    }

    public void Invoke()
    {
        foreach (var listener in _listeners.ToArray())
        {
            listener();
        }
    }
}

public class UnityEvent<T0>
{
    private readonly List<UnityAction<T0>> _listeners = new();

    public void AddListener(UnityAction<T0> call)
    {
        _listeners.Add(call);
    }

    public void RemoveListener(UnityAction<T0> call)
    {
        _listeners.Remove(call);
    }

    public void RemoveAllListeners()
    {
        _listeners.Clear();
    }

    public void Invoke(T0 arg0)
    {
        foreach (var listener in _listeners.ToArray())
        {
            listener(arg0);
        }
    }
}

public class UnityEvent<T0, T1>
{
    private readonly List<UnityAction<T0, T1>> _listeners = new();

    public void AddListener(UnityAction<T0, T1> call)
    {
        _listeners.Add(call);
    }

    public void RemoveListener(UnityAction<T0, T1> call)
    {
        _listeners.Remove(call);
    }

    public void RemoveAllListeners()
    {
        _listeners.Clear();
    }

    public void Invoke(T0 arg0, T1 arg1)
    {
        foreach (var listener in _listeners.ToArray())
        {
            listener(arg0, arg1);
        }
    }
}
