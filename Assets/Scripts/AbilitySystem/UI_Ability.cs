
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YuzuValen.AbilitySystem
{
    public class UI_Ability : MonoBehaviour, IPointerClickHandler
    {
        private BaseAbility ability;

        private Image icon;
        private TextMeshProUGUI cdText;
        private AbilityController controller;

        private void Awake()
        {
            icon = transform.Find("Icon").GetComponent<Image>();
            cdText = GetComponentInChildren<TextMeshProUGUI>();
            // Create a new material instance to change the saturation of the icon
            icon.material = Instantiate(icon.material);
        }

        public void AssignAbility(AbilityController controller, BaseAbility ability)
        {
            this.controller = controller;
            this.ability = ability;
            icon.sprite = ability.icon;
            cdText.text = "";
            ability.OnCooldownFinished += OnCooldownFinished;
            ability.OnCooldownStarted += OnCooldownStarted;
        }

        private void Update()
        {
            if (ability.IsOnCooldown())
            {
                UpdateCooldown();
            }
        }

        private void UpdateCooldown()
        {
            if (ability.showCooldownTimer)
                cdText.text = Mathf.CeilToInt(ability.cooldownTimer).ToString();
        }
        private void OnCooldownStarted()
        {
            icon.material.SetFloat("_Saturation", 0);
        }
        private void OnCooldownFinished()
        {
            cdText.text = "";
            icon.material.SetFloat("_Saturation", 1);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ability.TryUse(controller);
        }
    }
}
