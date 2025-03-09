using UnityEngine;
using System.Collections;

public class ObjectCubeRespawn : MonoBehaviour
{
    // Time delay before the cube reactivates
    public float respawnDelay = 5f;

    // Cache the components
    private Collider cubeCollider;
    private Renderer cubeRenderer;

    void Start()
    {
        cubeCollider = GetComponent<Collider>();
        cubeRenderer = GetComponent<Renderer>();
    }

    // When something collides with the cube (using trigger or collision based on your setup)
    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player (or any specific tag you need)
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            StartCoroutine(DisableAndRespawn());
            //Debug.Log("CubeWorks");
        }
    }

    // Coroutine to disable and then re-enable the cube components
    IEnumerator DisableAndRespawn()
    {
        // Hide the cube and disable collisions
        cubeRenderer.enabled = false;
        cubeCollider.enabled = false;

        // Wait for the specified respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Re-enable the cube's visual and collision components
        cubeRenderer.enabled = true;
        cubeCollider.enabled = true;
    }
}
