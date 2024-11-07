using UnityEngine;

/// <summary>
/// Allow player to teleport across levels using the 'tab' key.
/// Checkpoint positions get updated when teleporting from one to another.
/// </summary>

public class PlayerTeleport : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] PlayerMovement movement;

    // Checkpoints
    [SerializeField] Transform point0;
    [SerializeField] Transform point1;

    // Index of checkpoint to teleport to next
    int nextCheckPoint = 1;

    void Update()
    {
        // Teleport
        if (Input.GetKeyDown("tab"))
        {
            // Disabling movement to avoid position overwrite
            controller.enabled = false;
            if (nextCheckPoint == 0)
            {
                // Update checkpoint position & rotation
                point1.position = transform.position;
                point1.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, transform.eulerAngles.y, 0);

                // Teleport player and update last checkpoint
                transform.position = point0.position;
                transform.rotation = Quaternion.Euler(0, point0.eulerAngles.y, 0);
                movement.xRotation = NormalizeAngle(point0.eulerAngles.x);

                // Update last checkpoint
                nextCheckPoint = 1;
            }
            else
            {
                // Update checkpoint position & rotation
                point0.position = transform.position;
                point0.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, transform.eulerAngles.y, 0);
                // Teleport player
                transform.position = point1.position;
                transform.rotation = Quaternion.Euler(0, point1.eulerAngles.y, 0);
                movement.xRotation = NormalizeAngle(point1.eulerAngles.x);

                // Update last checkpoint
                nextCheckPoint = 0;
            }
            // Re-enabling movement
            controller.enabled = true;
        }

        // Helper method to normalize angles
        float NormalizeAngle(float angle)
        {
            if (angle > 180) angle -= 360;
            if (angle < -180) angle += 360;
            return angle;
        }


    }
}
