using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float circleRadius = 4f;

    private float currentAngle = 0f;
    [Header("Stomp")]
    [SerializeField] private AudioClip stompSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private bool isDead = false;
    [SerializeField]
    private CapsuleCollider col;

    void Start()
    {
        this.startPosition = this.transform.position;
        this.col = this.GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if (this.isDead) return;
        HandleMovement();
    }

    void HandleMovement()
    {
        this.currentAngle += this.moveSpeed * Time.deltaTime;

        float x = this.startPosition.x + Mathf.Cos(this.currentAngle) * this.circleRadius;
        float z = this.startPosition.z + Mathf.Sin(this.currentAngle) * this.circleRadius;

        Vector3 newPosition = new Vector3(x, this.transform.position.y, z);

        // Face the direction of movement
        Vector3 moveDirection = (newPosition - this.transform.position).normalized;
        if (moveDirection != Vector3.zero)
            this.transform.forward = moveDirection;

        this.transform.position = newPosition;
    }

    public void Stomp()
    {
        this.isDead = true;
        this.col.enabled = false; // disable collider so it can't be hit again

        // Play sound
        if (this.stompSound != null && this.audioSource != null)
            this.audioSource.PlayOneShot(this.stompSound);

        // Squash then disappear using DOTween sequence
        Sequence squash = DOTween.Sequence();
        squash.Append(this.transform.DOScaleY(0.1f, 0.15f).SetEase(Ease.OutQuart));
        squash.Append(this.transform.DOScaleX(1.0f, 0.1f));  // squish wide
        squash.AppendInterval(0.2f);                           // hold for a moment
        squash.Append(this.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        squash.OnComplete(() => Destroy(this.gameObject));
    }
}