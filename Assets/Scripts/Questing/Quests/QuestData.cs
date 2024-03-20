using System;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest", order = 1)]
public class QuestData : ScriptableObject
{
    #region Static Quest Info
    public string questName;
    public int id;

    [SerializeReference, SubclassSelector] public ObjectiveData[] requirements;
    [SerializeReference, SubclassSelector] public ObjectiveData[] questSteps;
    public Reward[] rewards;

    public bool autoStart = false;
    #endregion

    private int curRequirementIndex = 0;
    private int curStepIndex = 0;
    public ObjectiveData CurrentObj { get; protected set; }

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
    public event Action<ObjectiveData> OnProgressed;
    /// <summary>
    /// Called after quest is entirely completed
    /// </summary>
    /// <inheritdoc/>
    public event Action<QuestData> OnComplete;
    #endregion

    public QuestState currentState = QuestState.RequirementsNotMet;

    /// <summary>
    /// Resets the quest to its initial state
    /// In future, this will be used to load quest from save data
    /// </summary>
    public void Init()
    {
        curRequirementIndex = 0;
        curStepIndex = 0;
        CurrentObj = requirements[curRequirementIndex];
        currentState = QuestState.RequirementsNotMet;
        foreach (ObjectiveData obj in requirements)
            obj.Init();
        foreach (ObjectiveData obj in questSteps)
            obj.Init();
        CurrentObj.Start(this);
    }

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
        SetState(QuestState.Completed);
        OnComplete?.Invoke(this);
    }

    #region Quest Objectives Progression
    public void OnObjectiveCompleted(ObjectiveData completed)
    {
        if (currentState == QuestState.RequirementsNotMet)
            ProgressRequirement(completed);
        else if (currentState == QuestState.InProgress)
            ProgressQuest(completed);
    }
    protected virtual void ProgressRequirement(ObjectiveData completed)
    {
        if (completed != requirements[curRequirementIndex])
        {
            UnityEngine.Debug.LogError($"Current Requirement is not {completed.objectiveName}");
            return;
        }
        OnProgressed?.Invoke(completed);
        curRequirementIndex++;
        // check if no more requirements left
        if (curRequirementIndex >= requirements.Length)
        {
            if (autoStart)
                Start();
            else
                SetState(QuestState.CanStart);
            return;
        }
        // progress to next requirement objective
        CurrentObj = requirements[curRequirementIndex];
        CurrentObj.Start(this);
    }
    /// <summary>
    /// Called when current step is completed. If there are no more steps, the quest is completed. Otherwise, the next step is started.
    /// </summary>
    /// <inheritdoc/>
    protected virtual void ProgressQuest(ObjectiveData completed)
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

    /// <summary>
    /// Messages will be sent to the current objective
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="args"></param>
    public void TriggerEvent(string eventName, EventArgs args = null)
    {
        Debug.Log($"Event({eventName}) triggered on Quest: {questName}");
        //if (currentState == QuestState.RequirementsNotMet)
        //{
        //    IQuestEventResponder[] responders = Array.FindAll(requirements,
        //                       (obj) => obj.currentState == ObjectiveState.InProgress && obj is IQuestEventResponder).Cast<IQuestEventResponder>().ToArray();

        //    foreach (IQuestEventResponder responder in responders)
        //        responder.OnEventTrigger(eventName, args);
        //}
        //else if (currentState == QuestState.InProgress)
        //{
        //    IQuestEventResponder responder = CurrentObj as IQuestEventResponder;
        //    responder?.OnEventTrigger(eventName, args);
        //}
        (CurrentObj as IQuestEventResponder)?.OnEventTrigger(eventName, args);
    }
    public void SetState(QuestState state)
    {
        currentState = state;
        OnStateChanged?.Invoke(state);
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
