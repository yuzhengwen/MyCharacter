using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzuValen.AbilitySystem
{
    public class AbilityController : MonoBehaviour
    {
        public List<BaseAbility> abilities = new();
        public List<UI_Ability> uiAbilities = new();

        void Start()
        {
            for (int i = 0; i < abilities.Count; i++)
            {
                uiAbilities[i].AssignAbility(this, abilities[i]);
            }
        }
        public void UseAbility(int index)
        {
            abilities[index].TryUse(this);
        }
    }
}
