using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyController : EnemyController
{

    public WanderState WanderState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }

    public BasicProjectile projectile;
    public Transform projectileStart;

    [SyncVar(hook = nameof(DestinationChanged))]
    Vector3 destination;
    [SyncVar(hook = nameof(LookDirectionChanged))]
    Vector3 lookDirection;
    [SyncVar(hook = nameof(MovingChanged))]
    bool isMoving;


    public float shootCooldown;


    void Start()
    {
        StateMachine = new EnemyStateMachine(this);
        WanderState = new WanderState(this, StateMachine);
        ChaseState = new ChaseState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);

        StateMachine.Initialize(WanderState);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) { return; }
        StateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        if(!isServer) { return; }
        StateMachine.CurrentState.FixedUpdate();
    }

    #region Movement

    public void SetTarget(Transform target)
    {
        ServerSetTarget(target);
    }

    [Server]
    void ServerSetTarget(Transform target)
    {
        if(!isServer){ return; }
        this.target = target;
    }
    public void SetDestination(Vector3 newDestination)
    {
        if (!isServer) { return; }
        ServerSetDestination(newDestination);
    }

    [Server]
    void ServerSetDestination(Vector3 newDestination)
    {
        destination = newDestination;
    }

    void DestinationChanged(Vector3 oldDestination, Vector3 newDestination)
    {
        lookDirection = newDestination;
        navMeshAgent.SetDestination(newDestination);
    }

    public void SetLookDirection(Vector3 newLookDirection)
    {
        if (!isServer) { return; }
        ServerSetLookDirection(newLookDirection);
    }

    [Server]
    public void ServerSetLookDirection(Vector3 newLookDirection)
    {
        lookDirection = newLookDirection;
    }

    void LookDirectionChanged(Vector3 oldDirection, Vector3 newDirection)
    {
        Vector3 lookPosition = newDirection - transform.position;
        lookPosition.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.2f);
    }

    public void IsMoving(bool newMoving)
    {
        ServerIsMoving(newMoving);
    }

    [Server]
     void ServerIsMoving(bool newMoving)
    {
        isMoving = newMoving;
    }

    void MovingChanged(bool oldIsMoving, bool newIsMoving)
    {
        navMeshAgent.isStopped = newIsMoving;
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;
    }
    #endregion

    #region Shooting
    public void Shoot(Vector3 direction)
    {
        ServerShoot(direction);
    }

    [Server]
    void ServerShoot(Vector3 direction)
    {
        GameObject bulletInstance = Instantiate(projectile.gameObject, projectileStart.position, projectile.transform.rotation);
        bulletInstance.GetComponent<BasicProjectile>().Initialize(gameObject, direction - projectileStart.position);
        NetworkServer.Spawn(bulletInstance);
    }

    public Collider[] CheckForPlayer()
    {
        return ServerCheckForPlayer();
    }

    [Server]
    Collider[] ServerCheckForPlayer()
    {
        return Physics.OverlapSphere(transform.position, detectionRange, playerMask);
    }

    public float GetDistance()
    {
        
        return ServerGetDistance();
    }

    [Server]
    public float ServerGetDistance()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    #endregion
}
