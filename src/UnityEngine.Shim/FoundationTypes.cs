namespace UnityEngine;

public struct Bounds
{
    private Vector3 _center;
    private Vector3 _extents;

    public Bounds(Vector3 center, Vector3 size)
    {
        _center = center;
        _extents = size * 0.5f;
    }

    public Vector3 center
    {
        readonly get => _center;
        set => _center = value;
    }

    public Vector3 size
    {
        readonly get => _extents * 2f;
        set => _extents = value * 0.5f;
    }

    public Vector3 extents
    {
        readonly get => _extents;
        set => _extents = value;
    }

    public readonly Vector3 min => _center - _extents;

    public readonly Vector3 max => _center + _extents;

    public readonly bool Contains(Vector3 point)
    {
        var currentMin = min;
        var currentMax = max;
        return point.x >= currentMin.x && point.x <= currentMax.x
            && point.y >= currentMin.y && point.y <= currentMax.y
            && point.z >= currentMin.z && point.z <= currentMax.z;
    }

    public void Encapsulate(Vector3 point)
    {
        SetMinMax(Vector3.Min(min, point), Vector3.Max(max, point));
    }

    public readonly bool Intersects(Bounds bounds)
    {
        var currentMin = min;
        var currentMax = max;
        var otherMin = bounds.min;
        var otherMax = bounds.max;
        return currentMin.x <= otherMax.x && currentMax.x >= otherMin.x
            && currentMin.y <= otherMax.y && currentMax.y >= otherMin.y
            && currentMin.z <= otherMax.z && currentMax.z >= otherMin.z;
    }

    public void Expand(float amount)
    {
        _extents += Vector3.one * (amount * 0.5f);
    }

    public readonly Vector3 ClosestPoint(Vector3 point)
    {
        var currentMin = min;
        var currentMax = max;
        return new Vector3(
            Mathf.Clamp(point.x, currentMin.x, currentMax.x),
            Mathf.Clamp(point.y, currentMin.y, currentMax.y),
            Mathf.Clamp(point.z, currentMin.z, currentMax.z));
    }

    public void SetMinMax(Vector3 min, Vector3 max)
    {
        _center = (min + max) * 0.5f;
        _extents = (max - min) * 0.5f;
    }
}

public struct Ray
{
    public Vector3 origin;
    public Vector3 direction;

    public Ray(Vector3 origin, Vector3 direction)
    {
        this.origin = origin;
        this.direction = direction.normalized;
    }

    public readonly Vector3 GetPoint(float distance)
    {
        return origin + (direction * distance);
    }
}

public struct RaycastHit
{
    public Vector3 point { get; set; }

    public Vector3 normal { get; set; }

    public float distance { get; set; }

    public Collider? collider { get; set; }

    public readonly Transform? transform => collider?.transform;

    public readonly Rigidbody? rigidbody => collider?.attachedRigidbody ?? collider?.gameObject.GetComponent<Rigidbody>();
}

public interface IPhysicsBackend
{
    bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    RaycastHit[] RaycastAll(Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    Collider[] OverlapSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    bool CheckSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

    bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);
}

public enum QueryTriggerInteraction
{
    UseGlobal,
    Ignore,
    Collide
}

public enum ForceMode
{
    Force,
    Acceleration,
    Impulse,
    VelocityChange
}

public enum CollisionDetectionMode
{
    Discrete,
    Continuous,
    ContinuousDynamic,
    ContinuousSpeculative
}

public enum RigidbodyInterpolation
{
    None,
    Interpolate,
    Extrapolate
}

[Flags]
public enum RigidbodyConstraints
{
    None = 0,
    FreezePositionX = 1 << 1,
    FreezePositionY = 1 << 2,
    FreezePositionZ = 1 << 3,
    FreezeRotationX = 1 << 4,
    FreezeRotationY = 1 << 5,
    FreezeRotationZ = 1 << 6,
    FreezePosition = FreezePositionX | FreezePositionY | FreezePositionZ,
    FreezeRotation = FreezeRotationX | FreezeRotationY | FreezeRotationZ,
    FreezeAll = FreezePosition | FreezeRotation
}

public class Collider : Component
{
    private Bounds _bounds = new(Vector3.zero, Vector3.zero);

    public bool enabled { get; set; } = true;

    public bool isTrigger { get; set; }

    public PhysicMaterial? sharedMaterial { get; set; }

    public PhysicMaterial? material
    {
        get => sharedMaterial;
        set => sharedMaterial = value;
    }

