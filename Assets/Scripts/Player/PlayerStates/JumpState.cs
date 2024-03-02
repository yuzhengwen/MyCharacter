using UnityEngine;
using YuzuValen;
using YuzuValen.HFSM;

public class JumpState<TStateId> : BaseState<TStateId>
{
    private PlayerMovement player;
    private readonly Rigidbody2D rb;

    private Vector2 jumpForce = new(0, 12.0f);
    private Vector2 fallingForce = new(0, -4.0f);
    public JumpState(PlayerMovement player) : base()
    {
        this.player = player;
        rb = player.GetComponent<Rigidbody2D>();
    }
    public override void OnEnter()
    {
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
    }
    public override void OnExit()
    {
        rb.AddForce(fallingForce, ForceMode2D.Impulse);
    }
}
