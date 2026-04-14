using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    public Transform respawnPoint; // Assign in Inspector

    void OnTriggerEnter(Collider other)
    {
        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
        {
            Respawn(characterController);
        }
    }

    void Respawn(CharacterController characterController)
    {
        characterController.enabled = false;
        characterController.transform.position = respawnPoint.position;
        characterController.enabled = true;
    }
}