    public virtual Bounds bounds
    {
        get => _bounds;
        set => _bounds = value;
    }

    public Rigidbody? attachedRigidbody { get; internal set; }

    public virtual Vector3 ClosestPoint(Vector3 position)
    {
        return bounds.ClosestPoint(position);
    }

    public virtual bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
    {
        return Physics.RaycastColliderBounds(this, ray, out hitInfo, maxDistance);
    }
}

public class BoxCollider : Collider
{
    public Vector3 center { get; set; }

    public Vector3 size { get; set; } = Vector3.one;

    public override Bounds bounds
    {
        get => new(transform.TransformPoint(center), AbsVector(Vector3.Scale(size, transform.localScale)));
        set
        {
            center = transform.InverseTransformPoint(value.center);
            size = value.size;
        }
    }

    public override Vector3 ClosestPoint(Vector3 position)
    {
        return bounds.ClosestPoint(position);
    }

    private static Vector3 AbsVector(Vector3 value)
    {
        return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
    }
}

public class SphereCollider : Collider
{
    public Vector3 center { get; set; }

    public float radius { get; set; } = 0.5f;

    public override Bounds bounds
    {
        get
        {
            var scale = transform.localScale;
            var maxScale = Mathf.Max(Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y)), Mathf.Abs(scale.z));
            var scaledRadius = Mathf.Abs(radius) * maxScale;
            return new Bounds(transform.TransformPoint(center), Vector3.one * (scaledRadius * 2f));
        }
        set
        {
            center = transform.InverseTransformPoint(value.center);
            radius = Mathf.Max(Mathf.Max(value.extents.x, value.extents.y), value.extents.z);
        }
    }

    public override Vector3 ClosestPoint(Vector3 position)
    {
        var worldCenter = transform.TransformPoint(center);
        var direction = position - worldCenter;
        if (direction.sqrMagnitude < 1E-06f)
        {
            return worldCenter;
        }

        var scaledRadius = bounds.extents.x;
        return worldCenter + (direction.normalized * scaledRadius);
    }
}

public class CapsuleCollider : Collider
{
    public Vector3 center { get; set; }

    public float radius { get; set; } = 0.5f;

    public float height { get; set; } = 2f;

    public int direction { get; set; } = 1;

    public override Bounds bounds
    {
        get
        {
            var size = Vector3.one * (Mathf.Abs(radius) * 2f);
            var heightAxis = Mathf.Max(Mathf.Abs(height), Mathf.Abs(radius) * 2f);
            switch (direction)
            {
                case 0:
                    size.x = heightAxis;
                    break;
                case 2:
                    size.z = heightAxis;
                    break;
                default:
                    size.y = heightAxis;
                    break;
            }

            var scale = transform.localScale;
            size = new Vector3(Mathf.Abs(size.x * scale.x), Mathf.Abs(size.y * scale.y), Mathf.Abs(size.z * scale.z));
            return new Bounds(transform.TransformPoint(center), size);
        }
        set
        {
            center = transform.InverseTransformPoint(value.center);
            radius = Mathf.Min(value.extents.x, value.extents.z);
            height = value.size.y;
        }
    }

    public override Vector3 ClosestPoint(Vector3 position)
    {
        return bounds.ClosestPoint(position);
    }
}

public class Rigidbody : Component
{
    private bool _sleeping;

    public Vector3 position { get; set; }

    public Quaternion rotation { get; set; } = Quaternion.identity;

    public Vector3 velocity { get; set; }

    public Vector3 angularVelocity { get; set; }

    public float mass { get; set; } = 1f;

    public bool isKinematic { get; set; }

    public bool useGravity { get; set; } = true;

    public bool detectCollisions { get; set; } = true;

    public bool freezeRotation { get; set; }

    public RigidbodyConstraints constraints { get; set; }

    public CollisionDetectionMode collisionDetectionMode { get; set; }

    public RigidbodyInterpolation interpolation { get; set; }

    public void MovePosition(Vector3 position)
    {
        this.position = position;
        transform.position = position;
    }

    public void MoveRotation(Quaternion rotation)
    {
        this.rotation = rotation;
        transform.rotation = rotation;
    }

    public void AddForce(Vector3 force)
    {
        AddForce(force, ForceMode.Force);
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        if (isKinematic)
        {
            return;
        }

        var divisor = mass <= 0f ? 1f : mass;
        velocity += mode switch
        {
            ForceMode.Acceleration => force * Time.deltaTime,
            ForceMode.Impulse => force / divisor,
            ForceMode.VelocityChange => force,
            _ => (force / divisor) * Time.deltaTime
        };
        _sleeping = false;
    }

