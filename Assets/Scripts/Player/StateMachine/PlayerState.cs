using UnityEngine;

public abstract class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected PlayerController controller;

    public PlayerState(PlayerStateMachine stateMachine, PlayerController controller)
    {
        this.stateMachine = stateMachine;
        this.controller = controller;
    }

    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void Enter();
    public abstract void Exit();


}
