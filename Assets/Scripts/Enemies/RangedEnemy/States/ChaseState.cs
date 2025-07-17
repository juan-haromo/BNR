using UnityEngine;

public class ChaseState : EnemyState
{
    RangedEnemyController controller;
    float nextMove = 0;
    float moveDelay = .2f;
    public ChaseState( RangedEnemyController controller, EnemyStateMachine stateMachine) : base(stateMachine)
    {
        this.controller = controller;
    }

    public override void Enter()
    {
        Debug.Log("Chasing");
        controller.SetDestination(controller.target.position);
        controller.IsMoving(true);
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        float distance = Vector3.Distance(controller.transform.position, controller.target.position);
        if (distance > controller.detectionRange)
        {
            stateMachine.ChangeState(controller.WanderState);   
        }
        else if(distance <= controller.attackRange)
        {
            stateMachine.ChangeState(controller.AttackState);
        }
        else
        {
            if (Time.time >= nextMove)
            {
                nextMove = Time.time + moveDelay;
                controller.SetDestination(controller.target.position);
            }
        }
       
    }
}
