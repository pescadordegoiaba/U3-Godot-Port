using UnityEngine;

namespace U3.GodotBridge;

public enum DemoSpawnShape
{
    Box,
    Sphere,
    Capsule
}

public sealed class DemoSpawnDefinition
{
    public int Id { get; init; }

    public string Name { get; init; } = "Demo Object";

    public DemoSpawnShape Shape { get; init; }

    public Color Color { get; init; } = Color.white;

    public string InteractPrompt { get; init; } = "Interact";

    public Vector3 Position { get; init; }

    public GameObject CreateGameObject()
    {
        var gameObject = new GameObject(Name);
        gameObject.transform.position = Position;
        gameObject.transform.localScale = Shape == DemoSpawnShape.Capsule ? new Vector3(0.7f, 1.8f, 0.7f) : Vector3.one;

        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = Shape == DemoSpawnShape.Sphere ? Mesh.CreateSphere() : Mesh.CreateBox();

        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material { color = Color };

        switch (Shape)
        {
            case DemoSpawnShape.Sphere:
                gameObject.AddComponent<SphereCollider>().radius = 0.5f;
                break;
            case DemoSpawnShape.Capsule:
                var capsule = gameObject.AddComponent<CapsuleCollider>();
                capsule.radius = 0.35f;
                capsule.height = 1.8f;
                break;
            default:
                gameObject.AddComponent<BoxCollider>().size = Vector3.one;
                break;
        }

        var interactable = gameObject.AddComponent<InteractableDemo>();
        interactable.DisplayName = Name;
        interactable.Prompt = InteractPrompt;
        return gameObject;
    }
}
