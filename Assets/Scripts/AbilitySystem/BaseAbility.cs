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
        public bool showCooldownTimer = true;
        public event Action OnCooldownStarted;
        public event Action OnCooldownFinished;

        /// <summary>
        /// Allows next ability to be used if true <br/>
        /// If false, next ability will be queued and executed only when current ability allows exit <br/>
        /// Can use animation event to set this to true again
        /// </summary>
        public bool canExit = true;

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
            AbilityUpdate();
        }
        protected virtual void AbilityUpdate() { }
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
