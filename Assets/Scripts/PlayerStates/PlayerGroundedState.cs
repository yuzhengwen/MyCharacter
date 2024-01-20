using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public override void EnterState(PlayerStateMachine playerStateMachine)
    {
        base.EnterState(playerStateMachine);

    }
    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerStateMachine.SetCurrentState(PlayerStateType.Jump);
            return;
        }else
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            playerStateMachine.SetCurrentState(PlayerStateType.Idle);
        }
        else
        {
            playerStateMachine.SetCurrentState(PlayerStateType.Walk);
        }
    }
    public override void Update()
    {
    }
}