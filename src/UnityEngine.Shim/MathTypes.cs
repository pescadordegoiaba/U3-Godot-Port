namespace UnityEngine;

public struct Vector2
{
    public float x;
    public float y;

    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2 zero => new(0f, 0f);

    public static Vector2 one => new(1f, 1f);

    public static Vector2 operator +(Vector2 left, Vector2 right)
    {
        return new Vector2(left.x + right.x, left.y + right.y);
    }

    public static Vector2 operator -(Vector2 left, Vector2 right)
    {
        return new Vector2(left.x - right.x, left.y - right.y);
    }

    public static Vector2 operator -(Vector2 value)
    {
        return new Vector2(-value.x, -value.y);
    }

    public static Vector2 operator *(Vector2 value, float scalar)
    {
        return new Vector2(value.x * scalar, value.y * scalar);
    }

    public static Vector2 operator *(float scalar, Vector2 value)
    {
        return value * scalar;
    }

    public static Vector2 operator /(Vector2 value, float scalar)
    {
        return new Vector2(value.x / scalar, value.y / scalar);
    }
}

public struct Vector3
{
    public float x;
    public float y;
    public float z;

    public Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vector3 zero => new(0f, 0f, 0f);

    public static Vector3 one => new(1f, 1f, 1f);

    public static Vector3 up => new(0f, 1f, 0f);

    public static Vector3 down => new(0f, -1f, 0f);

    public static Vector3 right => new(1f, 0f, 0f);

    public static Vector3 left => new(-1f, 0f, 0f);

    public static Vector3 forward => new(0f, 0f, 1f);

    public static Vector3 back => new(0f, 0f, -1f);

    public readonly float magnitude => MathF.Sqrt((x * x) + (y * y) + (z * z));

    public readonly Vector3 normalized
    {
        get
        {
            var currentMagnitude = magnitude;
            return currentMagnitude > 0f ? this / currentMagnitude : zero;
        }
    }

    public static Vector3 operator +(Vector3 left, Vector3 right)
    {
        return new Vector3(left.x + right.x, left.y + right.y, left.z + right.z);
    }

    public static Vector3 operator -(Vector3 left, Vector3 right)
    {
        return new Vector3(left.x - right.x, left.y - right.y, left.z - right.z);
    }

    public static Vector3 operator -(Vector3 value)
    {
        return new Vector3(-value.x, -value.y, -value.z);
    }

    public static Vector3 operator *(Vector3 value, float scalar)
    {
        return new Vector3(value.x * scalar, value.y * scalar, value.z * scalar);
    }

    public static Vector3 operator *(float scalar, Vector3 value)
    {
        return value * scalar;
    }

    public static Vector3 operator /(Vector3 value, float scalar)
    {
        return new Vector3(value.x / scalar, value.y / scalar, value.z / scalar);
    }

    public static float Distance(Vector3 left, Vector3 right)
    {
        return (left - right).magnitude;
    }

    public static float Dot(Vector3 left, Vector3 right)
    {
        return (left.x * right.x) + (left.y * right.y) + (left.z * right.z);
    }

    public static Vector3 Cross(Vector3 left, Vector3 right)
    {
        return new Vector3(
            (left.y * right.z) - (left.z * right.y),
            (left.z * right.x) - (left.x * right.z),
            (left.x * right.y) - (left.y * right.x));
    }
}

public struct Quaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public Quaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public static Quaternion identity => new(0f, 0f, 0f, 1f);
}

public struct Color
{
    public float r;
    public float g;
    public float b;
    public float a;

    public Color(float r, float g, float b, float a = 1f)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static Color white => new(1f, 1f, 1f, 1f);

    public static Color black => new(0f, 0f, 0f, 1f);

    public static Color red => new(1f, 0f, 0f, 1f);

    public static Color green => new(0f, 1f, 0f, 1f);

    public static Color blue => new(0f, 0f, 1f, 1f);

    public static Color clear => new(0f, 0f, 0f, 0f);
}

public static class Mathf
{
    public const float PI = MathF.PI;
    public const float Deg2Rad = PI / 180f;
    public const float Rad2Deg = 180f / PI;

    public static float Clamp(float value, float min, float max)
    {
        return Math.Min(Math.Max(value, min), max);
    }

    public static int Clamp(int value, int min, int max)
    {
        return Math.Min(Math.Max(value, min), max);
    }

    public static float Clamp01(float value)
    {
        return Clamp(value, 0f, 1f);
    }

    public static float Lerp(float from, float to, float t)
    {
        return from + ((to - from) * Clamp01(t));
    }

    public static float Abs(float value)
    {
        return MathF.Abs(value);
    }

    public static int Abs(int value)
    {
        return Math.Abs(value);
    }

    public static float Min(float left, float right)
    {
        return MathF.Min(left, right);
    }

    public static int Min(int left, int right)
    {
        return Math.Min(left, right);
    }

    public static float Max(float left, float right)
    {
        return MathF.Max(left, right);
    }

    public static int Max(int left, int right)
    {
        return Math.Max(left, right);
    }

    public static float Sqrt(float value)
    {
        return MathF.Sqrt(value);
    }

    public static float Sin(float value)
    {
        return MathF.Sin(value);
    }

    public static float Cos(float value)
    {
        return MathF.Cos(value);
    }

    public static float Tan(float value)
    {
        return MathF.Tan(value);
    }

    public static int FloorToInt(float value)
    {
        return (int)MathF.Floor(value);
    }

    public static int CeilToInt(float value)
    {
        return (int)MathF.Ceiling(value);
    }

    public static int RoundToInt(float value)
    {
        return (int)MathF.Round(value);
    }
}
