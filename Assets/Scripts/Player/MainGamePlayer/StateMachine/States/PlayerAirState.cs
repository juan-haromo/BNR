using UnityEngine;

public class PlayerAirState : PlayerState
{
    float velocity;
    public PlayerAirState(PlayerStateMachine stateMachine, PlayerController controller) : base(stateMachine, controller)
    {
    }

    public override void Enter()
    {
        velocity = 0;
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
        controller.Move(controller.InputDirection);
    }

    public override void Update()
    {
        CheckGround();
        controller.rb.linearVelocity = new Vector3(controller.rb.linearVelocity.x, controller.rb.linearVelocity.y - (9.81f * Time.deltaTime), controller.rb.linearVelocity.z);
        controller.UpdateMoveInput();
    }


    void CheckGround()
    {
        Debug.DrawRay(controller.PlayerCollider.transform.position, Vector3.down * ((controller.PlayerCollider.height / 2) + 0.1f), Color.magenta);

        if (Physics.Raycast(controller.PlayerCollider.transform.position, Vector3.down, (controller.PlayerCollider.height / 2) + 0.1f, controller.groundMask))
        {
            stateMachine.ChangeState(controller.GroundState);
        }
    }
}
