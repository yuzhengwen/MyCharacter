using System.Collections;
using UnityEngine;
using YuzuValen.Utils;

namespace YuzuValen.AbilitySystem
{
    public class DashAbility : BaseAbility
    {
        //public float dashDistance = 5;
        public float dashDuration = 0.2f;
        private PlayerMovement player;
        private bool dashing = false;
        private Timer timer;
        private void Start()
        {
            displayName = "Dash";
            cooldown = 3;
        }
        public override void Use(AbilityController control)
        {
            player = control.GetComponentInParent<PlayerMovement>();
            DashStart(player);
        }

        private void DashStart(PlayerMovement player)
        {
            player.speedMultiplier = 2;
            dashing = true;
        }
        private void DashEnd(PlayerMovement player)
        {
            player.speedMultiplier = 1;
            dashing = false;
        }
        protected override void AbilityUpdate()
        {
            if (dashing)
            {
                if (timer == null)
                {
                    timer = new Timer(dashDuration);
                    timer.OnComplete += () => DashEnd(player);
                    timer.Restart();
                }
                timer.Update();
            }
        }
        private void LateUpdate()
        {
            if (player != null && dashing)
            {
                player.moveDir = player.facingDirection;
            }
        }
    }
}
