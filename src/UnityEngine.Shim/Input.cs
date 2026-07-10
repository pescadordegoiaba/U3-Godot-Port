namespace UnityEngine;

public enum KeyCode
{
    None = 0,
    W,
    A,
    S,
    D,
    Space,
    LeftShift,
    LeftControl,
    Escape,
    Mouse0,
    Mouse1,
    Mouse2,
    UpArrow,
    DownArrow,
    LeftArrow,
    RightArrow,
    E,
    Q,
    R,
    F,
    Tab,
    Return,
    Alpha1,
    Alpha2,
    Alpha3,
    Alpha4,
    Alpha5,
    Alpha6,
    Alpha7,
    Alpha8,
    Alpha9,
    LeftAlt,
    RightAlt,
    RightShift,
    RightControl
}

public enum CursorLockMode
{
    None,
    Locked,
    Confined
}

public interface IInputBackend
{
    bool GetKey(KeyCode key);

    bool GetKeyDown(KeyCode key);

    bool GetKeyUp(KeyCode key);

    float GetAxis(string axisName);

    float GetAxisRaw(string axisName);

    bool GetButton(string buttonName);

    bool GetButtonDown(string buttonName);

    bool GetButtonUp(string buttonName);

    Vector3 mousePosition { get; }

    Vector2 mouseDelta { get; }

    Vector2 mouseScrollDelta { get; }

    bool anyKey { get; }

    bool anyKeyDown { get; }

    CursorLockMode cursorLockState { get; set; }

    bool cursorVisible { get; set; }

    void ResetInputAxes();
}

public static class Input
{
    private static IInputBackend? _backend;

    public static Vector3 mousePosition => _backend?.mousePosition ?? Vector3.zero;

    public static Vector2 mouseDelta => _backend?.mouseDelta ?? Vector2.zero;

    public static Vector2 mouseScrollDelta => _backend?.mouseScrollDelta ?? Vector2.zero;

    public static bool anyKey => _backend?.anyKey ?? false;

    public static bool anyKeyDown => _backend?.anyKeyDown ?? false;

    public static void SetBackend(IInputBackend backend)
    {
        _backend = backend;
        backend.cursorLockState = Cursor.lockState;
        backend.cursorVisible = Cursor.visible;
        InputBackendState.Backend = backend;
    }

    public static void ResetBackend()
    {
        _backend = null;
        InputBackendState.Backend = null;
    }

    public static bool GetKey(KeyCode key)
    {
        return _backend?.GetKey(key) ?? false;
    }

    public static bool GetKeyDown(KeyCode key)
    {
        return _backend?.GetKeyDown(key) ?? false;
    }

    public static bool GetKeyUp(KeyCode key)
    {
        return _backend?.GetKeyUp(key) ?? false;
    }

    public static float GetAxis(string axisName)
    {
        return _backend?.GetAxis(axisName) ?? 0f;
    }

    public static float GetAxisRaw(string axisName)
    {
        return _backend?.GetAxisRaw(axisName) ?? 0f;
    }

    public static bool GetButton(string buttonName)
    {
        return _backend?.GetButton(buttonName) ?? false;
    }

    public static bool GetButtonDown(string buttonName)
    {
        return _backend?.GetButtonDown(buttonName) ?? false;
    }

    public static bool GetButtonUp(string buttonName)
    {
        return _backend?.GetButtonUp(buttonName) ?? false;
    }

    public static void ResetInputAxes()
    {
        _backend?.ResetInputAxes();
    }
}

public static class Cursor
{
    private static CursorLockMode _lockState;
    private static bool _visible = true;

    public static CursorLockMode lockState
    {
        get => InputBackendState.Backend?.cursorLockState ?? _lockState;
        set
        {
            _lockState = value;
            if (InputBackendState.Backend is not null)
            {
                InputBackendState.Backend.cursorLockState = value;
            }
        }
    }

    public static bool visible
    {
        get => InputBackendState.Backend?.cursorVisible ?? _visible;
        set
        {
            _visible = value;
            if (InputBackendState.Backend is not null)
            {
                InputBackendState.Backend.cursorVisible = value;
            }
        }
    }
}

internal static class InputBackendState
{
    public static IInputBackend? Backend { get; set; }
}
