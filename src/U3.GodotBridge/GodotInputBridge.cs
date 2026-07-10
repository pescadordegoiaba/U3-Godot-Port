using Godot;
using UnityEngine;
using UnityVector2 = UnityEngine.Vector2;
using UnityVector3 = UnityEngine.Vector3;

namespace U3.GodotBridge;

public sealed class GodotInputBridge : IInputBackend
{
    private readonly Dictionary<KeyCode, bool> _currentKeys = new();
    private readonly Dictionary<KeyCode, bool> _previousKeys = new();
    private UnityVector2 _mouseDelta;
    private UnityVector2 _mouseScrollDelta;
    private CursorLockMode _cursorLockState;
    private bool _cursorVisible = true;

    public UnityVector3 mousePosition
    {
        get
        {
            var position = Godot.Input.GetLastMouseVelocity();
            return new UnityVector3(position.X, position.Y, 0f);
        }
    }

    public UnityVector2 mouseDelta => _mouseDelta;

    public UnityVector2 mouseScrollDelta => _mouseScrollDelta;

    public bool anyKey => _currentKeys.Values.Any(value => value);

    public bool anyKeyDown => _currentKeys.Any(pair => pair.Value && !_previousKeys.GetValueOrDefault(pair.Key));

    public CursorLockMode cursorLockState
    {
        get => _cursorLockState;
        set
        {
            _cursorLockState = value;
            Godot.Input.MouseMode = value switch
            {
                CursorLockMode.Locked => Godot.Input.MouseModeEnum.Captured,
                CursorLockMode.Confined => Godot.Input.MouseModeEnum.Confined,
                _ => Godot.Input.MouseModeEnum.Visible
            };
        }
    }

    public bool cursorVisible
    {
        get => _cursorVisible;
        set
        {
            _cursorVisible = value;
            if (_cursorLockState == CursorLockMode.None)
            {
                Godot.Input.MouseMode = value ? Godot.Input.MouseModeEnum.Visible : Godot.Input.MouseModeEnum.Hidden;
            }
        }
    }

    public void HandleInputEvent(InputEvent inputEvent)
    {
        switch (inputEvent)
        {
            case InputEventMouseMotion motion:
                _mouseDelta += new UnityVector2(motion.Relative.X, motion.Relative.Y);
                break;
            case InputEventMouseButton { Pressed: true } button:
                if (button.ButtonIndex == MouseButton.WheelUp)
                {
                    _mouseScrollDelta += new UnityVector2(0f, 1f);
                }
                else if (button.ButtonIndex == MouseButton.WheelDown)
                {
                    _mouseScrollDelta += new UnityVector2(0f, -1f);
                }
                break;
        }
    }

    public void UpdateFrame()
    {
        _previousKeys.Clear();
        foreach (var pair in _currentKeys)
        {
            _previousKeys[pair.Key] = pair.Value;
        }

        foreach (var key in Enum.GetValues<KeyCode>())
        {
            if (key != KeyCode.None)
            {
                _currentKeys[key] = ReadKey(key);
            }
        }

        if (GetKeyDown(KeyCode.Escape))
        {
            cursorLockState = CursorLockMode.None;
            cursorVisible = true;
        }
    }

    public void EndFrame()
    {
        _mouseDelta = UnityVector2.zero;
        _mouseScrollDelta = UnityVector2.zero;
    }

    public bool GetKey(KeyCode key)
    {
        return _currentKeys.GetValueOrDefault(key);
    }

    public bool GetKeyDown(KeyCode key)
    {
        return _currentKeys.GetValueOrDefault(key) && !_previousKeys.GetValueOrDefault(key);
    }

    public bool GetKeyUp(KeyCode key)
    {
        return !_currentKeys.GetValueOrDefault(key) && _previousKeys.GetValueOrDefault(key);
    }

    public float GetAxis(string axisName)
    {
        return GetAxisRaw(axisName);
    }

    public float GetAxisRaw(string axisName)
    {
        return axisName switch
        {
            "Horizontal" => Axis(KeyCode.A, KeyCode.D) + Axis(KeyCode.LeftArrow, KeyCode.RightArrow),
            "Vertical" => Axis(KeyCode.S, KeyCode.W) + Axis(KeyCode.DownArrow, KeyCode.UpArrow),
            "Mouse X" => _mouseDelta.x,
            "Mouse Y" => _mouseDelta.y,
            _ => 0f
        };
    }

    public bool GetButton(string buttonName)
    {
        return GetButtonKey(buttonName, out var key) && GetKey(key);
    }

    public bool GetButtonDown(string buttonName)
    {
        return GetButtonKey(buttonName, out var key) && GetKeyDown(key);
    }

