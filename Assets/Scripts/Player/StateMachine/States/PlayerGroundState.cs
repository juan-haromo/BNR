using Mirror;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundState : PlayerState
{
    float nextAttack = 0;
    float attackDelay = 1.0f;
    public PlayerGroundState(PlayerStateMachine stateMachine, PlayerController controller) : base(stateMachine, controller)
    {
    }

    public override void Enter()
    {
        controller.drag = 5;
        controller.Input.PlayerMovement.Jump.performed += Jump;
        controller.Input.PlayerMovement.Attack.performed += Attack;
        controller.Input.PlayerMovement.Run.started += StartRun;
        controller.Input.PlayerMovement.Run.canceled += StopRun;

        controller.SetAnimation("GroundBlend");
    }

    public override void Exit()
    {
        controller.Input.PlayerMovement.Jump.performed -= Jump;
        controller.Input.PlayerMovement.Attack.performed -= Attack;
        controller.Input.PlayerMovement.Run.started -= StartRun;
        controller.Input.PlayerMovement.Run.canceled -= StopRun;
        controller.IsRunning = false;
    }

    public override void FixedUpdate()
    {
        controller.Move(controller.InputDirection);
    }

    public override void Update()
    {
        CheckGround();
        controller.UpdateMoveInput();
    }


    void CheckGround()
    {
        Debug.DrawRay(controller.PlayerCollider.transform.position, Vector3.down * ((controller.PlayerCollider.height/2) + 0.1f), Color.magenta);
        if (!Physics.Raycast(controller.PlayerCollider.transform.position, Vector3.down, (controller.PlayerCollider.height/2) + 0.1f, controller.groundMask))
        {
            stateMachine.ChangeState(controller.AirState);
        }
    }


    private void StopRun(InputAction.CallbackContext context)
    {
       controller.IsRunning = false;
    }

    private void StartRun(InputAction.CallbackContext context)
    {
        controller.IsRunning = true;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        controller.rb.AddForce(Vector3.up * controller.jumpForce, ForceMode.Impulse);
    }

    void Attack(InputAction.CallbackContext context)
    {        
        if (Time.time < nextAttack) { Debug.Log("Attack on cooldown"); return; }
        nextAttack = Time.time + attackDelay;
        CommandAttack(controller.Player.position, controller.Player.forward);
    }

    [Command]
    void CommandAttack(Vector3 position, Vector3 direction)
    {

        Debug.Log("Attack");
        RaycastHit[] hits = Physics.SphereCastAll(position, 1,direction , 10);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != controller.PlayerCollider)
            {
                if (hit.collider.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    damagable.DealDamage(10, controller.PlayerCollider.gameObject);
                }
            }
        }
    }
}
