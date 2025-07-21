using UnityEngine;

public class PlayerAttackState : PlayerState
{
    float exitTime;
    public PlayerAttackState(PlayerStateMachine stateMachine, PlayerController controller) : base(stateMachine, controller)
    {
    }

    public override void Enter()
    {

        exitTime = Time.time + controller.GetAttackDuration();
        controller.weapon.ChangeHitbox(true);
    }

    public override void Exit()
    {
        controller.weapon.ChangeHitbox(false);
        controller.SetCanChangeAnimation(true);
        controller.SetAnimation("GroundBlend",true);
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        if (Time.time >= exitTime)
        {
            stateMachine.ChangeState(controller.GroundState);
        }
    }
}
