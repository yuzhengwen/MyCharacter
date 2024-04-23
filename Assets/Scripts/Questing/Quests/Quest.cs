using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    public QuestData data;
    [SerializeReference] public Objective[] requirements;
    [SerializeReference] public Objective[] questSteps;
    private int curRequirementIndex = 0;
    private int curStepIndex = 0;
    public Objective CurrentObj { get; protected set; }

    #region Static Events
    /// <summary>
    /// Called when quest state changes <br/>
    /// E.g. From Unavailable to CanStart
    /// </summary>
    public static event Action<Quest, QuestState> OnStateChanged;
    /// <summary>
    /// Called after an objective is completed
    /// </summary>
    /// <inheritdoc/>
    public static event Action<Quest, Objective> OnProgressed;
    /// <summary>
    /// Called after quest is entirely completed
    /// </summary>
    /// <inheritdoc/>
    public static event Action<Quest> OnComplete;
    #endregion

    public QuestState currentState = QuestState.RequirementsNotMet;

    public Quest(QuestData data)
    {
        this.data = data;
        requirements = data.requirements;
        questSteps = data.questSteps;
        requirements = Array.ConvertAll(requirements, (obj) => obj.DeepCopy(this));
        questSteps = Array.ConvertAll(questSteps, (obj) => obj.DeepCopy(this));
    }
    /// <summary>
    /// Initialize quest <br/>
    /// If there are requirements, quest will be in RequirementsNotMet state <br/>
    /// If requirements are met, the quest will be in either CanStart or InProgress state (depending on autoStart)
    /// </summary>
    public void Init()
    {
        curRequirementIndex = 0;
        curStepIndex = 0;
        if (requirements.Length > 0)
        {
            CurrentObj = requirements[curRequirementIndex];
            currentState = QuestState.RequirementsNotMet;
            foreach (Objective obj in requirements)
                obj.Init();
        }
        else
            RequirementsComplete();
        foreach (Objective obj in questSteps)
            obj.Init();
        CurrentObj.Start(this);
    }

    /// <summary>
    /// Called when quest is started (requirements are met)
    /// </summary>
    /// <inheritdoc/>
    public virtual void Start()
    {
        currentState = QuestState.InProgress;
        CurrentObj = questSteps[curStepIndex];
        CurrentObj.Start(this);
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
        OnProgressed?.Invoke(this, completed);
        curRequirementIndex++;
        // check if no more requirements left
        if (curRequirementIndex >= requirements.Length)
        {
            RequirementsComplete();
            return;
        }
        // progress to next requirement objective
        CurrentObj = requirements[curRequirementIndex];
        CurrentObj.Start(this);
    }
    private void RequirementsComplete()
    {
        if (data.autoStart)
            Start();
        else
            SetState(QuestState.CanStart);
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
        OnProgressed?.Invoke(this,completed);
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
        (CurrentObj as IQuestEventResponder)?.OnEventTrigger(eventName, args);
    }
    public void SetState(QuestState state)
    {
        currentState = state;
        OnStateChanged?.Invoke(this, state);
    }
}
