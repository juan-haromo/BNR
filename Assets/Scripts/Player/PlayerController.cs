using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IDamagable
{
    [Header("Player References")]
    [SerializeField] Transform player;
    public Transform Player => player;
    [SerializeField] Transform playerModel;
    [SerializeField] Transform orientation;
    public Rigidbody rb;
    [SerializeField] CapsuleCollider playerCollider;
    public CapsuleCollider PlayerCollider => playerCollider;

    [Header("Movement")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    float currentSpeed;
    public float jumpForce;
    public float drag;

    [Header("Collision")]
    public LayerMask groundMask;

    [Header("Animation")]
    [SerializeField] Animator animator;
    private string currentAnimation = string.Empty;

    [Header("Stats")]
    [SerializeField] PlayerStats stats;

    [Header("Weapons")]
    [SerializeField] Weapon weapon;


    public bool IsRunning 
    {  
        get=>IsRunning;
        set
        {
            currentSpeed = value? runSpeed : walkSpeed;
            isRunning = value;
        }
    }
    bool isRunning;
    
    public IA_Player Input { get; private set; }

    public Vector3 InputDirection { get; private set; }
    Vector3 viewDir;

    #region StateMachine
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerGroundState GroundState { get; private set; }
    public PlayerAirState AirState { get; private set; }

    #endregion

    #region Unity Calls
    private void Start()
    {
        //Input set up
        Input = new IA_Player();
        Input.PlayerMovement.Enable();


        //Cursor set up
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = walkSpeed;

        //State machine set up
        StateMachine = new PlayerStateMachine(this);
        GroundState = new PlayerGroundState(StateMachine, this);
        AirState = new PlayerAirState(StateMachine, this);

        StateMachine.Initialize(AirState);
    }

    private void OnDestroy()
    {
        StateMachine.CurrentState.Exit();
        Input.Disable();
    }

    private void Update()
    {
        if (!isLocalPlayer) { return; }
        StateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return;  }
        StateMachine.CurrentState.FixedUpdate();
    }
    #endregion
    
    #region Mirror Calls
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (!isLocalPlayer) { return; }
        gameObject.SetActive(true);
    }

    #endregion

    #region Movement
    public void UpdateMoveInput()
    {
        
        viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        InputDirection = orientation.forward * Input.PlayerMovement.ForwardMove.ReadValue<float>() + orientation.right * Input.PlayerMovement.SideMove.ReadValue<float>();

        if (InputDirection != Vector3.zero) 
        {
            playerModel.forward = Vector3.Slerp(playerModel.forward, InputDirection.normalized, Time.deltaTime * rotationSpeed);
        }
        
    }
    public void Move(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            moveDirection = moveDirection.normalized * currentSpeed;
            moveDirection.y = rb.linearVelocity.y;
            rb.linearVelocity = moveDirection;
            animator.SetFloat("Speed",isRunning? 1 : 0.5f);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            animator.SetFloat("Speed",0);
        }
    }
    #endregion

    public void SetAnimation(string newAnimation)
    {
        if(newAnimation == currentAnimation) { return; }
        currentAnimation = newAnimation;
        animator.Play(currentAnimation);
    }

    [Command]
    public void DealDamage(float amount, GameObject dealer)
    {
        if (dealer == gameObject) { return; }

        stats.TakeDamage(amount);
    }
}
