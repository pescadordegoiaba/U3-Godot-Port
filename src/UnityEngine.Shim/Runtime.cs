namespace UnityEngine;

public static class Time
{
    public static float deltaTime { get; set; }

    public static float fixedDeltaTime { get; set; } = 0.02f;

    public static float time { get; set; }

    public static int frameCount { get; set; }
}

public static class Debug
{
    public static void Log(object? message)
    {
        Console.WriteLine(message);
    }

    public static void LogWarning(object? message)
    {
        Console.WriteLine(message);
    }

    public static void LogError(object? message)
    {
        Console.Error.WriteLine(message);
    }
}
