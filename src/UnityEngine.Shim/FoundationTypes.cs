namespace UnityEngine;

public struct Bounds
{
    private Vector3 _center;
    private Vector3 _extents;

    public Bounds(Vector3 center, Vector3 size)
    {
        _center = center;
        _extents = size * 0.5f;
    }

    public Vector3 center
    {
        readonly get => _center;
        set => _center = value;
    }

    public Vector3 size
    {
        readonly get => _extents * 2f;
        set => _extents = value * 0.5f;
    }

    public Vector3 extents
    {
        readonly get => _extents;
        set => _extents = value;
    }

    public readonly Vector3 min => _center - _extents;

    public readonly Vector3 max => _center + _extents;

    public readonly bool Contains(Vector3 point)
    {
        var currentMin = min;
        var currentMax = max;
        return point.x >= currentMin.x && point.x <= currentMax.x
            && point.y >= currentMin.y && point.y <= currentMax.y
            && point.z >= currentMin.z && point.z <= currentMax.z;
    }

    public void Encapsulate(Vector3 point)
    {
        SetMinMax(Vector3.Min(min, point), Vector3.Max(max, point));
    }

    public readonly bool Intersects(Bounds bounds)
    {
        var currentMin = min;
        var currentMax = max;
        var otherMin = bounds.min;
        var otherMax = bounds.max;
        return currentMin.x <= otherMax.x && currentMax.x >= otherMin.x
            && currentMin.y <= otherMax.y && currentMax.y >= otherMin.y
            && currentMin.z <= otherMax.z && currentMax.z >= otherMin.z;
    }

    public void Expand(float amount)
    {
        _extents += Vector3.one * (amount * 0.5f);
    }

    public readonly Vector3 ClosestPoint(Vector3 point)
    {
        var currentMin = min;
        var currentMax = max;
        return new Vector3(
            Mathf.Clamp(point.x, currentMin.x, currentMax.x),
            Mathf.Clamp(point.y, currentMin.y, currentMax.y),
            Mathf.Clamp(point.z, currentMin.z, currentMax.z));
    }

    public void SetMinMax(Vector3 min, Vector3 max)
    {
        _center = (min + max) * 0.5f;
        _extents = (max - min) * 0.5f;
    }
}

public struct Ray
{
    public Vector3 origin;
    public Vector3 direction;

    public Ray(Vector3 origin, Vector3 direction)
    {
        this.origin = origin;
        this.direction = direction.normalized;
    }

    public readonly Vector3 GetPoint(float distance)
    {
        return origin + (direction * distance);
    }
}

public struct RaycastHit
{
    public Vector3 point { get; set; }

    public Vector3 normal { get; set; }

    public float distance { get; set; }

    public Collider? collider { get; set; }

    public readonly Transform? transform => collider?.transform;

    public readonly Rigidbody? rigidbody => collider?.gameObject.GetComponent<Rigidbody>();
}

public class Collider : Component
{
}

public class Rigidbody : Component
{
}

public struct LayerMask
{
    public int value;

    public static implicit operator int(LayerMask mask)
    {
        return mask.value;
    }

    public static implicit operator LayerMask(int value)
    {
        return new LayerMask { value = value };
    }

    public static int GetMask(params string[] layerNames)
    {
        return 0;
    }

    public static int NameToLayer(string layerName)
    {
        return -1;
    }

    public static string LayerToName(int layer)
    {
        return string.Empty;
    }
}

public struct Plane
{
    public Vector3 normal;
    public float distance;

    public Plane(Vector3 normal, float distance)
    {
        this.normal = normal.normalized;
        this.distance = distance;
    }

    public Plane(Vector3 normal, Vector3 point)
    {
        this.normal = normal.normalized;
        distance = -Vector3.Dot(this.normal, point);
    }

    public Plane(Vector3 a, Vector3 b, Vector3 c)
    {
        normal = Vector3.Cross(b - a, c - a).normalized;
        distance = -Vector3.Dot(normal, a);
    }

    public readonly float GetDistanceToPoint(Vector3 point)
    {
        return Vector3.Dot(normal, point) + distance;
    }

    public readonly bool GetSide(Vector3 point)
    {
        return GetDistanceToPoint(point) > 0f;
    }

    public readonly bool Raycast(Ray ray, out float enter)
    {
        var denominator = Vector3.Dot(normal, ray.direction);
        if (MathF.Abs(denominator) < 1E-06f)
        {
            enter = 0f;
            return false;
        }

        enter = -(Vector3.Dot(normal, ray.origin) + distance) / denominator;
        return enter >= 0f;
    }
}

public struct Matrix4x4
{
    public float m00;
    public float m01;
    public float m02;
    public float m03;
    public float m10;
    public float m11;
    public float m12;
    public float m13;
    public float m20;
    public float m21;
    public float m22;
    public float m23;
    public float m30;
    public float m31;
    public float m32;
    public float m33;

    public static Matrix4x4 identity => new()
    {
        m00 = 1f,
        m11 = 1f,
        m22 = 1f,
        m33 = 1f
    };

