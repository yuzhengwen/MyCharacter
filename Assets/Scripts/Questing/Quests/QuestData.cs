using System;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest", order = 1)]
public class QuestData : ScriptableObject
{
    public string questName;
    public int id;

    [SerializeReference, SubclassSelector] public Objective[] requirements;
    [SerializeReference, SubclassSelector] public Objective[] questSteps;
    public Reward[] rewards;

    public bool autoStart = false;

}
public enum QuestState
{
    /// <summary>
    /// Requirements not met. Cannot start quest
    /// </summary>
    RequirementsNotMet,
    /// <summary>
    /// Requirements met. Can start quest
    /// </summary>
    CanStart,
    /// <summary>
    /// In progress (started but not completed)
    /// </summary>
    InProgress,
    /// <summary>
    /// All steps completed
    /// </summary>
    Completed,
    Failed
}
