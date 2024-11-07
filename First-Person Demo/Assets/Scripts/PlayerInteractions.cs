using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages player interactions with objects containing an 'Interaction' and an 'Inspection' method.
/// 
/// - Interacts with objects hit by a raycast if close enough
/// - Updates crosshair and object outlines
/// 
/// Interact: Press 'Interact'
/// Inspect: Hold 'Interact'
/// </summary>

public interface IInteractable
{
    void Interaction();
}

public class PlayerInteractions : MonoBehaviour
{
    // Player Settings
    [Header("Player Settings")]
    [Tooltip("Layer containing all interactable objects")]
    public LayerMask interactLayer;
    [Tooltip("Max distance required to interact with objects")]
    public float handReach = 2f;
    [Tooltip("How long to press 'Interact' to trigger 'Inspection'")]
    [SerializeField] float holdTime = 1f;

    // Crosshair
    [Header("Crosshair")]
    [SerializeField] GameObject crosshair;
    RawImage crosshairImage;
    [SerializeField] Texture normalCrosshairSprite;
    [SerializeField] Texture interactCrosshairSprite;

    // References
    new Camera camera;
    Ray ray;
    GameObject lastHitObject;

    // 'Inspect' logic
    Outline outline;
    bool isInspecting = false;
    float timer = 0f;

    void Start()
    {
        // References
        camera = gameObject.GetComponent<Camera>();
        crosshairImage = crosshair.GetComponent<RawImage>();

        // Locks and hides cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ray = new Ray(transform.position, transform.forward);

        // If not out of reach
        if (Physics.Raycast(ray, out RaycastHit hit, handReach))
        {
            // If interactable
            if (((1 << hit.collider.gameObject.layer) & interactLayer) != 0)
            {
                outline = hit.collider.gameObject.GetComponent<Outline>();

                if (lastHitObject != hit.collider.gameObject) // If we look at something else
                {
                    if (lastHitObject != null)
                    {
                        outline.OutlineWidth = 0f; // Hide outline
                    }

                    lastHitObject = hit.collider.gameObject;
                    outline.OutlineWidth = 10f; // Show outline
                }

                // If inspectable
                var inspectMethod = hit.collider.gameObject.GetComponent<IInteractable>()?.GetType().GetMethod("Inspection"); // Check for Inspect method

                if (inspectMethod != null)
                {
                    // Interactions
                    if (Input.GetButtonDown("Interact"))
                    {
                        StartCoroutine(HandleInteraction(hit));
                    }

                    // Inspections
                    if (Input.GetButtonUp("Interact"))
                    {
                        if (!isInspecting)
                        {
                            Interact(hit);
                        }

                        StopCoroutine(HandleInteraction(hit));
                        timer = 0f;
                        isInspecting = false;
                    }
                }
                else // If not inspectable
                {
                    // Interactions
                    if (Input.GetButtonDown("Interact"))
                    {
                        Interact(hit);
                    }
                }

                // Update crosshair
                crosshairImage.texture = interactCrosshairSprite;
            }
            else
            {
                ResetCrosshair();
            }
        }
        else
        {
            // If looking away from inspectable
            if (lastHitObject != null)
            {
                // Hide outline
                outline.OutlineWidth = 0f;
                lastHitObject = null;
            }

            ResetCrosshair();
        }
    }

    void ResetCrosshair()
    {
        crosshairImage.texture = normalCrosshairSprite;
    }

    // Call 'Interaction' method
    void Interact(RaycastHit hit)
    {
        IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interaction();
        }
    }

    // Call 'Inspection' method
    void Inspect(RaycastHit hit)
    {
        InspectInteractable inspectInteractable = hit.collider.gameObject.GetComponent<InspectInteractable>();
        inspectInteractable.Inspection();
    }

    // Check for how long 'Interact' is being pressed
    IEnumerator HandleInteraction(RaycastHit hit)
    {
        float t = 0f;
        timer = 0f;

        while (Input.GetButton("Interact")) // If 'Interact' is being pressed
        {
            if (lastHitObject == hit.collider.gameObject) // and we're looking at the same object
            {
                timer += Time.deltaTime;
                t = timer / holdTime;

                // Interpolate outline color
                outline.OutlineColor = Color.Lerp(Color.yellow, Color.red, t);

                if (timer >= holdTime) // Start inspection
                {
                    Inspect(hit);
                    isInspecting = true;
                    outline.OutlineColor = Color.yellow;
                    yield break;
                }

                yield return null;
            }
            else
            {
                outline.OutlineColor = Color.yellow;
                yield return null; // Start interaction
            }
        }
    }
}
