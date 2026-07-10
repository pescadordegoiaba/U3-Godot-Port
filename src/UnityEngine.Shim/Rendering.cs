namespace UnityEngine;

public class Mesh : Object
{
    public enum PrimitiveKind
    {
        Box,
        Sphere
    }

    public Mesh()
        : this(PrimitiveKind.Box)
    {
    }

    public Mesh(PrimitiveKind primitiveKind)
    {
        this.primitiveKind = primitiveKind;
    }

    public PrimitiveKind primitiveKind { get; }

    public Bounds bounds { get; set; } = new(Vector3.zero, Vector3.zero);

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
    private Material[] _materials = { new() };

    public bool enabled { get; set; } = true;

    public Material sharedMaterial
    {
        get => _materials.FirstOrDefault() ?? new Material();
        set => _materials = new[] { value };
    }

    public Material material
    {
        get => sharedMaterial;
        set => sharedMaterial = value;
    }

    public Material[] sharedMaterials
    {
        get => _materials.ToArray();
        set => _materials = value ?? Array.Empty<Material>();
    }

    public Material[] materials
    {
        get => sharedMaterials;
        set => sharedMaterials = value;
    }
}

public class MeshRenderer : Renderer
{
}

public class Material : Object
{
    public Color color { get; set; } = Color.white;
}

public class Texture : Object
{
}

public class Texture2D : Texture
{
    public Texture2D()
    {
    }

    public Texture2D(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public int width { get; }

    public int height { get; }
}

public class Sprite : Object
{
}

public class Shader : Object
{
    public static Shader Find(string name)
    {
        return new Shader { name = name };
    }
}

public class SkinnedMeshRenderer : Renderer
{
    public Mesh? sharedMesh { get; set; }
}

public class LineRenderer : Renderer
{
    public int positionCount { get; set; }

    public float startWidth { get; set; }

    public float endWidth { get; set; }

    public void SetPosition(int index, Vector3 position)
    {
    }
}

public class ParticleSystem : Component
{
    public bool isPlaying { get; private set; }

    public void Play()
    {
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
    }
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

public class Animator : Behaviour
{
    private readonly HashSet<string> _triggers = new(StringComparer.Ordinal);
    private readonly Dictionary<string, bool> _bools = new(StringComparer.Ordinal);
    private readonly Dictionary<string, int> _ints = new(StringComparer.Ordinal);
    private readonly Dictionary<string, float> _floats = new(StringComparer.Ordinal);

    public RuntimeAnimatorController? runtimeAnimatorController { get; set; }

    public void SetBool(string name, bool value) => _bools[name] = value;

    public bool GetBool(string name) => _bools.GetValueOrDefault(name);

    public void SetInteger(string name, int value) => _ints[name] = value;

    public int GetInteger(string name) => _ints.GetValueOrDefault(name);

    public void SetFloat(string name, float value) => _floats[name] = value;

    public float GetFloat(string name) => _floats.GetValueOrDefault(name);

    public void SetTrigger(string name) => _triggers.Add(name);

    public void ResetTrigger(string name) => _triggers.Remove(name);

    public bool HasTrigger(string name) => _triggers.Contains(name);

    public void Play(string stateName)
    {
    }
}

public class Animation : Behaviour
{
}

public class AnimationClip : Object
{
}

public class RuntimeAnimatorController : Object
{
}

public class AudioClip : Object
{
}

public class AudioSource : Behaviour
{
    public AudioClip? clip { get; set; }

    public float volume { get; set; } = 1f;

    public float pitch { get; set; } = 1f;

    public bool loop { get; set; }

    public bool isPlaying { get; private set; }

    public void Play()
    {
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
    }

    public void Pause()
    {
        isPlaying = false;
    }
}
