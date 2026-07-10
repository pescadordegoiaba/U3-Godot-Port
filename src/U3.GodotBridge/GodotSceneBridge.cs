using Godot;
using UnityEngine;

namespace U3.GodotBridge;

public sealed class GodotSceneBridge
{
    private readonly Dictionary<GameObject, Node3D> _nodesByGameObject = new();
    private readonly Dictionary<GameObject, Node> _rootNodesByGameObject = new();
    private readonly Dictionary<GameObject, MeshInstance3D> _meshInstancesByGameObject = new();
    private readonly Dictionary<GameObject, Camera3D> _camerasByGameObject = new();
    private readonly Dictionary<GameObject, Light3D> _lightsByGameObject = new();
    private readonly Dictionary<Collider, CollisionObject3D> _collisionObjectsByCollider = new();
    private readonly Dictionary<Collider, CollisionShape3D> _collisionShapesByCollider = new();
    private readonly Dictionary<CollisionObject3D, Collider> _collidersByCollisionObject = new();
    private readonly Dictionary<UnityEngine.Mesh, Godot.Mesh> _meshResourcesByUnityMesh = new();
    private readonly Dictionary<UnityEngine.Material, MaterialCacheEntry> _materialResourcesByUnityMaterial = new();
    private readonly HashSet<GameObject> _warnedMissingParentNodes = new();

    public int MeshResourcesCreated { get; private set; }

    public int MaterialResourcesCreated { get; private set; }

