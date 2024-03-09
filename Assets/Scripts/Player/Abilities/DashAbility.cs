using YuzuValen.Utils;
using System.Collections;
using UnityEngine;

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
            displayName = "Fire Ball";
            cooldown = 3;
            timer = new();
        }
        public override void Use(AbilityController control)
        {
            Debug.Log("Dash");
            player = control.GetComponentInParent<PlayerMovement>();
            DashStart(player);
        }

        private void DashStart(PlayerMovement player)
        {
            player.speedMultiplier = 2;
            dashing = true;
            timer.Start();
        }
        private void DashEnd(PlayerMovement player)
        {
            player.speedMultiplier = 1;
            dashing = false;
        }
        protected override void AbilityUpdate()
        {
            Debug.Log($"DELTA: {Time.deltaTime}, TIMER: {timer.GetTimeElapsed()}: Cancel dash - {timer.GetTimeElapsed() >= dashDuration}, Dash DUr: {dashDuration}");
            if (dashing && timer.GetTimeElapsed() >= dashDuration)
            {
                DashEnd(player);
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
