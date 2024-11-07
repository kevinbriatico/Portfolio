using UnityEngine;

/// <summary>
/// A basic bouncing mechanic that works without rigidbodies.
/// An opposite force is applied to the player velocity when falling at a certain speed. Otherwise, acts as a normal collider.
/// </summary>

public class BounceForce : MonoBehaviour
{
    GameObject player;
    PlayerMovement playerMovement;
    Collider collisions;

    Vector3 velocity;
    float lastFallSpeed;

    // Init references
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        collisions = gameObject.GetComponent<Collider>();
    }
    void Update()
    {
        // Switch between trigger/standard collision
        if (playerMovement.lastFallSpeed < -10f)
        {
            collisions.isTrigger = true;
        }
        else
        {
            collisions.isTrigger = false;
            playerMovement.lastFallSpeed = 0;
        }
    }

    // Bounce
    void OnTriggerEnter(Collider other)
    {
        playerMovement.velocity.y *= -1;
    }
}
