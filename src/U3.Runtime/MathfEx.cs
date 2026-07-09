using UnityEngine;

namespace SDG.Unturned;

public static class MathfEx
{
    public static float Square(float x)
    {
        return x * x;
    }

    public static bool IsNearlyEqual(float a, float b, float tolerance = 0.01f)
    {
        return Mathf.Abs(b - a) < tolerance;
    }

    public static bool IsAngleDegreesNearlyEqual(float a, float b, float tolerance = 0.1f)
    {
        return Mathf.Abs(Mathf.DeltaAngle(a, b)) < tolerance;
    }

    public static bool IsNearlyZero(float x, float tolerance = 0.01f)
    {
        return Mathf.Abs(x) < tolerance;
    }

    public static float Min(float a, float b, float c)
    {
        return Mathf.Min(Mathf.Min(a, b), c);
    }

    public static float Max(float a, float b, float c)
    {
        return Mathf.Max(Mathf.Max(a, b), c);
    }
}
