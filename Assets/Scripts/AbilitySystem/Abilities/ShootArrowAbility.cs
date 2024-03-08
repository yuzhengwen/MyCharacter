using UnityEngine;

namespace YuzuValen.AbilitySystem
{
    public class ShootArrowAbility : BaseAbility
    {
        private void Start()
        {
            displayName = "Shoot Arrow";
            cooldown = 3;
        }

        public override void Use(AbilityController controller)
        {
            Debug.Log("Arrow has shot");
            GoOnCooldown();
        }
    }
}
