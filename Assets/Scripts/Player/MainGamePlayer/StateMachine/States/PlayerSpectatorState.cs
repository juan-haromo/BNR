using UnityEngine;

public class PlayerSpectatorState : PlayerState
{
    float x = 0;
    float y = 0;
    float z = 0;

    Vector3 input;
    public PlayerSpectatorState(PlayerStateMachine stateMachine, PlayerController controller) : base(stateMachine, controller)
    {
    }

    public override void Enter()
    {
        controller.Stats.SetExpectating(true);
    }

    public override void Exit()
    {
        controller.Stats.SetExpectating(false);
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        x = controller.Input.SpectatorMovement.LeftRight.ReadValue<float>();
        y = controller.Input.SpectatorMovement.UpDown.ReadValue<float>();
        z = controller.Input.SpectatorMovement.ForwardBack.ReadValue<float>();

        input = (x * controller.transform.right) + (y * controller.transform.up) + (z * controller.transform.forward);
        controller.rb.linearVelocity = input.normalized * controller.spectatorSpeed;
    }
}
