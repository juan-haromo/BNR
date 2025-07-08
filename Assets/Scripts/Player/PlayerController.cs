using Mirror;
using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] float moveSpeed;
    public float jumpForce;
    public float drag;

    [Header("Collision")]
    public LayerMask groundMask;

    
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
            rb.AddForce(moveDirection.normalized * moveSpeed,ForceMode.Force); 
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x,0,rb.linearVelocity.z);
            if(rb.linearVelocity.magnitude > moveSpeed)
            {
                Vector3 maxSpeed = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);
                rb.linearVelocity = maxSpeed.normalized * moveSpeed;
            }
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
    #endregion
}
