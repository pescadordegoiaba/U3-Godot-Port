using Godot;
using UnityEngine;
using UnityVector2 = UnityEngine.Vector2;
using UnityVector3 = UnityEngine.Vector3;

namespace U3.GodotBridge;

public sealed class GodotInputBridge : IInputBackend
{
    private readonly Dictionary<KeyCode, bool> _currentKeys = new();
    private readonly Dictionary<KeyCode, bool> _previousKeys = new();

    public UnityVector3 mousePosition
    {
        get
        {
            var position = Godot.Input.GetLastMouseVelocity();
            return new UnityVector3(position.X, position.Y, 0f);
        }
    }

    public UnityVector2 mouseScrollDelta => UnityVector2.zero;

    public bool anyKey => _currentKeys.Values.Any(value => value);

    public bool anyKeyDown => _currentKeys.Any(pair => pair.Value && !_previousKeys.GetValueOrDefault(pair.Key));

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
            _ => false
        };
    }
}
