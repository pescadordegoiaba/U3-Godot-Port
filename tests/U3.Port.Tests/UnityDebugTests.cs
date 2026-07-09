using UnityEngine;
using Xunit;

namespace U3.Port.Tests;

public sealed class UnityDebugTests
{
    public UnityDebugTests()
    {
        Debug.ResetLogger();
    }

    [Fact]
    public void DefaultLoggerDoesNotThrow()
    {
        Debug.ResetLogger();

        Debug.Log("default log");
        Debug.LogWarning("default warning");
        Debug.LogError("default error");
    }

    [Fact]
    public void SetLoggerCapturesLogWarningAndError()
    {
        var logger = new CapturingLogger();

        Debug.SetLogger(logger);

        Debug.Log("log");
        Debug.LogWarning("warning");
        Debug.LogError("error");

        Assert.Equal(["log"], logger.Logs);
        Assert.Equal(["warning"], logger.Warnings);
        Assert.Equal(["error"], logger.Errors);
    }

    [Fact]
    public void ResetLoggerRestoresDefaultLoggerWithoutException()
    {
        var logger = new CapturingLogger();
        Debug.SetLogger(logger);

        Debug.ResetLogger();

        Debug.Log("after reset");
        Debug.LogWarning("after reset warning");
        Debug.LogError("after reset error");

        Assert.Empty(logger.Logs);
        Assert.Empty(logger.Warnings);
        Assert.Empty(logger.Errors);
    }

    private sealed class CapturingLogger : IUnityLogger
    {
        public List<string> Logs { get; } = new();

        public List<string> Warnings { get; } = new();

        public List<string> Errors { get; } = new();

        public void Log(object? message)
        {
            Logs.Add(Format(message));
        }

        public void LogWarning(object? message)
        {
            Warnings.Add(Format(message));
        }

        public void LogError(object? message)
        {
            Errors.Add(Format(message));
        }

        private static string Format(object? message)
        {
            return message?.ToString() ?? "null";
        }
    }
}
