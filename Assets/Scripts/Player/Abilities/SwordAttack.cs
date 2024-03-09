using UnityEngine;

namespace YuzuValen.AbilitySystem
{
    public class SwordAttack : BaseAbility
    {
        private void Start()
        {
            displayName = "Sword Attack";
            cooldown = 0.5f;
            showCooldownTimer = false;
        }
        public override void Use(AbilityController control)
        {
            Debug.Log("Sword Attack");
        }
    }
}
