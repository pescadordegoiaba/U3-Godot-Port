namespace UnityEngine.Serialization;

[AttributeUsage(AttributeTargets.Field)]
public sealed class FormerlySerializedAsAttribute : Attribute
{
    public FormerlySerializedAsAttribute(string oldName)
    {
        this.oldName = oldName;
    }

    public string oldName { get; }
}
