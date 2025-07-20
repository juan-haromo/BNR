using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
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
    public float spectatorSpeed;
    float currentSpeed;
    public float jumpForce;
    public float drag;

    [Header("Collision")]
    public LayerMask groundMask;

    [Header("Animation")]
    [SerializeField] Animator animator;
    public Animator Animator => animator;
    [SyncVar(hook = nameof(AnimationChanged))]
    private string currentAnimation = string.Empty;
    [SyncVar]
    bool canChangeAnimation = true;

    [Header("Stats")]
    [SerializeField] PlayerStats stats;
    public PlayerStats Stats => stats;

    [Header("Weapons")]
    public NetworkWeapon weapon;

    public LayerMask playerMask;


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
    public PlayerSpectatorState SpectatorState { get; private set; }

    public PlayerAttackState AttackState { get; private set; }

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
        SpectatorState = new PlayerSpectatorState(StateMachine, this);
        AttackState = new PlayerAttackState(StateMachine, this);

        StateMachine.Initialize(AirState);
        if (!isLocalPlayer) 
        {
            gameObject.SetActive(false);
        }
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
        IsRunning = Input.PlayerMovement.Run.IsPressed();
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
            SetAnimationFloat("Speed",isRunning? 1 : 0.5f);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            SetAnimationFloat("Speed",0);
        }
    }

    public void SetCanChangeAnimation(bool canChange)
    {
        CmdCanChangeAnimation(canChange);
    }

    [Command]
    void CmdCanChangeAnimation(bool canChange)
    {
        canChangeAnimation = canChange;
    }
    #endregion
    public void SetAnimation(string newAnimation, bool persistentAnimation)
    {
        CMDSetAnimation(newAnimation, persistentAnimation);
    }

    [Command]
     void CMDSetAnimation(string newAnimation, bool persistentAnimation)
    {
        if(newAnimation == currentAnimation || !canChangeAnimation) { return; }
        currentAnimation = newAnimation;
        canChangeAnimation = persistentAnimation;
    }

    [Command]
    public void SetAnimationFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }

    void AnimationChanged(string oldAnimation, string newAnimation)
    {
        animator.CrossFade(newAnimation,0.2f);
    }

    [SyncVar]
    bool heavyAttack;

    public void SetAttackType(bool isHeavy)
    {
        CmdSetAttackType(isHeavy);
    }

    [Command]
    void CmdSetAttackType(bool isHeavy)
    {
        heavyAttack = isHeavy;
    }


    public float GetAttackDuration()
    {
        string animationName = heavyAttack ? "HeavyAttack" : "LightAttack";
        Debug.Log("Attack type " +  animationName);
        RuntimeAnimatorController runtimeAnimator = animator.runtimeAnimatorController;
        foreach(AnimationClip clip in runtimeAnimator.animationClips)
        {
            if(clip.name == animationName)
            {
                return clip.length;
            }
        }

        return 0f;
    }
}
