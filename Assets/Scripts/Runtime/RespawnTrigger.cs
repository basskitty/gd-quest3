using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    public Transform respawnPoint; // Assign in Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Character>(out Character character))
        {
            character.InflictDamage(character.GetMaxHealth()); // kill the player
        }
    }

    public void Respawn(CharacterController characterController)
    {
        characterController.enabled = false;
        characterController.transform.position = respawnPoint.position;
        characterController.enabled = true;
    }
}