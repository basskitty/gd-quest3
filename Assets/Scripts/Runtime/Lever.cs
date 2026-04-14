using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{
    private bool on = false;
    private InputAction interactAction;
    
    [SerializeField]
    private Transform onPosition;
    [SerializeField]
    private Transform offPosition;
    [SerializeField]
    private GameObject leverHandle;

    [SerializeField] private float switchTime;

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.interactAction = InputSystem.actions.FindAction("Interact");
    }

    IEnumerator InterpolateLeverCoroutine()
    {
        Vector3 startPosition, targetPosition;
        Quaternion startRotation, targetRotation;

        if (this.on)
        {
            startPosition = this.offPosition.position;
            targetPosition = this.onPosition.position;
            
            startRotation = this.offPosition.rotation;
            targetRotation = this.onPosition.rotation;
        }
        else
        {
            startPosition = this.onPosition.position;
            targetPosition = this.offPosition.position;
            
            startRotation = this.onPosition.rotation;
            targetRotation = this.offPosition.rotation;
        }

        float currentInterpolationTime = 0.0f;
        while (currentInterpolationTime < this.switchTime)
        {
            float percent = currentInterpolationTime / switchTime;
            
            var currentPosition = Vector3.Lerp(startPosition, targetPosition, percent);
            var currentRotation = Quaternion.Slerp(startRotation, targetRotation, percent);
            
            this.leverHandle.transform.SetPositionAndRotation(currentPosition, currentRotation);
            
            yield return null;
            
            currentInterpolationTime += Time.deltaTime;
        }
    }

    void ToggleLever()
    {
        this.on = !this.on;
        this.StartCoroutine(this.InterpolateLeverCoroutine());
        // if(this.on) {
        //     this.leverHandle.transform.SetPositionAndRotation(this.onPosition.position, this.onPosition.rotation);
        // } else {
        //     this.leverHandle.transform.SetPositionAndRotation(this.offPosition.position, this.offPosition.rotation);
        // }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerInRange && this.interactAction.WasPressedThisFrame())
        {
            this.ToggleLever();
        }
    }
    
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            playerInRange = false;
        }
    }
}
