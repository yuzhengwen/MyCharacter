using System;
using UnityEngine;
using YuzuValen.HFSM;

namespace YuzuValen
{
    public class JumpState : BasePlayerState
    {
        private Vector2 jumpForce = new(0, 12.0f);
        private Vector2 fallingForce = new(0, -2.0f);
        public JumpState(string id, PlayerMovement player) : base(id, player)
        {
        }

        public override void OnEnter()
        {
            rb.AddForce(jumpForce, ForceMode2D.Impulse);
        }
        public override void OnExit()
        {
            rb.gravityScale = 2.0f;
            rb.AddForce(fallingForce, ForceMode2D.Impulse);
        }
        public override void Init()
        {
            parent.AddTransition(PlayerState.AirborneState.Jumping, PlayerState.AirborneState.Falling, () => rb.velocity.y < 0);
        }
        public override void TriggerEvent(string eventName, EventArgs args=null)
        {
            // increase gravity if jump input is released early for a shorter jump
            if (eventName == "JumpInputReleased")
            {
                rb.gravityScale = 4.0f;
            }
        }
    }
}
