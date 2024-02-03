using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class BaseQuest
{
    #region Static Quest Info
    public string questName;
    public int questId;

    protected QuestAction[] requirements;
    protected QuestAction[] questSteps;
    public QuestReward[] rewards { get; protected set; }
    #endregion

    #region Events
    public event Action<QuestState> OnStateChanged;
    /// <summary>
    /// Called after a step is completed but quest is not entirely completed
    /// </summary>
    /// <inheritdoc/>
    public event Action<QuestAction, QuestAction> OnProgressed;
    /// <summary>
    /// Called after quest is entirely completed
    /// </summary>
    /// <inheritdoc/>
    public event Action<BaseQuest> OnCompleted;
    #endregion

    protected QuestState state = QuestState.Unavailable;
    private bool autoStart = false;

    private int requirementsCompleted = 0;
    private QuestAction currentStep, prevStep;
    private int currentStepIndex = 0;

    protected readonly QuestManager questManager;

    /// <param name="autoStart">Auto Start: Quest will start automatically when requirements are met. Default: true</param>
    public BaseQuest(QuestManager questManager, bool autoStart = true)
    {
        this.questManager = questManager;
        this.autoStart = autoStart;

        AddActions();
        foreach (QuestAction action in requirements)
            action.OnComplete += RequirementCompleted;
    }
    protected abstract void AddActions();

    private void RequirementCompleted(QuestAction questAction)
    {
        questAction.OnComplete -= RequirementCompleted;
        requirementsCompleted++;
        if (requirementsCompleted >= requirements.Length)
            if (autoStart)
                StartQuest();
            else
                SetState(QuestState.CanStart);
    }


    /// <summary>
    /// Called when quest is started
    /// </summary>
    /// <inheritdoc/>
    public virtual void StartQuest()
    {
        SetState(QuestState.InProgress);
        currentStepIndex = 0;
        currentStep = questSteps[currentStepIndex];
        StartStep(currentStep);
        UnityEngine.Debug.Log("Quest Started: " + questName);
    }

    /// <summary>
    /// Called when current step is completed. If there are no more steps, the quest is completed. Otherwise, the next step is started.
    /// </summary>
    /// <inheritdoc/>
    protected virtual void ProgressQuest(QuestAction questAction)
    {
        currentStepIndex++;
        // check if no more steps left
        if (currentStepIndex >= questSteps.Length)
        {
            CompleteQuest();
            return;
        }
        // progress to next step
        prevStep = currentStep;
        currentStep = questSteps[currentStepIndex];
        StartStep(currentStep, prevStep);
        OnProgressed?.Invoke(prevStep, currentStep);
    }

    /// <summary>
    /// Starts the next step
    /// </summary>
    /// <inheritdoc/>
    protected virtual void StartStep(QuestAction next, QuestAction prev = null)
    {
        if (prev != null)
            prev.OnComplete -= ProgressQuest;
        currentStep.Start();
        currentStep.OnComplete += ProgressQuest;
    }

    /// <summary>
    /// Called when quest is entirely completed
    /// </summary>
    /// <inheritdoc/>
    protected virtual void CompleteQuest()
    {
        SetState(QuestState.Completed);
        currentStep.OnComplete -= ProgressQuest;
        OnCompleted?.Invoke(this);
    }

    public void SetState(QuestState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(state);
    }
    public QuestState GetState() { return state; }
}
public enum QuestState
{
    /// <summary>
    /// Requirements not met. Cannot start quest
    /// </summary>
    Unavailable,
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
