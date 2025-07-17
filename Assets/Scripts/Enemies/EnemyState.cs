using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine stateMachine;
    public EnemyState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void Enter();
    public abstract void Exit();
}
