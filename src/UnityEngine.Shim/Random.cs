namespace UnityEngine;

public static class Random
{
    private static System.Random _generator = new(12345);
    private static readonly object SyncRoot = new();

    public static float value
    {
        get
        {
            lock (SyncRoot)
            {
                return (float)_generator.NextDouble();
            }
        }
    }

    public static Vector2 insideUnitCircle
    {
        get
        {
            lock (SyncRoot)
            {
                var angle = _generator.NextDouble() * MathF.PI * 2.0;
                var radius = MathF.Sqrt((float)_generator.NextDouble());
                return new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius);
            }
        }
    }

    public static Vector3 insideUnitSphere
    {
        get
        {
            lock (SyncRoot)
            {
                while (true)
                {
                    var point = new Vector3(
                        RangeUnlocked(-1f, 1f),
                        RangeUnlocked(-1f, 1f),
                        RangeUnlocked(-1f, 1f));
                    if (point.sqrMagnitude <= 1f)
                    {
                        return point;
                    }
                }
            }
        }
    }

    public static Vector3 onUnitSphere
    {
        get
        {
            lock (SyncRoot)
            {
                var z = RangeUnlocked(-1f, 1f);
                var angle = RangeUnlocked(0f, Mathf.PI * 2f);
                var radius = MathF.Sqrt(1f - (z * z));
                return new Vector3(
                    radius * MathF.Cos(angle),
                    radius * MathF.Sin(angle),
                    z);
            }
        }
    }

    public static Quaternion rotation => Quaternion.LookRotation(onUnitSphere);

    public static void InitState(int seed)
    {
        lock (SyncRoot)
        {
            _generator = new System.Random(seed);
        }
    }

    public static int Range(int minInclusive, int maxExclusive)
    {
        lock (SyncRoot)
        {
            return minInclusive == maxExclusive
                ? minInclusive
                : _generator.Next(minInclusive, maxExclusive);
        }
    }

    public static float Range(float minInclusive, float maxInclusive)
    {
        lock (SyncRoot)
        {
            return RangeUnlocked(minInclusive, maxInclusive);
        }
    }

    private static float RangeUnlocked(float minInclusive, float maxInclusive)
    {
        return minInclusive + ((float)_generator.NextDouble() * (maxInclusive - minInclusive));
    }
}
