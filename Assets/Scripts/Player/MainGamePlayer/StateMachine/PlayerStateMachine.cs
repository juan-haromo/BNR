using UnityEngine;

public class PlayerStateMachine
{
    private PlayerState currentState;
    public PlayerState CurrentState => currentState;
    private PlayerController controller;
    
    public PlayerStateMachine(PlayerController controller) 
    {
        this.controller = controller;
    }

    public void Initialize(PlayerState startState)
    {
        currentState = startState;
        startState.Enter();
    } 

    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
