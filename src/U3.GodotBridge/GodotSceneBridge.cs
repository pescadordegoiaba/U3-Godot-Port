using Godot;
using UnityEngine;

namespace U3.GodotBridge;

public sealed class GodotSceneBridge
{
    private readonly Dictionary<GameObject, Node3D> _nodesByGameObject = new();

    public Node3D CreateNode(GameObject gameObject, Node parent)
    {
        if (_nodesByGameObject.TryGetValue(gameObject, out var existingNode))
        {
            return existingNode;
        }

        var node = new Node3D
        {
            Name = string.IsNullOrWhiteSpace(gameObject.name) ? "GameObject" : gameObject.name
        };

        parent.AddChild(node);
        _nodesByGameObject.Add(gameObject, node);
        Sync(gameObject, node);
        return node;
    }

    public Node3D GetNode(GameObject gameObject)
    {
        return _nodesByGameObject[gameObject];
    }

    public bool TryGetNode(GameObject gameObject, out Node3D? node)
    {
        return _nodesByGameObject.TryGetValue(gameObject, out node);
    }

    public void SyncAll()
    {
        foreach (var pair in _nodesByGameObject)
        {
            Sync(pair.Key, pair.Value);
        }
    }

    public void Clear()
    {
        foreach (var node in _nodesByGameObject.Values)
        {
            node.QueueFree();
        }

        _nodesByGameObject.Clear();
    }

    private static void Sync(GameObject gameObject, Node3D node)
    {
        var transform = gameObject.transform;
        node.Position = ToGodot(transform.position);
        node.Scale = ToGodot(transform.localScale);
        node.Quaternion = ToGodot(transform.localRotation);
        node.Visible = gameObject.activeInHierarchy;
    }

    private static Godot.Vector3 ToGodot(UnityEngine.Vector3 value)
    {
        return new Godot.Vector3(value.x, value.y, value.z);
    }

    private static Godot.Quaternion ToGodot(UnityEngine.Quaternion value)
    {
        return new Godot.Quaternion(value.x, value.y, value.z, value.w);
    }
}