    public void Sleep()
    {
        _sleeping = true;
    }

    public void WakeUp()
    {
        _sleeping = false;
    }

    public bool IsSleeping()
    {
        return _sleeping;
    }
}

public class PhysicMaterial : Object
{
    public float bounciness { get; set; }

    public float dynamicFriction { get; set; } = 0.6f;

    public float staticFriction { get; set; } = 0.6f;
}

public class CharacterController : Collider
{
    public float height { get; set; } = 2f;

    public float radius { get; set; } = 0.5f;

    public Vector3 center { get; set; }

    public bool isGrounded { get; private set; }

    public Vector3 velocity { get; private set; }

    public override Bounds bounds => new(transform.TransformPoint(center), new Vector3(radius * 2f, height, radius * 2f));

    public Vector3 Move(Vector3 motion)
    {
        transform.position += motion;
        velocity = Time.deltaTime > 0f ? motion / Time.deltaTime : motion;
        isGrounded = motion.y <= 0f && Physics.CheckSphere(transform.position + center + Vector3.down * ((height * 0.5f) + 0.05f), radius * 0.95f);
        return motion;
    }

    public bool SimpleMove(Vector3 speed)
    {
        Move(speed * Time.deltaTime);
        return isGrounded;
    }
}

public static class Physics
{
    private static readonly List<Collider> Colliders = new();
    private static IPhysicsBackend? _backend;

    public static Vector3 gravity { get; set; } = new(0f, -9.81f, 0f);

    public static IEnumerable<Collider> AllColliders => Colliders.Where(IsUsableCollider);

    public static void SetBackend(IPhysicsBackend backend)
    {
        _backend = backend;
    }

    public static void ResetBackend()
    {
        _backend = null;
    }

    internal static void ResetForTests()
    {
        _backend = null;
        Colliders.Clear();
    }

    internal static void RegisterCollider(Collider collider)
    {
        if (!Colliders.Contains(collider))
        {
            Colliders.Add(collider);
        }
    }

    internal static void UnregisterCollider(Collider collider)
    {
        Colliders.Remove(collider);
    }

    public static bool Raycast(Ray ray)
    {
        return Raycast(ray, out _, float.PositiveInfinity, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static bool Raycast(Ray ray, float maxDistance)
    {
        return Raycast(ray, out _, maxDistance, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static bool Raycast(Ray ray, out RaycastHit hitInfo)
    {
        return Raycast(ray, out hitInfo, float.PositiveInfinity, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
    {
        return Raycast(ray, out hitInfo, maxDistance, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        }

        return RaycastRegisteredColliders(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static bool Raycast(Vector3 origin, Vector3 direction)
    {
        return Raycast(new Ray(origin, direction));
    }

    public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance)
    {
        return Raycast(new Ray(origin, direction), maxDistance);
    }

    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
    {
        return Raycast(new Ray(origin, direction), out hitInfo);
    }

    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        return Raycast(new Ray(origin, direction), out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
    }

    public static RaycastHit[] RaycastAll(Ray ray, float maxDistance = float.PositiveInfinity, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
        }

        var hits = new List<RaycastHit>();
        foreach (var collider in AllColliders)
        {
            if (ColliderMatches(collider, layerMask, queryTriggerInteraction) && RaycastColliderBounds(collider, ray, out var hit, maxDistance))
            {
                hits.Add(hit);
            }
        }

        return hits.OrderBy(hit => hit.distance).ToArray();
    }

    public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.OverlapSphere(position, radius, layerMask, queryTriggerInteraction);
        }

        return AllColliders
            .Where(collider => ColliderMatches(collider, layerMask, queryTriggerInteraction))
            .Where(collider => Vector3.Distance(collider.bounds.ClosestPoint(position), position) <= radius)
            .ToArray();
    }

    public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents)
    {
        return OverlapBox(center, halfExtents, Quaternion.identity, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
        }

        var bounds = new Bounds(center, halfExtents * 2f);
        return AllColliders
            .Where(collider => ColliderMatches(collider, layerMask, queryTriggerInteraction))
            .Where(collider => collider.bounds.Intersects(bounds))
            .ToArray();
    }

    public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius)
    {
        return OverlapCapsule(point0, point1, radius, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.OverlapCapsule(point0, point1, radius, layerMask, queryTriggerInteraction);
        }

        var bounds = CapsuleBounds(point0, point1, radius);
        return AllColliders
            .Where(collider => ColliderMatches(collider, layerMask, queryTriggerInteraction))
            .Where(collider => collider.bounds.Intersects(bounds))
            .ToArray();
    }

    public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.OverlapSphereNonAlloc(position, radius, results, layerMask, queryTriggerInteraction);
        }

        return CopyResults(OverlapSphere(position, radius, layerMask, queryTriggerInteraction), results);
    }

