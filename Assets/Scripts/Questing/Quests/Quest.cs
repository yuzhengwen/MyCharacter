using System;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest", order = 1)]
public class Quest : ScriptableObject
{
    #region Static Quest Info
    public string questName;
    public int id;

    [SerializeReference, SubclassSelector] public Objective[] requirements;
    [SerializeReference, SubclassSelector] public Objective[] questSteps;
    public Reward[] rewards;

    public bool autoStart = false;
    #endregion

    private int curRequirementIndex = 0;
    private int curStepIndex = 0;
    public Objective CurrentObj { get; protected set; }

    #region Events
    /// <summary>
    /// Called when quest state changes <br/>
    /// E.g. From Unavailable to CanStart
    /// </summary>
    public event Action<QuestState> OnStateChanged;
    /// <summary>
    /// Called after an objective is completed
    /// </summary>
    /// <inheritdoc/>
    public event Action<Objective> OnProgressed;
    /// <summary>
    /// Called after quest is entirely completed
    /// </summary>
    /// <inheritdoc/>
    public event Action<Quest> OnComplete;
    #endregion

    public QuestState currentState = QuestState.RequirementsNotMet;

    /// <summary>
    /// Called when quest is started
    /// </summary>
    /// <inheritdoc/>
    public virtual void Start()
    {
        currentState = QuestState.InProgress;
        CurrentObj = questSteps[curStepIndex];
        CurrentObj.Start(this);
        UnityEngine.Debug.Log("Quest Started: " + name);
    }
    /// <summary>
    /// Called when quest is entirely completed
    /// </summary>
    /// <inheritdoc/>
    public virtual void Complete()
    {
        currentState = QuestState.Completed;
        OnComplete?.Invoke(this);
    }

    #region Quest Objectives Progression
    public void OnObjectiveCompleted(Objective completed)
    {
        if (currentState == QuestState.RequirementsNotMet)
            ProgressRequirement(completed);
        else if (currentState == QuestState.InProgress)
            ProgressQuest(completed);
    }
    protected virtual void ProgressRequirement(Objective completed)
    {
        if (completed != requirements[curRequirementIndex])
        {
            UnityEngine.Debug.LogError($"Current Requirement is not {completed.objectiveName}");
            return;
        }
        OnProgressed?.Invoke(completed);
        curRequirementIndex++;
        if (curRequirementIndex >= requirements.Length)
            if (autoStart)
                Start();
            else
                currentState = QuestState.CanStart;
    }
    /// <summary>
    /// Called when current step is completed. If there are no more steps, the quest is completed. Otherwise, the next step is started.
    /// </summary>
    /// <inheritdoc/>
    protected virtual void ProgressQuest(Objective completed)
    {
        if (completed != questSteps[curStepIndex])
        {
            UnityEngine.Debug.LogError($"Current step is not {completed.objectiveName}");
            return;
        }
        OnProgressed?.Invoke(completed);
        curStepIndex++;
        // check if no more steps left
        if (curStepIndex >= questSteps.Length)
        {
            Complete();
            return;
        }
        // progress to next step
        CurrentObj = questSteps[curStepIndex];
        CurrentObj.Start(this);
    }
    #endregion

    public void TriggerEvent(string eventName, EventArgs args = null)
    {
        if (currentState == QuestState.RequirementsNotMet)
        {
            IQuestEventResponder[] responders = Array.FindAll(requirements,
                               (obj) => obj.currentState == ObjectiveState.InProgress && obj is IQuestEventResponder).Cast<IQuestEventResponder>().ToArray();

            foreach (IQuestEventResponder responder in responders)
                responder.OnEventTrigger(eventName, args);
        }
        else if (currentState == QuestState.InProgress)
        {
            IQuestEventResponder responder = CurrentObj as IQuestEventResponder;
            responder?.OnEventTrigger(eventName, args);
        }
    }
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
public interface IQuestEventResponder
{
    void OnEventTrigger(string eventName, EventArgs args = null);
}
