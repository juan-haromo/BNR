using UnityEngine;

public class EnemyStateMachine
{
    public EnemyController Controller {  get; private set; }
    public EnemyState CurrentState { get; protected set; }
    public EnemyStateMachine(EnemyController controller)
    {
        Controller = controller;
    }
    public void ChangeState(EnemyState newState)
    {
        if (newState ==  CurrentState) { return; }
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
    public void Initialize(EnemyState initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }
}