    public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.OverlapBoxNonAlloc(center, halfExtents, results, orientation, layerMask, queryTriggerInteraction);
        }

        return CopyResults(OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction), results);
    }

    public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results)
    {
        return OverlapBoxNonAlloc(center, halfExtents, results, Quaternion.identity, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.OverlapCapsuleNonAlloc(point0, point1, radius, results, layerMask, queryTriggerInteraction);
        }

        return CopyResults(OverlapCapsule(point0, point1, radius, layerMask, queryTriggerInteraction), results);
    }

    public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results)
    {
        return OverlapCapsuleNonAlloc(point0, point1, radius, results, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static bool CheckSphere(Vector3 position, float radius, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.CheckSphere(position, radius, layerMask, queryTriggerInteraction);
        }

        return OverlapSphere(position, radius, layerMask, queryTriggerInteraction).Length > 0;
    }

    public static bool CheckBox(Vector3 center, Vector3 halfExtents)
    {
        return CheckBox(center, halfExtents, Quaternion.identity, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask = ~0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.CheckBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction);
        }

        return OverlapBox(center, halfExtents, orientation, layerMask, queryTriggerInteraction).Length > 0;
    }

    public static bool CheckCapsule(Vector3 start, Vector3 end, float radius)
    {
        return CheckCapsule(start, end, radius, ~0, QueryTriggerInteraction.UseGlobal);
    }

    public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (_backend is not null)
        {
            return _backend.CheckCapsule(start, end, radius, layerMask, queryTriggerInteraction);
        }

        return OverlapCapsule(start, end, radius, layerMask, queryTriggerInteraction).Length > 0;
    }

    public static void IgnoreCollision(Collider collider1, Collider collider2, bool ignore = true)
    {
    }

    internal static bool RaycastColliderBounds(Collider collider, Ray ray, out RaycastHit hitInfo, float maxDistance)
    {
        hitInfo = default;
        if (!IsUsableCollider(collider))
        {
            return false;
        }

        var bounds = collider.bounds;
        if (!RayIntersectsBounds(ray, bounds, maxDistance, out var distance))
        {
            return false;
        }

        var point = ray.GetPoint(distance);
        hitInfo = new RaycastHit
        {
            collider = collider,
            distance = distance,
            point = point,
            normal = EstimateBoundsNormal(bounds, point)
        };
        return true;
    }

    private static bool RaycastRegisteredColliders(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        hitInfo = default;
        var bestDistance = float.PositiveInfinity;
        var found = false;

        foreach (var collider in AllColliders)
        {
            if (!ColliderMatches(collider, layerMask, queryTriggerInteraction))
            {
                continue;
            }

            if (RaycastColliderBounds(collider, ray, out var candidate, maxDistance) && candidate.distance < bestDistance)
            {
                bestDistance = candidate.distance;
                hitInfo = candidate;
                found = true;
            }
        }

        return found;
    }

    private static bool IsUsableCollider(Collider collider)
    {
        return !collider.IsDestroyed && !collider.gameObject.IsDestroyed && collider.enabled && collider.gameObject.activeInHierarchy;
    }

    private static bool ColliderMatches(Collider collider, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        if (!IsUsableCollider(collider))
        {
            return false;
        }

        if (((1 << collider.gameObject.layer) & layerMask) == 0)
        {
            return false;
        }

        return queryTriggerInteraction != QueryTriggerInteraction.Ignore || !collider.isTrigger;
    }

    private static int CopyResults(Collider[] source, Collider[] results)
    {
        var count = Math.Min(source.Length, results.Length);
        Array.Copy(source, results, count);
        return count;
    }

    private static Bounds CapsuleBounds(Vector3 point0, Vector3 point1, float radius)
    {
        var bounds = new Bounds((point0 + point1) * 0.5f, Vector3.zero);
        bounds.Encapsulate(point0);
        bounds.Encapsulate(point1);
        bounds.Expand(radius * 2f);
        return bounds;
    }

    private static bool RayIntersectsBounds(Ray ray, Bounds bounds, float maxDistance, out float distance)
    {
        distance = 0f;
        var min = bounds.min;
        var max = bounds.max;
        var tMin = 0f;
        var tMax = maxDistance;

        if (!Slab(ray.origin.x, ray.direction.x, min.x, max.x, ref tMin, ref tMax)
            || !Slab(ray.origin.y, ray.direction.y, min.y, max.y, ref tMin, ref tMax)
            || !Slab(ray.origin.z, ray.direction.z, min.z, max.z, ref tMin, ref tMax))
        {
            return false;
        }

        distance = tMin;
        return distance <= maxDistance;
    }

    private static bool Slab(float origin, float direction, float min, float max, ref float tMin, ref float tMax)
    {
        if (Mathf.Abs(direction) < 1E-06f)
        {
            return origin >= min && origin <= max;
        }

        var inverse = 1f / direction;
        var t1 = (min - origin) * inverse;
        var t2 = (max - origin) * inverse;
        if (t1 > t2)
        {
            (t1, t2) = (t2, t1);
        }

        tMin = Mathf.Max(tMin, t1);
        tMax = Mathf.Min(tMax, t2);
        return tMin <= tMax;
    }

    private static Vector3 EstimateBoundsNormal(Bounds bounds, Vector3 point)
    {
        var min = bounds.min;
        var max = bounds.max;
        var distances = new[]
        {
            (Mathf.Abs(point.x - min.x), Vector3.left),
            (Mathf.Abs(point.x - max.x), Vector3.right),
            (Mathf.Abs(point.y - min.y), Vector3.down),
            (Mathf.Abs(point.y - max.y), Vector3.up),
            (Mathf.Abs(point.z - min.z), Vector3.back),
            (Mathf.Abs(point.z - max.z), Vector3.forward)
        };

        return distances.OrderBy(pair => pair.Item1).First().Item2;
    }
}

