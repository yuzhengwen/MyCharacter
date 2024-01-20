using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerStateMachine : MonoBehaviour
{
    private readonly Dictionary<PlayerStateType, PlayerBaseState> states = new();
    private PlayerBaseState currentState;
    private PlayerBaseState previousState;
    private PlayerBaseState defaultState;

    public LayerMask terrain;

    void Awake()
    {
        states.Add(PlayerStateType.Idle, new PlayerIdleState());
        states.Add(PlayerStateType.Jump, new PlayerJumpState());
        states.Add(PlayerStateType.Walk, new PlayerWalkState());

        defaultState = states[PlayerStateType.Idle];
        SetCurrentState(PlayerStateType.Idle);
    }
    void Update()
    {
        currentState.Update();
        currentState.HandleInput();
        //Debug.Log(currentState.GetType().Name);
    }
    public void SetCurrentState(PlayerStateType newStateType)
    {
        previousState = currentState;
        if (states.TryGetValue(newStateType, out currentState))
        {
            currentState.EnterState(this);
        }
        else
            Debug.LogError("State " + newStateType + " does not exist");
    }
}
