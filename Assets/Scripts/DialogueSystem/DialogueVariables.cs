using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.Linq;

namespace YuzuValen.DialogueSystem
{
    public class DialogueVariables
    {
        /// <summary>
        /// If a variable is changed in the story, it will be updated here <br/>
        /// We can also set variables here, which will be reflected in the story <br/>
        /// Updating variables while story is running may cause unexpected behavior
        /// </summary>
        public readonly Dictionary<string, Ink.Runtime.Object> variables = new();
        public void StartListening(Story story)
        {
            story.variablesState.variableChangedEvent += ReadVariable;
        }
        public void StopListening(Story story)
        {
            story.variablesState.variableChangedEvent -= ReadVariable;
        }
        private void ReadVariable(string varName, Ink.Runtime.Object value)
        {
            if (variables.ContainsKey(varName) && variables[varName].GetType() != value.GetType())
            {
                Debug.LogError($"Variable {varName} already exists with type {variables[varName].GetType()}, but tried to set with type {value.GetType()}");
                return;
            }
            variables[varName] = value;
        }
        public void UpdateStoryVariables(Story story)
        {
            List<string> keys = story.variablesState.ToList();
            foreach (var key in keys)
            {
                if (variables.ContainsKey(key))
                {
                    var storedValue = variables[key];
                    story.variablesState.SetGlobal(key, storedValue);
                }
            }
        }
    }
}
