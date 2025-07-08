using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(PlayerStateMachine stateMachine, PlayerController controller) : base(stateMachine, controller)
    {
    }

    public override void Enter()
    {
        Debug.Log("Enter ground state");
        controller.drag = 5;
        controller.Input.PlayerMovement.Jump.performed += Jump;
    }


    public override void Exit()
    {
        Debug.Log("Exit ground state");
        controller.Input.PlayerMovement.Jump.performed -= Jump;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        controller.rb.AddForce(Vector3.up * controller.jumpForce,ForceMode.Impulse);
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
}
