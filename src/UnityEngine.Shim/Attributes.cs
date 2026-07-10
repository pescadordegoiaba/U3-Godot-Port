namespace UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public sealed class SerializeField : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class HideInInspector : Attribute
{
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class HeaderAttribute : Attribute
{
    public HeaderAttribute(string header)
    {
        this.header = header;
    }

    public string header { get; }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class TooltipAttribute : Attribute
{
    public TooltipAttribute(string tooltip)
    {
        this.tooltip = tooltip;
    }

    public string tooltip { get; }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class RangeAttribute : Attribute
{
    public RangeAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float min { get; }

    public float max { get; }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class MinAttribute : Attribute
{
    public MinAttribute(float min)
    {
        this.min = min;
    }

    public float min { get; }
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class SpaceAttribute : Attribute
{
    public SpaceAttribute(float height = 8f)
    {
        this.height = height;
    }

    public float height { get; }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ContextMenu : Attribute
{
    public ContextMenu(string itemName)
    {
        this.itemName = itemName;
    }

    public string itemName { get; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequireComponent : Attribute
{
    public RequireComponent(Type requiredComponent)
    {
        this.requiredComponent = requiredComponent;
    }

    public Type requiredComponent { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class DisallowMultipleComponent : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class ExecuteInEditMode : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class ExecuteAlways : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class AddComponentMenu : Attribute
{
    public AddComponentMenu(string menuName)
    {
        this.menuName = menuName;
    }

    public string menuName { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class CreateAssetMenuAttribute : Attribute
{
    public string fileName { get; set; } = string.Empty;

    public string menuName { get; set; } = string.Empty;

    public int order { get; set; }
}

public enum RuntimeInitializeLoadType
{
    AfterSceneLoad,
    BeforeSceneLoad,
    AfterAssembliesLoaded,
    BeforeSplashScreen,
    SubsystemRegistration
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class RuntimeInitializeOnLoadMethodAttribute : Attribute
{
    public RuntimeInitializeOnLoadMethodAttribute()
    {
    }

    public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType)
    {
        this.loadType = loadType;
    }

    public RuntimeInitializeLoadType loadType { get; }
}
