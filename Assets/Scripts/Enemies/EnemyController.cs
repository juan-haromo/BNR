using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    protected EnemyStateMachine StateMachine;
    public NavMeshAgent navMeshAgent;
    [SyncVar]
    public Transform target;
    public float attackRange;
    public float detectionRange;
    public LayerMask playerMask;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position,detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
