using UnityEngine;
using UnityEngine.InputSystem;
public class Character : MonoBehaviour
{
    private bool isJumping = false;
    private float jumpCooldownTimer;
    private CharacterController controller;
    private InputAction moveAction;
    private InputAction jumpAction;
    [SerializeField]
    private float jumpCooldown;
//We set gravity lower than in real live as it is more fun!
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float characterSpeed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float dampening;
    [SerializeField]
    private Transform cameraTransform;
    private Vector3 characterMovement;
    private Vector3 jumpVelocity;
    private Vector3 characterGravity;
    private Vector3 platformVelocity;
    private bool isOnPlatform;
    private bool jumpQueued;
    void Start()
    {
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;
    }

    void Update()
    {
        if (this.jumpAction != null && this.jumpAction.WasPressedThisFrame())
        {
            this.jumpQueued = true;
        }
    }

    void HandleJumping()
    {
        if (this.jumpCooldownTimer > 0.0f)
        {
            this.jumpCooldownTimer -= Time.fixedDeltaTime;
            if (this.jumpCooldownTimer < 0.0f)
            {
                this.jumpCooldownTimer = 0.0f;
            }
        }

        if (this.controller.isGrounded)
        {
            this.isJumping = false;
            this.characterGravity.y = 0.0f;

            if (this.jumpQueued && this.jumpCooldownTimer <= 0.0f)
            {
                this.jumpVelocity = Vector3.zero;
                this.jumpVelocity.y = this.jumpSpeed;
                this.jumpCooldownTimer = this.jumpCooldown;
                this.isJumping = true;
                this.jumpQueued = false;
            }
        }
        else
        {
            this.jumpQueued = false;
        }

        if (this.jumpVelocity.y > 0.0f)
        {
            this.jumpVelocity.y += this.gravity * Time.fixedDeltaTime;
            if (this.jumpVelocity.y < 0.0f)
            {
                this.jumpVelocity.y = 0.0f;
            }
        }
        else
        {
            this.jumpVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        this.HandleJumping();
        var inputMovement = this.moveAction.ReadValue<Vector2>();
        var inputRightDirection = this.cameraTransform.right;
        var inputForwardDirection = this.cameraTransform.forward;
        inputRightDirection.y = 0.0f;
        inputForwardDirection.y = 0.0f;
        inputRightDirection.Normalize();
        inputForwardDirection.Normalize();
        //Since we do not use the physics system, we have to simulate gravity ourselves
        if(this.controller.isGrounded) {
            this.characterGravity.y = 0.0f;
        }
        this.characterGravity.y += this.gravity * Time.fixedDeltaTime;
        this.characterMovement += this.characterGravity * Time.fixedDeltaTime;
        this.characterMovement += this.jumpVelocity * Time.fixedDeltaTime;
        this.characterMovement += inputRightDirection * inputMovement.x * this.characterSpeed * Time.fixedDeltaTime;
        this.characterMovement += inputForwardDirection * inputMovement.y * this.characterSpeed * Time.fixedDeltaTime;
        this.characterMovement *= (1 - this.dampening);
        Vector3 characterForward = this.characterMovement;
        characterForward.y = 0.0f;
        if (characterForward.sqrMagnitude > 0.0f && characterForward != Vector3.zero) {
            this.transform.forward = characterForward.normalized;
        }
    
        GetPlatformVelocity();
        var combinedMovement = this.characterMovement + this.platformVelocity * Time.fixedDeltaTime;
        this.controller.Move(combinedMovement);
    }
    
    private void GetPlatformVelocity()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, LayerMask.GetMask("Platforms")))
        {
            MovingPlatform platform = hit.collider.GetComponent<MovingPlatform>();
            if (platform != null && !this.jumpAction.WasPressedThisFrame())
            {
                platformVelocity = platform.PlatformVelocity;
                isOnPlatform = true;
            }
            else
            {
                platformVelocity = Vector3.zero;
                isOnPlatform = false;
            }
        }
        else
        {
            platformVelocity = Vector3.zero;
            isOnPlatform = false;
        }
    }
    
    
}