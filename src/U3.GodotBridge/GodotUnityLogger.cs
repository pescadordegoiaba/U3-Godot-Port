using Godot;
using UnityEngine;

namespace U3.GodotBridge;

public sealed class GodotUnityLogger : IUnityLogger
{
    public void Log(object? message)
    {
        GD.Print(Format(message));
    }

    public void LogWarning(object? message)
    {
        GD.PushWarning(Format(message));
    }

    public void LogError(object? message)
    {
        GD.PushError(Format(message));
    }

    private static string Format(object? message)
    {
        return message?.ToString() ?? "null";
    }
}
