using UnityEngine;
using UnityEngine.InputSystem;
public class Character : MonoBehaviour
{
    private Animator animator;
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
    
    public AudioSource audioSource;
    public AudioSource audioSourceOneShot;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip stepSound;
    void Start()
    {
        this.controller = this.GetComponent<CharacterController>();
        this.moveAction = InputSystem.actions.FindAction("Move");
        this.jumpAction = InputSystem.actions.FindAction("Jump");
        this.jumpCooldownTimer = 0.0f;
        this.animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (this.jumpAction != null && this.jumpAction.WasPressedThisFrame())
        {
            this.jumpQueued = true;
        }
    }
    
    void HandleFootsteps(Vector3 inputMovement)
    {
        bool isMoving = this.controller.isGrounded && inputMovement != Vector3.zero;

        if (isMoving)
        {
            if (!this.audioSource.isPlaying)
            {
                this.audioSource.clip = this.stepSound;
                this.audioSource.loop = true;
                this.audioSource.Play();
            }
        }
        else
        {
            if (this.audioSource.isPlaying)
                this.audioSource.Stop();
        }
    }

    void HandleJumping()
    {
        this.jumpCooldownTimer = Mathf.Max(0.0f, this.jumpCooldownTimer - Time.fixedDeltaTime);

        if (this.controller.isGrounded)
        {
            if (this.isJumping) // was in the air last frame, now grounded
                this.audioSourceOneShot.PlayOneShot(this.landSound);
            
            this.isJumping = false;
            this.characterGravity.y = 0.0f;

            if (this.jumpQueued && this.jumpCooldownTimer <= 0.0f)
            {
                this.jumpVelocity = new Vector3(0.0f, this.jumpSpeed, 0.0f);
                this.jumpCooldownTimer = this.jumpCooldown;
                this.isJumping = true;
                this.jumpQueued = false;
                
                this.audioSourceOneShot.PlayOneShot(this.jumpSound);
            }
        }
        else
        {
            this.jumpQueued = false;
        }
        
        if (this.jumpVelocity.y > 0.0f)
        {
            this.jumpVelocity.y = Mathf.Max(0.0f, this.jumpVelocity.y + this.gravity * Time.fixedDeltaTime);
        }
        else
        {
            this.jumpVelocity = Vector3.zero;
        }
    }
    
    void SetAnimationState() {
        this.animator.SetBool("IsJumping", this.isJumping);
        
    }
    
    void SetAnimationState(Vector2 inputMovement) {
        this.animator.SetBool("IsJumping", this.isJumping);
        this.animator.SetBool("IsRunning", inputMovement != Vector2.zero);
        this.animator.SetFloat("MovementForward", inputMovement.magnitude);
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
        this.HandleFootsteps(inputMovement);
        this.SetAnimationState();
        this.SetAnimationState(inputMovement);
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
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Enemy")) return;

        // Check player is coming from above
        bool stompedFromAbove = hit.normal.y > 0.7f;
        if (!stompedFromAbove) return;

        Enemy enemy = hit.collider.GetComponent<Enemy>();
        if (enemy != null)
            enemy.Stomp();
    }
    
    
}