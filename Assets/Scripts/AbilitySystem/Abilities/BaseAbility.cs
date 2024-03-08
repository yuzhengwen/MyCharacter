using System;
using UnityEngine;

namespace YuzuValen.AbilitySystem
{
    public abstract class BaseAbility : MonoBehaviour
    {
        public Sprite icon;
        public string displayName;
        public string description;

        public float cooldown = 0;
        public float cooldownTimer = 0;
        public event Action OnCooldownStarted;
        public event Action OnCooldownFinished;

        private void Update()
        {
            if (IsOnCooldown())
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    OnCooldownFinished?.Invoke();
                    cooldownTimer = 0;
                }
            }
        }
        public void GoOnCooldown()
        {
            cooldownTimer = cooldown;
            OnCooldownStarted?.Invoke();
        }
        public bool IsOnCooldown()
        {
            return cooldownTimer > 0;
        }
        public bool TryUse(AbilityController control)
        {
            if (CanUse())
            {
                Use(control);
                if (cooldown > 0)
                    GoOnCooldown();
                return true;
            }
            return false;
        }

        public virtual bool CanUse()
        {
            return !IsOnCooldown();
        }
        public abstract void Use(AbilityController control);
    }
}