public struct LayerMask
{
    public int value;

    public static implicit operator int(LayerMask mask)
    {
        return mask.value;
    }

    public static implicit operator LayerMask(int value)
    {
        return new LayerMask { value = value };
    }

    public static int GetMask(params string[] layerNames)
    {
        return 0;
    }

    public static int NameToLayer(string layerName)
    {
        return -1;
    }

    public static string LayerToName(int layer)
    {
        return string.Empty;
    }
}

public struct Plane
{
    public Vector3 normal;
    public float distance;

    public Plane(Vector3 normal, float distance)
    {
        this.normal = normal.normalized;
        this.distance = distance;
    }

    public Plane(Vector3 normal, Vector3 point)
    {
        this.normal = normal.normalized;
        distance = -Vector3.Dot(this.normal, point);
    }

    public Plane(Vector3 a, Vector3 b, Vector3 c)
    {
        normal = Vector3.Cross(b - a, c - a).normalized;
        distance = -Vector3.Dot(normal, a);
    }

    public readonly float GetDistanceToPoint(Vector3 point)
    {
        return Vector3.Dot(normal, point) + distance;
    }

    public readonly bool GetSide(Vector3 point)
    {
        return GetDistanceToPoint(point) > 0f;
    }

    public readonly bool Raycast(Ray ray, out float enter)
    {
        var denominator = Vector3.Dot(normal, ray.direction);
        if (MathF.Abs(denominator) < 1E-06f)
        {
            enter = 0f;
            return false;
        }

        enter = -(Vector3.Dot(normal, ray.origin) + distance) / denominator;
        return enter >= 0f;
    }
}

public struct Matrix4x4
{
    public float m00;
    public float m01;
    public float m02;
    public float m03;
    public float m10;
    public float m11;
    public float m12;
    public float m13;
    public float m20;
    public float m21;
    public float m22;
    public float m23;
    public float m30;
    public float m31;
    public float m32;
    public float m33;

    public static Matrix4x4 identity => new()
    {
        m00 = 1f,
        m11 = 1f,
        m22 = 1f,
        m33 = 1f
    };

    public static Matrix4x4 zero => new();

