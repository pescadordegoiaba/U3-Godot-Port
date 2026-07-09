namespace UnityEngine;

public class Mesh : Object
{
    public enum PrimitiveKind
    {
        Box,
        Sphere
    }

    public Mesh(PrimitiveKind primitiveKind)
    {
        this.primitiveKind = primitiveKind;
    }

    public PrimitiveKind primitiveKind { get; }

    public static Mesh CreateBox()
    {
        return new Mesh(PrimitiveKind.Box);
    }

    public static Mesh CreateSphere()
    {
        return new Mesh(PrimitiveKind.Sphere);
    }
}

public class MeshFilter : Component
{
    private Mesh? _mesh;

    public Mesh? sharedMesh
    {
        get => _mesh;
        set => _mesh = value;
    }

    public Mesh? mesh
    {
        get => _mesh;
        set => _mesh = value;
    }
}

public class Renderer : Component
{
    private Material _material = new();

    public bool enabled { get; set; } = true;

    public Material sharedMaterial
    {
        get => _material;
        set => _material = value;
    }

    public Material material
    {
        get => _material;
        set => _material = value;
    }
}

public class MeshRenderer : Renderer
{
}

public class Material : Object
{
    public Color color { get; set; } = Color.white;
}

public class Camera : Behaviour
{
    public float fieldOfView { get; set; } = 60f;

    public float nearClipPlane { get; set; } = 0.3f;

    public float farClipPlane { get; set; } = 1000f;

    public static Camera? main { get; private set; }

    internal static void SetMainIfMissing(Camera camera)
    {
        main ??= camera;
    }

    internal static void ClearMainIfSame(Camera camera)
    {
        if (ReferenceEquals(main, camera))
        {
            main = null;
        }
    }

    internal static void ResetMain()
    {
        main = null;
    }
}

public enum LightType
{
    Directional,
    Point,
    Spot
}

public class Light : Behaviour
{
    public LightType type { get; set; } = LightType.Directional;

    public float intensity { get; set; } = 1f;

    public Color color { get; set; } = Color.white;

    public float range { get; set; } = 10f;
}
