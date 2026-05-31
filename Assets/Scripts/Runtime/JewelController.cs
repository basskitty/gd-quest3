using UnityEngine;

public class JewelController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            UIManager.Instance.TriggerVictory();
            gameObject.SetActive(false);
        }
    }
}
