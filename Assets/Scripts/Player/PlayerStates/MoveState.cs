using System;
using UnityEngine;
using YuzuValen;
using YuzuValen.HFSM;

public class MoveState<TStateId> : BaseState<TStateId>
{
    PlayerMovement player;
    Rigidbody2D rb;
    public MoveState(PlayerMovement player) : base()
    {
        this.player = player;
        rb = player.GetComponent<Rigidbody2D>();
    }
}
