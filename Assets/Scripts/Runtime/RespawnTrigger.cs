using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    public Transform respawnPoint; // Assign in Inspector

    void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        CharacterController characterController = other.GetComponent<CharacterController>();

        if (characterController != null)
        {
            Respawn(characterController);
        }
    }

    void Respawn(CharacterController characterController)
    {
        // Deactivate the CharacterController to avoid collisions during teleport
        characterController.enabled = false;

        // Move the player to the respawn point
        characterController.transform.position = respawnPoint.position;

        // Reactivate the CharacterController
        characterController.enabled = true;
    }
}