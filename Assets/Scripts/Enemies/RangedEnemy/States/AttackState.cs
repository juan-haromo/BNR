using UnityEngine; 

public class AttackState : EnemyState
{
    RangedEnemyController controller;
    float attackDelay = 2.5f;
    float attackReadyTime;
    float exitStateTime;
    bool canShoot;
    public AttackState(RangedEnemyController controller, EnemyStateMachine stateMachine) : base(stateMachine)
    {
        this.controller = controller;
    }

    public override void Enter()
    {
        controller.SetLookDirection(controller.target.position);
        controller.IsMoving(false);
        canShoot = true;
        attackReadyTime = Time.time + attackDelay;
        exitStateTime = attackReadyTime + controller.shootCooldown;
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        if (!controller.Stats.IsAlive)
        {attackReadyTime = Time.time + attackDelay;  return; }
        controller.SetLookDirection(controller.target.position);
        if (Time.time > attackReadyTime && canShoot) 
        {
            canShoot = false;
            controller.Shoot(controller.target.position);
        }
        if(Time.time > exitStateTime)
        {
            stateMachine.ChangeState(controller.WanderState);
        }

    }
}
