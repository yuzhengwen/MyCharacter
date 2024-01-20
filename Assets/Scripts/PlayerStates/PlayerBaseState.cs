using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    public bool canTransition {get; private set;} = true;
    protected PlayerStateMachine playerStateMachine;
    protected GameObject playerObject;
    protected Rigidbody2D playerRigidbody;

    public virtual void EnterState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerObject = playerStateMachine.gameObject;
        playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
    }
    public abstract void Update();

    public abstract void HandleInput();
}
