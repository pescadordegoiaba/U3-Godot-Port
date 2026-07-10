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
        return Array.Empty<Collider>();
    }

    public int OverlapSphereNonAlloc(UnityVector3 position, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return 0;
    }

    public int OverlapBoxNonAlloc(UnityVector3 center, UnityVector3 halfExtents, Collider[] results, UnityQuaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return 0;
    }

    public int OverlapCapsuleNonAlloc(UnityVector3 point0, UnityVector3 point1, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return 0;
    }

    public bool CheckSphere(UnityVector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        return false;
    }
}
