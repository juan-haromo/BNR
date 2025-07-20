using UnityEngine;
using UnityEngine.AI;

public class WanderState : EnemyState
{
    RangedEnemyController controller;
    float nextMove = 0;
    float moveDelay = 5.0f;
    
    public WanderState(RangedEnemyController controller, EnemyStateMachine stateMachine) : base(stateMachine)
    {
        this.controller = controller;
    }

    public override void Enter()
    {
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
        Collider[] checkForPlayer = controller.CheckForPlayer();

        if (checkForPlayer.Length > 0)
        {
            controller.SetTarget(checkForPlayer[0].gameObject.transform);
            stateMachine.ChangeState(controller.ChaseState);
        }
        else
        {
            if (nextMove < Time.time)
            {
                nextMove = Time.time + moveDelay;
                bool postitionFound = false;
                while (!postitionFound)
                {
                    Vector3 randomDirection = (Random.insideUnitSphere * controller.Stats.SpawnRadius) + controller.Stats.StartPosition;
                    if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, controller.detectionRange, NavMesh.AllAreas))
                    {
                        controller.SetDestination(hit.position);
                        postitionFound = true;
                    }
                }
               
            }
        }
    }
}
