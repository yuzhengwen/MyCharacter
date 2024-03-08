using UnityEngine;

namespace YuzuValen.AbilitySystem
{
    public class BlackHoleAbility : BaseAbility
    {
        private void Start()
        {
            displayName = "Black Hole";
            cooldown = 15;
        }
        public override void Use(AbilityController controller)
        {
            GoOnCooldown();
            Debug.Log("Black Hole Ability Used");
        }
    }
}
