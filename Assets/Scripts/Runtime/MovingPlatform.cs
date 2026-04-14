using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float platformSpeed = 1.0f;
    [SerializeField] private Vector3 start = Vector3.zero;
    [SerializeField] private Vector3 end = Vector3.one;
    private Vector3 lastPosition;
    public Vector3 PlatformVelocity { get; private set; }

    void Start()
    {
        lastPosition = transform.localPosition;
    }

    void FixedUpdate()
    {
        // Calculate ping-pong value for smooth back-and-forth movement
        float pingPong = Mathf.PingPong(Time.fixedTime * platformSpeed, 1.0f);

        // Calculate new position using Lerp
        Vector3 newPosition = Vector3.Lerp(start, end, pingPong);

        // Update platform position
        transform.localPosition = newPosition;

        // Calculate velocity: (current position - last position) / fixedDeltaTime
        PlatformVelocity = (newPosition - lastPosition) / Time.fixedDeltaTime;

        // Update lastPosition for next frame
        lastPosition = newPosition;
    }
}