    public static Matrix4x4 zero => new();

    public float this[int row, int column]
    {
        readonly get
        {
            return (row, column) switch
            {
                (0, 0) => m00,
                (0, 1) => m01,
                (0, 2) => m02,
                (0, 3) => m03,
                (1, 0) => m10,
                (1, 1) => m11,
                (1, 2) => m12,
                (1, 3) => m13,
                (2, 0) => m20,
                (2, 1) => m21,
                (2, 2) => m22,
                (2, 3) => m23,
                (3, 0) => m30,
                (3, 1) => m31,
                (3, 2) => m32,
                (3, 3) => m33,
                _ => throw new IndexOutOfRangeException("Invalid Matrix4x4 index.")
            };
        }
        set
        {
            switch (row, column)
            {
                case (0, 0):
                    m00 = value;
                    break;
                case (0, 1):
                    m01 = value;
                    break;
                case (0, 2):
                    m02 = value;
                    break;
                case (0, 3):
                    m03 = value;
                    break;
                case (1, 0):
                    m10 = value;
                    break;
                case (1, 1):
                    m11 = value;
                    break;
                case (1, 2):
                    m12 = value;
                    break;
                case (1, 3):
                    m13 = value;
                    break;
                case (2, 0):
                    m20 = value;
                    break;
                case (2, 1):
                    m21 = value;
                    break;
                case (2, 2):
                    m22 = value;
                    break;
                case (2, 3):
                    m23 = value;
                    break;
                case (3, 0):
                    m30 = value;
                    break;
                case (3, 1):
                    m31 = value;
                    break;
                case (3, 2):
                    m32 = value;
                    break;
                case (3, 3):
                    m33 = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Matrix4x4 index.");
            }
        }
    }

    public readonly Vector3 MultiplyPoint(Vector3 point)
    {
        var x = (m00 * point.x) + (m01 * point.y) + (m02 * point.z) + m03;
        var y = (m10 * point.x) + (m11 * point.y) + (m12 * point.z) + m13;
        var z = (m20 * point.x) + (m21 * point.y) + (m22 * point.z) + m23;
        var w = (m30 * point.x) + (m31 * point.y) + (m32 * point.z) + m33;
        return MathF.Abs(w) > 1E-06f && MathF.Abs(w - 1f) > 1E-06f
            ? new Vector3(x / w, y / w, z / w)
            : new Vector3(x, y, z);
    }

    public readonly Vector3 MultiplyPoint3x4(Vector3 point)
    {
        return new Vector3(
            (m00 * point.x) + (m01 * point.y) + (m02 * point.z) + m03,
            (m10 * point.x) + (m11 * point.y) + (m12 * point.z) + m13,
            (m20 * point.x) + (m21 * point.y) + (m22 * point.z) + m23);
    }

    public readonly Vector3 MultiplyVector(Vector3 vector)
    {
        return new Vector3(
            (m00 * vector.x) + (m01 * vector.y) + (m02 * vector.z),
            (m10 * vector.x) + (m11 * vector.y) + (m12 * vector.z),
            (m20 * vector.x) + (m21 * vector.y) + (m22 * vector.z));
    }

    public static Matrix4x4 Translate(Vector3 vector)
    {
        var matrix = identity;
        matrix.m03 = vector.x;
        matrix.m13 = vector.y;
        matrix.m23 = vector.z;
        return matrix;
    }

    public static Matrix4x4 Scale(Vector3 vector)
    {
        var matrix = identity;
        matrix.m00 = vector.x;
        matrix.m11 = vector.y;
        matrix.m22 = vector.z;
        return matrix;
    }

    public static Matrix4x4 Rotate(Quaternion q)
    {
        var rotation = q.normalized;
        var xx = rotation.x * rotation.x;
        var yy = rotation.y * rotation.y;
        var zz = rotation.z * rotation.z;
        var xy = rotation.x * rotation.y;
        var xz = rotation.x * rotation.z;
        var yz = rotation.y * rotation.z;
        var wx = rotation.w * rotation.x;
        var wy = rotation.w * rotation.y;
        var wz = rotation.w * rotation.z;

        var matrix = identity;
        matrix.m00 = 1f - (2f * (yy + zz));
        matrix.m01 = 2f * (xy - wz);
        matrix.m02 = 2f * (xz + wy);
        matrix.m10 = 2f * (xy + wz);
        matrix.m11 = 1f - (2f * (xx + zz));
        matrix.m12 = 2f * (yz - wx);
        matrix.m20 = 2f * (xz - wy);
        matrix.m21 = 2f * (yz + wx);
        matrix.m22 = 1f - (2f * (xx + yy));
        return matrix;
    }

    public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
    {
        return Translate(pos) * Rotate(q) * Scale(s);
    }

    public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
    {
        var result = zero;
        for (var row = 0; row < 4; row++)
        {
            for (var column = 0; column < 4; column++)
            {
                var value = 0f;
                for (var index = 0; index < 4; index++)
                {
                    value += left[row, index] * right[index, column];
                }

                result[row, column] = value;
            }
        }

        return result;
    }
}
