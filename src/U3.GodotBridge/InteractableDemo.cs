using UnityEngine;

namespace U3.GodotBridge;

public sealed class InteractableDemo : MonoBehaviour
{
    public string DisplayName { get; set; } = "Interactable";

    public string Prompt { get; set; } = "Interact";

    public bool WasInteracted { get; private set; }

    public void Interact()
    {
        WasInteracted = true;
        Debug.Log($"{Prompt}: {DisplayName}");

        var renderer = GetComponent<MeshRenderer>();
        if (renderer is not null)
        {
            renderer.material.color = new Color(1f, 1f, 0f);
        }
    }
}
