using UnityEngine;

public class PlayerWalkState : PlayerGroundedState
{
    private float speed = 5;

    public override void Update()
    {
        base.Update();
        playerRigidbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, playerRigidbody.velocity.y);
    }
}