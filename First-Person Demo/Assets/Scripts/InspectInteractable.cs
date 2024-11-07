using UnityEngine;

/// <summary>
/// Manages 'interaction' and 'inspection' of each object.
/// </summary>

public class InspectInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("Message to print to console when 'interacting' with this object")]
    [SerializeField] string interactMessage;

    [Tooltip("Message to print to console when 'inspecting' this object")]
    [SerializeField] string inspectMessage;

    public void Interaction()
    {
        Debug.Log(interactMessage);
    }

    public void Inspection()
    {
        Debug.Log(inspectMessage);
    }
}
