using UnityEngine;

namespace YuzuValen.AbilitySystem
{
    public class FireBallAbility : BaseAbility
    {
        private void Start()
        {
            displayName = "Fire Ball";
            cooldown = 5;
        }

        public override void Use(AbilityController controller)
        {
            Debug.Log("Fire Ball used");
            GoOnCooldown();
        }
    }
}
