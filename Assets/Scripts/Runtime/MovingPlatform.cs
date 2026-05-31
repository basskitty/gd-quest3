using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float platformSpeed = 1.0f;
    public Vector3 start = Vector3.zero;
    public Vector3 end = Vector3.one;
    [NonSerialized]
    public float editorPlatformPercent;
    
    private Vector3 lastPosition;
    public Vector3 PlatformVelocity { get; private set; }

    void Start()
    {
        lastPosition = transform.localPosition;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;
        var mf = this.GetComponent<MeshFilter>();
        var pos = Vector3.Lerp(start, end, editorPlatformPercent);
        Gizmos.DrawWireMesh( mf.sharedMesh, pos);
    }

    void FixedUpdate()
    {
        float pingPong = Mathf.PingPong(Time.fixedTime * this.platformSpeed, 1.0f);

        var newPosition = Vector3.Lerp(start, end, pingPong);
        
        Debug.Log($"pingPong: {pingPong}, newPosition: {newPosition}, localPosition: {transform.localPosition}");

      //  this.transform.localPosition = newPosition;
        this.transform.position = newPosition;

        PlatformVelocity = (newPosition - lastPosition) / Time.fixedDeltaTime;

        lastPosition = newPosition;
    }
}