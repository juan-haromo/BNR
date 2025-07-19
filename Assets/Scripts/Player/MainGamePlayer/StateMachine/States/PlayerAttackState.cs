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
        Debug.Log(controller.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        Debug.Log("Enter attack Current time:" + Time.time + " exit time" + exitTime);
        controller.weapon.ChangeHitbox(true);
    }

    public override void Exit()
    {
        Debug.Log("Exit attack");
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
