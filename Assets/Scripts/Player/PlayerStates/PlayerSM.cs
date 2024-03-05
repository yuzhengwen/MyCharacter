using System;
using UnityEngine;
using YuzuValen.HFSM;

namespace YuzuValen
{
    public class PlayerSM : StateMachine<string>
    {
        protected PlayerMovement player;
        protected Rigidbody2D rb;
        protected PlayerInputHandler inputHandler;
        public PlayerSM(string id, PlayerMovement player) : base(id)
        {
            this.player = player;
            rb = player.GetComponent<Rigidbody2D>();
            inputHandler = player.GetComponent<PlayerInputHandler>();
        }
    }
}
