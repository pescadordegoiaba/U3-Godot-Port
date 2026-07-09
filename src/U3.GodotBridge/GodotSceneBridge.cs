using Godot;
using UnityEngine;

namespace U3.GodotBridge;

public sealed class GodotSceneBridge
{
    private readonly Dictionary<GameObject, Node3D> _nodesByGameObject = new();
    private readonly Dictionary<GameObject, Node> _rootNodesByGameObject = new();
    private readonly Dictionary<GameObject, MeshInstance3D> _meshInstancesByGameObject = new();
    private readonly HashSet<GameObject> _warnedMissingParentNodes = new();

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
        _rootNodesByGameObject.Add(gameObject, parent);
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
            SyncParent(pair.Key, pair.Value);
        }

        foreach (var pair in _nodesByGameObject)
        {
            Sync(pair.Key, pair.Value);
            SyncMesh(pair.Key, pair.Value);
        }
    }

    public void Clear()
    {
        foreach (var node in _nodesByGameObject.Values)
        {
            node.QueueFree();
        }

        _nodesByGameObject.Clear();
        _rootNodesByGameObject.Clear();
        _meshInstancesByGameObject.Clear();
        _warnedMissingParentNodes.Clear();
    }

    private void SyncParent(GameObject gameObject, Node3D node)
    {
        var transformParent = gameObject.transform.parent;
        var desiredParent = GetDesiredParent(gameObject, transformParent);

        if (desiredParent is null || ReferenceEquals(node.GetParent(), desiredParent))
        {
            return;
        }

        node.Reparent(desiredParent, keepGlobalTransform: false);
    }

    private Node? GetDesiredParent(GameObject gameObject, Transform? transformParent)
    {
        if (transformParent is null)
        {
            return _rootNodesByGameObject.GetValueOrDefault(gameObject);
        }

        var parentGameObject = transformParent.gameObject;
        if (_nodesByGameObject.TryGetValue(parentGameObject, out var parentNode))
        {
            _warnedMissingParentNodes.Remove(gameObject);
            return parentNode;
        }

        if (_warnedMissingParentNodes.Add(gameObject))
        {
            Debug.LogWarning($"No Godot node exists for parent GameObject '{parentGameObject.name}'. Keeping '{gameObject.name}' under bridge root.");
        }

        return _rootNodesByGameObject.GetValueOrDefault(gameObject);
    }

    private static void Sync(GameObject gameObject, Node3D node)
    {
        var transform = gameObject.transform;
        node.Position = ToGodot(transform.position);
        node.Scale = ToGodot(transform.localScale);
        node.Quaternion = ToGodot(transform.localRotation);
        node.Visible = gameObject.activeInHierarchy;
    }

    private void SyncMesh(GameObject gameObject, Node3D node)
    {
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        var unityMesh = meshFilter?.sharedMesh;

        if (meshFilter is null || meshRenderer is null || unityMesh is null)
        {
            return;
        }

        var meshInstance = GetOrCreateMeshInstance(gameObject, node);
        meshInstance.Mesh = CreateGodotMesh(unityMesh.primitiveKind);
        meshInstance.Visible = gameObject.activeInHierarchy && meshRenderer.enabled;
        meshInstance.MaterialOverride = CreateGodotMaterial(meshRenderer.sharedMaterial);
    }

    private MeshInstance3D GetOrCreateMeshInstance(GameObject gameObject, Node3D node)
    {
        if (_meshInstancesByGameObject.TryGetValue(gameObject, out var meshInstance))
        {
            if (!ReferenceEquals(meshInstance.GetParent(), node))
            {
                meshInstance.Reparent(node, keepGlobalTransform: false);
            }

            return meshInstance;
        }

        meshInstance = new MeshInstance3D
        {
            Name = $"{node.Name}Mesh"
        };

        node.AddChild(meshInstance);
        _meshInstancesByGameObject.Add(gameObject, meshInstance);
        return meshInstance;
    }

    private static Godot.Mesh CreateGodotMesh(UnityEngine.Mesh.PrimitiveKind primitiveKind)
    {
        return primitiveKind switch
        {
            UnityEngine.Mesh.PrimitiveKind.Sphere => new SphereMesh(),
            _ => new BoxMesh()
        };
    }

    private static StandardMaterial3D CreateGodotMaterial(UnityEngine.Material material)
    {
        return new StandardMaterial3D
        {
            AlbedoColor = ToGodot(material.color)
        };
    }

    private static Godot.Vector3 ToGodot(UnityEngine.Vector3 value)
    {
        return new Godot.Vector3(value.x, value.y, value.z);
    }

    private static Godot.Color ToGodot(UnityEngine.Color value)
    {
        return new Godot.Color(value.r, value.g, value.b, value.a);
    }

    private static Godot.Quaternion ToGodot(UnityEngine.Quaternion value)
    {
        return new Godot.Quaternion(value.x, value.y, value.z, value.w);
    }
}
