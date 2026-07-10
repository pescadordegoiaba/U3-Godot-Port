using Godot;
using UnityEngine;
using UnityQuaternion = UnityEngine.Quaternion;
using UnityVector3 = UnityEngine.Vector3;

namespace U3.GodotBridge;

public sealed class GodotPhysicsBridge : IPhysicsBackend
{
    private readonly Node3D _worldRoot;
    private readonly GodotSceneBridge _sceneBridge;

    public GodotPhysicsBridge(Node3D worldRoot, GodotSceneBridge sceneBridge)
    {
        _worldRoot = worldRoot;
        _sceneBridge = sceneBridge;
    }

    public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        hitInfo = default;
        var world = _worldRoot.GetWorld3D();
        if (world is null)
        {
            return false;
        }

        var from = GodotSceneBridge.ToGodot(ray.origin);
        var to = GodotSceneBridge.ToGodot(ray.GetPoint(float.IsPositiveInfinity(maxDistance) ? 10000f : maxDistance));
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        query.CollisionMask = layerMask < 0 ? uint.MaxValue : (uint)layerMask;
        query.CollideWithAreas = queryTriggerInteraction != QueryTriggerInteraction.Ignore;
        query.CollideWithBodies = true;

        var result = world.DirectSpaceState.IntersectRay(query);
        if (result.Count == 0)
        {
            return false;
        }

        var point = GodotSceneBridge.ToUnity(result["position"].AsVector3());
        var normal = GodotSceneBridge.ToUnity(result["normal"].AsVector3());
        var collisionObject = result["collider"].AsGodotObject() as CollisionObject3D;

        _sceneBridge.TryGetUnityCollider(collisionObject!, out var collider);
        hitInfo = new RaycastHit
        {
            collider = collider,
            point = point,
            normal = normal,
            distance = UnityEngine.Vector3.Distance(ray.origin, point)
        };
        return true;
    }

    public RaycastHit[] RaycastAll(Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return Raycast(ray, out var hit, maxDistance, layerMask, queryTriggerInteraction)
            ? new[] { hit }
            : Array.Empty<RaycastHit>();
    }

    public Collider[] OverlapSphere(UnityVector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        var shape = new SphereShape3D { Radius = radius };
        return IntersectShape(shape, GodotSceneBridge.ToGodot(position), Godot.Quaternion.Identity, layerMask, queryTriggerInteraction);
    }

    public Collider[] OverlapBox(UnityVector3 center, UnityVector3 halfExtents, UnityQuaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        var shape = new BoxShape3D { Size = GodotSceneBridge.ToGodot(halfExtents * 2f) };
        return IntersectShape(shape, GodotSceneBridge.ToGodot(center), ToGodot(orientation), layerMask, queryTriggerInteraction);
    }

    public Collider[] OverlapCapsule(UnityVector3 point0, UnityVector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        var center = (point0 + point1) * 0.5f;
        var height = Math.Max(UnityVector3.Distance(point0, point1) + (radius * 2f), radius * 2f);
        var shape = new CapsuleShape3D
        {
            Radius = radius,
            Height = height
        };
        return IntersectShape(shape, GodotSceneBridge.ToGodot(center), Godot.Quaternion.Identity, layerMask, queryTriggerInteraction);
    }

    public int OverlapSphereNonAlloc(UnityVector3 position, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return Copy(OverlapSphere(position, radius, layerMask, queryTriggerInteraction), results);
    }

    public int OverlapBoxNonAlloc(UnityVector3 center, UnityVector3 halfExtents, Collider[] results, UnityQuaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return Copy(OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction), results);
    }

    public int OverlapCapsuleNonAlloc(UnityVector3 point0, UnityVector3 point1, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return Copy(OverlapCapsule(point0, point1, radius, layerMask, queryTriggerInteraction), results);
    }

    public bool CheckSphere(UnityVector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return OverlapSphere(position, radius, layerMask, queryTriggerInteraction).Length > 0;
    }

    public bool CheckBox(UnityVector3 center, UnityVector3 halfExtents, UnityQuaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction).Length > 0;
    }

    public bool CheckCapsule(UnityVector3 start, UnityVector3 end, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return OverlapCapsule(start, end, radius, layerMask, queryTriggerInteraction).Length > 0;
    }

    private Collider[] IntersectShape(Shape3D shape, Godot.Vector3 origin, Godot.Quaternion rotation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        var world = _worldRoot.GetWorld3D();
        if (world is null)
        {
            return Array.Empty<Collider>();
        }

        var query = new PhysicsShapeQueryParameters3D
        {
            Shape = shape,
            Transform = new Transform3D(new Basis(rotation), origin),
            CollisionMask = layerMask < 0 ? uint.MaxValue : (uint)layerMask,
            CollideWithAreas = queryTriggerInteraction != QueryTriggerInteraction.Ignore,
            CollideWithBodies = true
        };

        var colliders = new HashSet<Collider>();
        foreach (var result in world.DirectSpaceState.IntersectShape(query, maxResults: 64))
        {
            var collisionObject = result["collider"].AsGodotObject() as CollisionObject3D;
            if (collisionObject is not null
                && _sceneBridge.TryGetUnityCollider(collisionObject, out var collider)
                && collider is not null
                && collider.enabled
                && collider.gameObject.activeInHierarchy)
            {
                colliders.Add(collider);
            }
        }

        return colliders.ToArray();
    }

    private static int Copy(Collider[] source, Collider[] results)
    {
        var count = Math.Min(source.Length, results.Length);
        Array.Copy(source, results, count);
        return count;
    }

    private static Godot.Quaternion ToGodot(UnityQuaternion value)
    {
        return new Godot.Quaternion(value.x, value.y, value.z, value.w);
    }
}
