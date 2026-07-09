namespace UnityEngine;

public static class RuntimeLoop
{
    private static readonly List<MonoBehaviour> Behaviours = new();
    private static readonly HashSet<MonoBehaviour> AwakenedBehaviours = new();
    private static readonly HashSet<MonoBehaviour> StartedBehaviours = new();

    public static void Tick()
    {
        Tick(Time.deltaTime);
    }

    public static void Tick(float deltaTime)
    {
        Time.deltaTime = deltaTime;
        Time.time += deltaTime;
        Time.frameCount++;

        foreach (var behaviour in SnapshotBehaviours())
        {
            EnsureAwake(behaviour);
        }

        foreach (var behaviour in SnapshotBehaviours())
        {
            EnsureStart(behaviour);
        }

        foreach (var behaviour in SnapshotBehaviours().Where(CanRunBehaviour))
        {
            behaviour.Update();
        }

        foreach (var behaviour in SnapshotBehaviours().Where(CanRunBehaviour))
        {
            behaviour.LateUpdate();
        }
    }

    public static void TickFixed(float? fixedDeltaTime = null)
    {
        if (fixedDeltaTime.HasValue)
        {
            Time.fixedDeltaTime = fixedDeltaTime.Value;
        }

        foreach (var behaviour in SnapshotBehaviours())
        {
            EnsureAwake(behaviour);
        }

        foreach (var behaviour in SnapshotBehaviours())
        {
            EnsureStart(behaviour);
        }

        foreach (var behaviour in SnapshotBehaviours().Where(CanRunBehaviour))
        {
            behaviour.FixedUpdate();
        }
    }

    internal static void Register(MonoBehaviour behaviour)
    {
        if (!Behaviours.Contains(behaviour))
        {
            Behaviours.Add(behaviour);
        }
    }

    internal static void Unregister(MonoBehaviour behaviour)
    {
        Behaviours.Remove(behaviour);
        AwakenedBehaviours.Remove(behaviour);
        StartedBehaviours.Remove(behaviour);
    }

    public static void Reset()
    {
        Behaviours.Clear();
        AwakenedBehaviours.Clear();
        StartedBehaviours.Clear();
        Time.deltaTime = 0f;
        Time.fixedDeltaTime = 0.02f;
        Time.time = 0f;
        Time.frameCount = 0;
        GameObject.ResetRegistryForTests();
    }

    private static MonoBehaviour[] SnapshotBehaviours()
    {
        return Behaviours.ToArray();
    }

    private static void EnsureAwake(MonoBehaviour behaviour)
    {
        if (AwakenedBehaviours.Add(behaviour))
        {
            behaviour.Awake();
        }
    }

    private static void EnsureStart(MonoBehaviour behaviour)
    {
        if (StartedBehaviours.Add(behaviour))
        {
            behaviour.Start();
        }
    }

    private static bool CanRunBehaviour(MonoBehaviour behaviour)
    {
        return behaviour.enabled && behaviour.gameObject.activeInHierarchy;
    }
}

public static class Time
{
    public static float deltaTime { get; set; }

    public static float fixedDeltaTime { get; set; } = 0.02f;

    public static float time { get; set; }

    public static int frameCount { get; set; }
}

public interface IUnityLogger
{
    void Log(object? message);

    void LogWarning(object? message);

    void LogError(object? message);
}

public sealed class ConsoleUnityLogger : IUnityLogger
{
    public void Log(object? message)
    {
        Console.WriteLine(message);
    }

    public void LogWarning(object? message)
    {
        Console.WriteLine(message);
    }

    public void LogError(object? message)
    {
        Console.Error.WriteLine(message);
    }
}

public static class Debug
{
    private static readonly IUnityLogger DefaultLogger = new ConsoleUnityLogger();
    private static IUnityLogger _logger = DefaultLogger;

    public static void SetLogger(IUnityLogger? logger)
    {
        _logger = logger ?? DefaultLogger;
    }

    public static void ResetLogger()
    {
        _logger = DefaultLogger;
    }

    public static void Log(object? message)
    {
        _logger.Log(message);
    }

    public static void LogWarning(object? message)
    {
        _logger.LogWarning(message);
    }

    public static void LogError(object? message)
    {
        _logger.LogError(message);
    }
}
