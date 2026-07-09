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
