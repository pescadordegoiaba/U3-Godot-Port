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

    public readonly float sqrMagnitude => (x * x) + (y * y);

    public readonly float magnitude => MathF.Sqrt(sqrMagnitude);

    public readonly Vector2 normalized
    {
        get
        {
            var currentMagnitude = magnitude;
            return currentMagnitude > 1E-06f ? this / currentMagnitude : zero;
        }
    }

    public float this[int index]
    {
        readonly get
        {
            return index switch
            {
                0 => x,
                1 => y,
                _ => throw new IndexOutOfRangeException("Invalid Vector2 index.")
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector2 index.");
            }
        }
    }

    public void Normalize()
    {
        var normalizedValue = normalized;
        x = normalizedValue.x;
        y = normalizedValue.y;
    }

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

    public static float Distance(Vector2 left, Vector2 right)
    {
        return (left - right).magnitude;
    }

    public static float Dot(Vector2 left, Vector2 right)
    {
        return (left.x * right.x) + (left.y * right.y);
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

    public readonly float sqrMagnitude => (x * x) + (y * y) + (z * z);

    public readonly float magnitude => MathF.Sqrt((x * x) + (y * y) + (z * z));

    public readonly Vector3 normalized
    {
        get
        {
            var currentMagnitude = magnitude;
            return currentMagnitude > 1E-06f ? this / currentMagnitude : zero;
        }
    }

    public float this[int index]
    {
        readonly get
        {
            return index switch
            {
                0 => x,
                1 => y,
                2 => z,
                _ => throw new IndexOutOfRangeException("Invalid Vector3 index.")
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Vector3 index.");
            }
        }
    }

    public void Normalize()
    {
        var normalizedValue = normalized;
        x = normalizedValue.x;
        y = normalizedValue.y;
        z = normalizedValue.z;
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

    public static float Angle(Vector3 from, Vector3 to)
    {
        var denominator = MathF.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
        if (denominator < 1E-06f)
        {
            return 0f;
        }

        var dot = Math.Clamp(Dot(from, to) / denominator, -1f, 1f);
        return MathF.Acos(dot) * Mathf.Rad2Deg;
    }

    public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
    {
        var denominator = Dot(planeNormal, planeNormal);
        if (denominator < 1E-06f)
        {
            return vector;
        }

        return vector - (planeNormal * (Dot(vector, planeNormal) / denominator));
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

    public readonly Quaternion normalized => Normalize(this);

    public float this[int index]
    {
        readonly get
        {
            return index switch
            {
                0 => x,
                1 => y,
                2 => z,
                3 => w,
                _ => throw new IndexOutOfRangeException("Invalid Quaternion index.")
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                case 3:
                    w = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Quaternion index.");
            }
        }
    }

    public readonly Vector3 eulerAngles
    {
        get
        {
            var q = normalized;

            var sinrCosp = 2f * ((q.w * q.x) + (q.y * q.z));
            var cosrCosp = 1f - (2f * ((q.x * q.x) + (q.y * q.y)));
            var xAngle = MathF.Atan2(sinrCosp, cosrCosp);

            var sinp = 2f * ((q.w * q.y) - (q.z * q.x));
            var yAngle = MathF.Abs(sinp) >= 1f
                ? MathF.CopySign(MathF.PI / 2f, sinp)
                : MathF.Asin(sinp);

            var sinyCosp = 2f * ((q.w * q.z) + (q.x * q.y));
            var cosyCosp = 1f - (2f * ((q.y * q.y) + (q.z * q.z)));
            var zAngle = MathF.Atan2(sinyCosp, cosyCosp);

            return new Vector3(
                NormalizeAngle(xAngle * Mathf.Rad2Deg),
                NormalizeAngle(yAngle * Mathf.Rad2Deg),
                NormalizeAngle(zAngle * Mathf.Rad2Deg));
        }
    }

    public static Quaternion Euler(float x, float y, float z)
    {
        var xRotation = AngleAxis(x, Vector3.right);
        var yRotation = AngleAxis(y, Vector3.up);
        var zRotation = AngleAxis(z, Vector3.forward);
        return (yRotation * xRotation * zRotation).normalized;
    }

    public static Quaternion Euler(Vector3 euler)
    {
        return Euler(euler.x, euler.y, euler.z);
    }

    public static Quaternion LookRotation(Vector3 forward)
    {
        return LookRotation(forward, Vector3.up);
    }

    public static Quaternion LookRotation(Vector3 forward, Vector3 upwards)
    {
        var normalizedForward = forward.normalized;
        if (normalizedForward.sqrMagnitude < 1E-06f)
        {
            normalizedForward = Vector3.forward;
        }

        var normalizedUp = upwards.normalized;
        if (normalizedUp.sqrMagnitude < 1E-06f)
        {
            normalizedUp = Vector3.up;
        }

        var right = Vector3.Cross(normalizedUp, normalizedForward).normalized;
        if (right.sqrMagnitude < 1E-06f)
        {
            right = Vector3.Cross(Vector3.up, normalizedForward).normalized;
            if (right.sqrMagnitude < 1E-06f)
            {
                right = Vector3.Cross(Vector3.right, normalizedForward).normalized;
            }
        }

        var up = Vector3.Cross(normalizedForward, right);
        return FromBasis(right, up, normalizedForward).normalized;
    }

    public static Quaternion Normalize(Quaternion value)
    {
        var magnitude = MathF.Sqrt(
            (value.x * value.x) +
            (value.y * value.y) +
            (value.z * value.z) +
            (value.w * value.w));

        return magnitude > 1E-06f
            ? new Quaternion(value.x / magnitude, value.y / magnitude, value.z / magnitude, value.w / magnitude)
            : identity;
    }

    public static Quaternion operator *(Quaternion left, Quaternion right)
    {
        return new Quaternion(
            (left.w * right.x) + (left.x * right.w) + (left.y * right.z) - (left.z * right.y),
            (left.w * right.y) - (left.x * right.z) + (left.y * right.w) + (left.z * right.x),
            (left.w * right.z) + (left.x * right.y) - (left.y * right.x) + (left.z * right.w),
            (left.w * right.w) - (left.x * right.x) - (left.y * right.y) - (left.z * right.z));
    }

    public static Vector3 operator *(Quaternion rotation, Vector3 point)
    {
        var q = rotation.normalized;
        var vector = new Vector3(q.x, q.y, q.z);
        var uv = Vector3.Cross(vector, point);
        var uuv = Vector3.Cross(vector, uv);
        return point + (uv * (2f * q.w)) + (uuv * 2f);
    }

    private static Quaternion AngleAxis(float degrees, Vector3 axis)
    {
        var normalizedAxis = axis.normalized;
        if (normalizedAxis.sqrMagnitude < 1E-06f)
        {
            return identity;
        }

        var radians = degrees * Mathf.Deg2Rad;
        var halfRadians = radians * 0.5f;
        var sin = MathF.Sin(halfRadians);
        return new Quaternion(
            normalizedAxis.x * sin,
            normalizedAxis.y * sin,
            normalizedAxis.z * sin,
            MathF.Cos(halfRadians));
    }

    private static Quaternion FromBasis(Vector3 right, Vector3 up, Vector3 forward)
    {
        var m00 = right.x;
        var m01 = up.x;
        var m02 = forward.x;
        var m10 = right.y;
        var m11 = up.y;
        var m12 = forward.y;
        var m20 = right.z;
        var m21 = up.z;
        var m22 = forward.z;
        var trace = m00 + m11 + m22;

        if (trace > 0f)
        {
            var s = MathF.Sqrt(trace + 1f) * 2f;
            return new Quaternion(
                (m21 - m12) / s,
                (m02 - m20) / s,
                (m10 - m01) / s,
                0.25f * s);
        }

        if (m00 > m11 && m00 > m22)
        {
            var s = MathF.Sqrt(1f + m00 - m11 - m22) * 2f;
            return new Quaternion(
                0.25f * s,
                (m01 + m10) / s,
                (m02 + m20) / s,
                (m21 - m12) / s);
        }

        if (m11 > m22)
        {
            var s = MathF.Sqrt(1f + m11 - m00 - m22) * 2f;
            return new Quaternion(
                (m01 + m10) / s,
                0.25f * s,
                (m12 + m21) / s,
                (m02 - m20) / s);
        }

        var finalS = MathF.Sqrt(1f + m22 - m00 - m11) * 2f;
        return new Quaternion(
            (m02 + m20) / finalS,
            (m12 + m21) / finalS,
            0.25f * finalS,
            (m10 - m01) / finalS);
    }

    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        return angle < 0f ? angle + 360f : angle;
    }
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

public struct Color32
{
    public byte r;
    public byte g;
    public byte b;
    public byte a;

    public Color32(byte r, byte g, byte b, byte a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public static implicit operator Color(Color32 value)
    {
        const float scale = 1f / byte.MaxValue;
        return new Color(value.r * scale, value.g * scale, value.b * scale, value.a * scale);
    }

    public static implicit operator Color32(Color value)
    {
        return new Color32(
            FloatToByte(value.r),
            FloatToByte(value.g),
            FloatToByte(value.b),
            FloatToByte(value.a));
    }

    private static byte FloatToByte(float value)
    {
        var clamped = Math.Clamp(value, 0f, 1f);
        return (byte)MathF.Round(clamped * byte.MaxValue);
    }
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

    public static float Atan2(float y, float x)
    {
        return MathF.Atan2(y, x);
    }

    public static bool Approximately(float left, float right)
    {
        return MathF.Abs(right - left) < MathF.Max(1E-06f * MathF.Max(MathF.Abs(left), MathF.Abs(right)), 1.121039E-44f);
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