    public int MeshInstancesCreated { get; private set; }

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
            SyncCamera(pair.Key, pair.Value);
            SyncLight(pair.Key, pair.Value);
            SyncCollider(pair.Key, pair.Value);
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
        _camerasByGameObject.Clear();
        _lightsByGameObject.Clear();
        _collisionObjectsByCollider.Clear();
        _collisionShapesByCollider.Clear();
        _collidersByCollisionObject.Clear();
        _meshResourcesByUnityMesh.Clear();
        _materialResourcesByUnityMaterial.Clear();
        _warnedMissingParentNodes.Clear();
        MeshResourcesCreated = 0;
        MaterialResourcesCreated = 0;
        MeshInstancesCreated = 0;
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
        node.Quaternion = ToGodot(transform.localRotation.normalized);
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
        meshInstance.Mesh = GetOrCreateGodotMesh(unityMesh);
        meshInstance.Visible = gameObject.activeInHierarchy && meshRenderer.enabled;
        meshInstance.MaterialOverride = GetOrCreateGodotMaterial(meshRenderer.sharedMaterial);
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
        MeshInstancesCreated++;
        return meshInstance;
    }

    private void SyncCamera(GameObject gameObject, Node3D node)
    {
        var camera = gameObject.GetComponent<UnityEngine.Camera>();
        if (camera is null)
        {
            return;
        }

        var cameraNode = GetOrCreateCamera(gameObject, node);
        var active = gameObject.activeInHierarchy && camera.enabled;
        cameraNode.Current = active;
        cameraNode.Visible = active;
        cameraNode.Fov = camera.fieldOfView;
        cameraNode.Near = camera.nearClipPlane;
        cameraNode.Far = camera.farClipPlane;
    }

    private Camera3D GetOrCreateCamera(GameObject gameObject, Node3D node)
    {
        if (_camerasByGameObject.TryGetValue(gameObject, out var cameraNode))
        {
            if (!ReferenceEquals(cameraNode.GetParent(), node))
            {
                cameraNode.Reparent(node, keepGlobalTransform: false);
            }

            return cameraNode;
        }

        cameraNode = new Camera3D
        {
            Name = $"{node.Name}Camera"
        };

        node.AddChild(cameraNode);
        _camerasByGameObject.Add(gameObject, cameraNode);
        return cameraNode;
    }

    private void SyncLight(GameObject gameObject, Node3D node)
    {
        var light = gameObject.GetComponent<UnityEngine.Light>();
        if (light is null)
        {
            return;
        }

        var lightNode = GetOrCreateLight(gameObject, node, light.type);
        lightNode.Visible = gameObject.activeInHierarchy && light.enabled;
        lightNode.LightEnergy = light.intensity;
        lightNode.LightColor = ToGodot(light.color);

        switch (lightNode)
        {
            case OmniLight3D omniLight:
                omniLight.OmniRange = light.range;
                break;
            case SpotLight3D spotLight:
                spotLight.SpotRange = light.range;
                break;
        }
    }

    private Light3D GetOrCreateLight(GameObject gameObject, Node3D node, LightType lightType)
    {
        if (_lightsByGameObject.TryGetValue(gameObject, out var lightNode))
        {
            if (!MatchesLightType(lightNode, lightType))
            {
                lightNode.QueueFree();
                _lightsByGameObject.Remove(gameObject);
            }
            else
            {
                if (!ReferenceEquals(lightNode.GetParent(), node))
                {
                    lightNode.Reparent(node, keepGlobalTransform: false);
                }

                return lightNode;
            }
        }

        lightNode = CreateLightNode(lightType);
        lightNode.Name = $"{node.Name}Light";
        node.AddChild(lightNode);
        _lightsByGameObject.Add(gameObject, lightNode);
        return lightNode;
    }

    public bool TryGetUnityCollider(CollisionObject3D collisionObject, out Collider? collider)
    {
        return _collidersByCollisionObject.TryGetValue(collisionObject, out collider);
    }

    private void SyncCollider(GameObject gameObject, Node3D node)
    {
        var collider = gameObject.GetComponent<Collider>();
        if (collider is null)
        {
            return;
        }

        var collisionObject = GetOrCreateCollisionObject(gameObject, node, collider);
        var shapeNode = GetOrCreateCollisionShape(collider, collisionObject);

        collisionObject.Position = ToGodot(GetColliderLocalCenter(collider));
        collisionObject.CollisionLayer = ToGodotLayer(gameObject.layer);
        collisionObject.CollisionMask = uint.MaxValue;
        shapeNode.Disabled = !gameObject.activeInHierarchy || !collider.enabled;
        shapeNode.Shape = CreateOrUpdateShape(collider, shapeNode.Shape);
    }

    private CollisionObject3D GetOrCreateCollisionObject(GameObject gameObject, Node3D node, Collider collider)
    {
        if (_collisionObjectsByCollider.TryGetValue(collider, out var existingObject))
        {
            if (!MatchesCollisionObject(existingObject, collider))
            {
                _collidersByCollisionObject.Remove(existingObject);
                existingObject.QueueFree();
                _collisionObjectsByCollider.Remove(collider);
                _collisionShapesByCollider.Remove(collider);
            }
            else
            {
                if (!ReferenceEquals(existingObject.GetParent(), node))
                {
                    existingObject.Reparent(node, keepGlobalTransform: false);
                }

                return existingObject;
            }
        }

        var collisionObject = CreateCollisionObject(gameObject, collider);
        node.AddChild(collisionObject);
        _collisionObjectsByCollider[collider] = collisionObject;
        _collidersByCollisionObject[collisionObject] = collider;
        return collisionObject;
    }

    private static bool MatchesCollisionObject(CollisionObject3D collisionObject, Collider collider)
    {
        if (collider.isTrigger)
        {
            return collisionObject is Area3D;
        }

        if (collider.attachedRigidbody is not null)
        {
            return collisionObject is AnimatableBody3D;
        }

        return collisionObject is StaticBody3D;
    }

    private static CollisionObject3D CreateCollisionObject(GameObject gameObject, Collider collider)
    {
        CollisionObject3D collisionObject = collider.isTrigger
            ? new Area3D()
            : collider.attachedRigidbody is not null
                ? new AnimatableBody3D()
                : new StaticBody3D();

        collisionObject.Name = $"{gameObject.name}Collision";
        return collisionObject;
    }

    private CollisionShape3D GetOrCreateCollisionShape(Collider collider, CollisionObject3D collisionObject)
    {
        if (_collisionShapesByCollider.TryGetValue(collider, out var shapeNode))
        {
            if (!ReferenceEquals(shapeNode.GetParent(), collisionObject))
            {
                shapeNode.Reparent(collisionObject, keepGlobalTransform: false);
            }

            return shapeNode;
        }

        shapeNode = new CollisionShape3D
        {
            Name = $"{collisionObject.Name}Shape"
        };
        collisionObject.AddChild(shapeNode);
        _collisionShapesByCollider[collider] = shapeNode;
        return shapeNode;
    }

    private static Shape3D CreateOrUpdateShape(Collider collider, Shape3D? currentShape)
    {
        switch (collider)
        {
            case BoxCollider boxCollider:
                var boxShape = currentShape as BoxShape3D ?? new BoxShape3D();
                boxShape.Size = ToGodot(boxCollider.size);
                return boxShape;
            case SphereCollider sphereCollider:
                var sphereShape = currentShape as SphereShape3D ?? new SphereShape3D();
                sphereShape.Radius = UnityEngine.Mathf.Abs(sphereCollider.radius);
                return sphereShape;
            case CapsuleCollider capsuleCollider:
                var capsuleShape = currentShape as CapsuleShape3D ?? new CapsuleShape3D();
                capsuleShape.Radius = UnityEngine.Mathf.Abs(capsuleCollider.radius);
                capsuleShape.Height = UnityEngine.Mathf.Max(UnityEngine.Mathf.Abs(capsuleCollider.height), capsuleShape.Radius * 2f);
                return capsuleShape;
            default:
                var fallbackShape = currentShape as BoxShape3D ?? new BoxShape3D();
                fallbackShape.Size = ToGodot(collider.bounds.size);
                return fallbackShape;
        }
    }

    private static UnityEngine.Vector3 GetColliderLocalCenter(Collider collider)
    {
        return collider switch
        {
            BoxCollider boxCollider => boxCollider.center,
            SphereCollider sphereCollider => sphereCollider.center,
            CapsuleCollider capsuleCollider => capsuleCollider.center,
            _ => UnityEngine.Vector3.zero
        };
    }

    private static uint ToGodotLayer(int layer)
    {
        return layer is >= 0 and < 32 ? 1u << layer : 1u;
    }

    private static bool MatchesLightType(Light3D lightNode, LightType lightType)
    {
        return lightType switch
        {
            LightType.Point => lightNode is OmniLight3D,
            LightType.Spot => lightNode is SpotLight3D,
            _ => lightNode is DirectionalLight3D
        };
    }

    private static Light3D CreateLightNode(LightType lightType)
    {
        return lightType switch
        {
            LightType.Point => new OmniLight3D(),
            LightType.Spot => new SpotLight3D(),
            _ => new DirectionalLight3D()
        };
    }

    private Godot.Mesh GetOrCreateGodotMesh(UnityEngine.Mesh mesh)
    {
        if (_meshResourcesByUnityMesh.TryGetValue(mesh, out var godotMesh))
        {
            return godotMesh;
        }

        godotMesh = mesh.primitiveKind switch
        {
            UnityEngine.Mesh.PrimitiveKind.Sphere => new SphereMesh(),
            _ => new BoxMesh()
        };

        _meshResourcesByUnityMesh.Add(mesh, godotMesh);
        MeshResourcesCreated++;
        return godotMesh;
    }

    private StandardMaterial3D GetOrCreateGodotMaterial(UnityEngine.Material material)
    {
        if (!_materialResourcesByUnityMaterial.TryGetValue(material, out var entry))
        {
            entry = new MaterialCacheEntry(new StandardMaterial3D());
            _materialResourcesByUnityMaterial.Add(material, entry);
            MaterialResourcesCreated++;
        }

        var color = ToGodot(material.color);
        if (entry.LastColor != color)
        {
            entry.Material.AlbedoColor = color;
            entry.LastColor = color;
        }

        if (entry.LastName != material.name)
        {
            entry.Material.ResourceName = material.name;
            entry.LastName = material.name;
        }

        return entry.Material;
    }

    internal static Godot.Vector3 ToGodot(UnityEngine.Vector3 value)
    {
        return new Godot.Vector3(value.x, value.y, value.z);
    }

    internal static UnityEngine.Vector3 ToUnity(Godot.Vector3 value)
    {
        return new UnityEngine.Vector3(value.X, value.Y, value.Z);
    }

    private static Godot.Color ToGodot(UnityEngine.Color value)
    {
        return new Godot.Color(value.r, value.g, value.b, value.a);
    }

    private static Godot.Quaternion ToGodot(UnityEngine.Quaternion value)
    {
        return new Godot.Quaternion(value.x, value.y, value.z, value.w);
    }

    private sealed class MaterialCacheEntry
    {
        public MaterialCacheEntry(StandardMaterial3D material)
        {
            Material = material;
        }

        public StandardMaterial3D Material { get; }

        public Godot.Color LastColor { get; set; }

        public string LastName { get; set; } = string.Empty;
    }
}
