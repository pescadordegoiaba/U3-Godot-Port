namespace UnityEngine;

public static class Random
{
    private static readonly System.Random Generator = new(12345);
    private static readonly object SyncRoot = new();

    public static Vector2 insideUnitCircle
    {
        get
        {
            lock (SyncRoot)
            {
                var angle = Generator.NextDouble() * MathF.PI * 2.0;
                var radius = MathF.Sqrt((float)Generator.NextDouble());
                return new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius);
            }
        }
    }
}