    public bool GetButtonUp(string buttonName)
    {
        return GetButtonKey(buttonName, out var key) && GetKeyUp(key);
    }

    public void ResetInputAxes()
    {
        _currentKeys.Clear();
        _previousKeys.Clear();
    }

    private float Axis(KeyCode negative, KeyCode positive)
    {
        var value = 0f;
        if (GetKey(negative))
        {
            value -= 1f;
        }

        if (GetKey(positive))
        {
            value += 1f;
        }

        return Math.Clamp(value, -1f, 1f);
    }

    private static bool GetButtonKey(string buttonName, out KeyCode key)
    {
        key = buttonName switch
        {
            "Jump" or "jump" => KeyCode.Space,
            "Sprint" or "sprint" => KeyCode.LeftShift,
            "Interact" or "interact" => KeyCode.E,
            "Fire1" or "fire" => KeyCode.Mouse0,
            "Fire2" or "aim" => KeyCode.Mouse1,
            _ => KeyCode.None
        };

        return key != KeyCode.None;
    }

    private static bool ReadKey(KeyCode key)
    {
        return key switch
        {
            KeyCode.W => Godot.Input.IsKeyPressed(Key.W) || Godot.Input.IsActionPressed("move_forward"),
            KeyCode.A => Godot.Input.IsKeyPressed(Key.A) || Godot.Input.IsActionPressed("move_left"),
            KeyCode.S => Godot.Input.IsKeyPressed(Key.S) || Godot.Input.IsActionPressed("move_back"),
            KeyCode.D => Godot.Input.IsKeyPressed(Key.D) || Godot.Input.IsActionPressed("move_right"),
            KeyCode.Space => Godot.Input.IsKeyPressed(Key.Space) || Godot.Input.IsActionPressed("jump"),
            KeyCode.LeftShift => Godot.Input.IsKeyPressed(Key.Shift) || Godot.Input.IsActionPressed("sprint"),
            KeyCode.LeftControl => Godot.Input.IsKeyPressed(Key.Ctrl),
            KeyCode.Escape => Godot.Input.IsKeyPressed(Key.Escape),
            KeyCode.UpArrow => Godot.Input.IsKeyPressed(Key.Up),
            KeyCode.DownArrow => Godot.Input.IsKeyPressed(Key.Down),
            KeyCode.LeftArrow => Godot.Input.IsKeyPressed(Key.Left),
            KeyCode.RightArrow => Godot.Input.IsKeyPressed(Key.Right),
            KeyCode.E => Godot.Input.IsKeyPressed(Key.E) || Godot.Input.IsActionPressed("interact"),
            KeyCode.Q => Godot.Input.IsKeyPressed(Key.Q),
            KeyCode.R => Godot.Input.IsKeyPressed(Key.R),
            KeyCode.F => Godot.Input.IsKeyPressed(Key.F),
            KeyCode.Mouse0 => Godot.Input.IsMouseButtonPressed(MouseButton.Left) || Godot.Input.IsActionPressed("fire"),
            KeyCode.Mouse1 => Godot.Input.IsMouseButtonPressed(MouseButton.Right) || Godot.Input.IsActionPressed("aim"),
            KeyCode.Mouse2 => Godot.Input.IsMouseButtonPressed(MouseButton.Middle),
            KeyCode.Tab => Godot.Input.IsKeyPressed(Key.Tab),
            KeyCode.Return => Godot.Input.IsKeyPressed(Key.Enter),
            KeyCode.Alpha1 => Godot.Input.IsKeyPressed(Key.Key1),
            KeyCode.Alpha2 => Godot.Input.IsKeyPressed(Key.Key2),
            KeyCode.Alpha3 => Godot.Input.IsKeyPressed(Key.Key3),
            KeyCode.Alpha4 => Godot.Input.IsKeyPressed(Key.Key4),
            KeyCode.Alpha5 => Godot.Input.IsKeyPressed(Key.Key5),
            KeyCode.Alpha6 => Godot.Input.IsKeyPressed(Key.Key6),
            KeyCode.Alpha7 => Godot.Input.IsKeyPressed(Key.Key7),
            KeyCode.Alpha8 => Godot.Input.IsKeyPressed(Key.Key8),
            KeyCode.Alpha9 => Godot.Input.IsKeyPressed(Key.Key9),
            KeyCode.LeftAlt => Godot.Input.IsKeyPressed(Key.Alt),
            KeyCode.RightAlt => Godot.Input.IsKeyPressed(Key.Alt),
            KeyCode.RightShift => Godot.Input.IsKeyPressed(Key.Shift),
            KeyCode.RightControl => Godot.Input.IsKeyPressed(Key.Ctrl),
            _ => false
        };
    }
}
