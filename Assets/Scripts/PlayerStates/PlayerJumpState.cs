using System;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private int initialY;
    private Collider2D collider;
    public override void EnterState(PlayerStateMachine playerStateMachine)
    {
        base.EnterState(playerStateMachine);
        playerRigidbody.AddForce(playerRigidbody.transform.up * 10, UnityEngine.ForceMode2D.Impulse);
        initialY = (int)playerRigidbody.transform.position.y;
        collider = playerObject.GetComponent<BoxCollider2D>();
    }
    public override void HandleInput()
    {
    }

    public override void Update()
    {
        if(playerRigidbody.velocity.y < 0.2f)
            if (GroundCheck())
            {
                playerRigidbody.gravityScale = 1;
                playerStateMachine.SetCurrentState(PlayerStateType.Idle);
                return;
            }
        if (playerRigidbody.gravityScale< 3)
            playerRigidbody.gravityScale = 1+(5*Math.Abs(playerRigidbody.transform.position.y - initialY));
    }

    private bool GroundCheck()
    {
        return Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, 0.1f, playerStateMachine.terrain);
    }
}