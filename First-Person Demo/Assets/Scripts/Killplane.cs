using UnityEngine;

/// <summary>
/// Reset player position when touched
/// </summary>

public class Killplane : MonoBehaviour
{
    GameObject player;
    CharacterController controller;
    [SerializeField] Transform spawn;

    // References
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        controller = player.GetComponent<CharacterController>();
    }

    // "Respawn" player
    void OnTriggerEnter()
    {
        // Disabling controller to avoid position overwrite
        controller.enabled = false;

        // Reset position
        player.transform.position = spawn.position;

        controller.enabled = true;
    }
}