    public float this[int row, int column]
    {
        readonly get
        {
            return (row, column) switch
            {
                (0, 0) => m00,
                (0, 1) => m01,
                (0, 2) => m02,
                (0, 3) => m03,
                (1, 0) => m10,
                (1, 1) => m11,
                (1, 2) => m12,
                (1, 3) => m13,
                (2, 0) => m20,
                (2, 1) => m21,
                (2, 2) => m22,
                (2, 3) => m23,
                (3, 0) => m30,
                (3, 1) => m31,
                (3, 2) => m32,
                (3, 3) => m33,
                _ => throw new IndexOutOfRangeException("Invalid Matrix4x4 index.")
            };
        }
        set
        {
            switch (row, column)
            {
                case (0, 0):
                    m00 = value;
                    break;
                case (0, 1):
                    m01 = value;
                    break;
                case (0, 2):
                    m02 = value;
                    break;
                case (0, 3):
                    m03 = value;
                    break;
                case (1, 0):
                    m10 = value;
                    break;
                case (1, 1):
                    m11 = value;
                    break;
                case (1, 2):
                    m12 = value;
                    break;
                case (1, 3):
                    m13 = value;
                    break;
                case (2, 0):
                    m20 = value;
                    break;
                case (2, 1):
                    m21 = value;
                    break;
                case (2, 2):
                    m22 = value;
                    break;
                case (2, 3):
                    m23 = value;
                    break;
                case (3, 0):
                    m30 = value;
                    break;
                case (3, 1):
                    m31 = value;
                    break;
                case (3, 2):
                    m32 = value;
                    break;
                case (3, 3):
                    m33 = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid Matrix4x4 index.");
            }
        }
    }

    public readonly Vector3 MultiplyPoint(Vector3 point)
    {
        var x = (m00 * point.x) + (m01 * point.y) + (m02 * point.z) + m03;
        var y = (m10 * point.x) + (m11 * point.y) + (m12 * point.z) + m13;
        var z = (m20 * point.x) + (m21 * point.y) + (m22 * point.z) + m23;
        var w = (m30 * point.x) + (m31 * point.y) + (m32 * point.z) + m33;
        return MathF.Abs(w) > 1E-06f && MathF.Abs(w - 1f) > 1E-06f
            ? new Vector3(x / w, y / w, z / w)
            : new Vector3(x, y, z);
    }

    public readonly Vector3 MultiplyPoint3x4(Vector3 point)
    {
        return new Vector3(
            (m00 * point.x) + (m01 * point.y) + (m02 * point.z) + m03,
            (m10 * point.x) + (m11 * point.y) + (m12 * point.z) + m13,
            (m20 * point.x) + (m21 * point.y) + (m22 * point.z) + m23);
    }

    public readonly Vector3 MultiplyVector(Vector3 vector)
    {
        return new Vector3(
            (m00 * vector.x) + (m01 * vector.y) + (m02 * vector.z),
            (m10 * vector.x) + (m11 * vector.y) + (m12 * vector.z),
            (m20 * vector.x) + (m21 * vector.y) + (m22 * vector.z));
    }

    public static Matrix4x4 Translate(Vector3 vector)
    {
        var matrix = identity;
        matrix.m03 = vector.x;
        matrix.m13 = vector.y;
        matrix.m23 = vector.z;
        return matrix;
    }

    public static Matrix4x4 Scale(Vector3 vector)
    {
        var matrix = identity;
        matrix.m00 = vector.x;
        matrix.m11 = vector.y;
        matrix.m22 = vector.z;
        return matrix;
    }

    public static Matrix4x4 Rotate(Quaternion q)
    {
        var rotation = q.normalized;
        var xx = rotation.x * rotation.x;
        var yy = rotation.y * rotation.y;
        var zz = rotation.z * rotation.z;
        var xy = rotation.x * rotation.y;
        var xz = rotation.x * rotation.z;
        var yz = rotation.y * rotation.z;
        var wx = rotation.w * rotation.x;
        var wy = rotation.w * rotation.y;
        var wz = rotation.w * rotation.z;

        var matrix = identity;
        matrix.m00 = 1f - (2f * (yy + zz));
        matrix.m01 = 2f * (xy - wz);
        matrix.m02 = 2f * (xz + wy);
        matrix.m10 = 2f * (xy + wz);
        matrix.m11 = 1f - (2f * (xx + zz));
        matrix.m12 = 2f * (yz - wx);
        matrix.m20 = 2f * (xz - wy);
        matrix.m21 = 2f * (yz + wx);
        matrix.m22 = 1f - (2f * (xx + yy));
        return matrix;
    }

    public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
    {
        return Translate(pos) * Rotate(q) * Scale(s);
    }

    public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
    {
        var result = zero;
        for (var row = 0; row < 4; row++)
        {
            for (var column = 0; column < 4; column++)
            {
                var value = 0f;
                for (var index = 0; index < 4; index++)
                {
                    value += left[row, index] * right[index, column];
                }

                result[row, column] = value;
            }
        }

        return result;
    }
}
