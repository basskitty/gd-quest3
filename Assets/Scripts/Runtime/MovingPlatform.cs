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
        float pingPong = Mathf.PingPong(Time.fixedTime * this.platformSpeed, 1.0f);

        var newPosition = Vector3.Lerp(start, end, pingPong);

        this.transform.localPosition = newPosition;

        PlatformVelocity = (newPosition - lastPosition) / Time.fixedDeltaTime;

        lastPosition = newPosition;
    }
}