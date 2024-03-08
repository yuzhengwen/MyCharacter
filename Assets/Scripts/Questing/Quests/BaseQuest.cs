using System;

[Serializable]
public abstract class BaseQuest : IObjective
{
    #region Static Quest Info
    public string name;
    public int id;

    public BaseObjective[] requirements;
    public BaseObjective[] questSteps;

    private int curRequirementIndex = 0;
    private int curStepIndex = 0;
    public BaseObjective CurrentStep { get; protected set; }
    public QuestReward[] rewards { get; protected set; }
    #endregion

    #region Events
    /// <summary>
    /// Called when quest state changes <br/>
    /// E.g. From Unavailable to CanStart
    /// </summary>
    public event Action<ObjectiveState> OnStateChanged;
    /// <summary>
    /// Called after an objective is completed
    /// </summary>
    /// <inheritdoc/>
    public event Action<BaseObjective> OnProgress;
    /// <summary>
    /// Called after quest is entirely completed
    /// </summary>
    /// <inheritdoc/>
    public event Action<BaseQuest> OnComplete;
    #endregion

    private ObjectiveState currentState = ObjectiveState.RequirementsNotMet;
    // A read-write instance property:
    public ObjectiveState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            OnStateChanged?.Invoke(currentState);
        }
    }

    private readonly bool autoStart = false;


    protected readonly QuestManager questManager;

    /// <param name="autoStart">Auto Start: Quest will start automatically when requirements are met. Default: true</param>
    public BaseQuest(QuestManager questManager, bool autoStart = true)
    {
        this.questManager = questManager;
        this.autoStart = autoStart;

        AddActions();
    }
    /// <summary>
    /// Add Quest Steps and Requirements
    /// </summary>
    protected abstract void AddActions();

    /// <summary>
    /// Called when quest is started
    /// </summary>
    /// <inheritdoc/>
    public virtual void Start()
    {
        CurrentState = ObjectiveState.InProgress;
        CurrentStep = questSteps[curStepIndex];
        CurrentStep.Start();
        UnityEngine.Debug.Log("Quest Started: " + name);
    }
    /// <summary>
    /// Called when quest is entirely completed
    /// </summary>
    /// <inheritdoc/>
    public virtual void Complete()
    {
        CurrentState = ObjectiveState.Completed;
        OnComplete?.Invoke(this);
    }

    #region Quest Objectives Progression
    public void OnObjectiveCompleted(BaseObjective completed)
    {
        if (CurrentState == ObjectiveState.RequirementsNotMet)
            ProgressRequirement(completed);
        else if (CurrentState == ObjectiveState.InProgress)
            ProgressQuest(completed);
    }
    protected virtual void ProgressRequirement(BaseObjective completed)
    {
        if (completed != requirements[curRequirementIndex])
        {
            UnityEngine.Debug.LogError($"Current Requirement is not {completed.name}");
            return;
        }
        OnProgress?.Invoke(completed);
        curRequirementIndex++;
        if (curRequirementIndex >= requirements.Length)
            if (autoStart)
                Start();
            else
                CurrentState = ObjectiveState.CanStart;
    }
    /// <summary>
    /// Called when current step is completed. If there are no more steps, the quest is completed. Otherwise, the next step is started.
    /// </summary>
    /// <inheritdoc/>
    protected virtual void ProgressQuest(BaseObjective completed)
    {
        if (completed != questSteps[curStepIndex])
        {
            UnityEngine.Debug.LogError($"Current step is not {completed.name}");
            return;
        }
        OnProgress?.Invoke(completed);
        curStepIndex++;
        // check if no more steps left
        if (curStepIndex >= questSteps.Length)
        {
            Complete();
            return;
        }
        // progress to next step
        CurrentStep = questSteps[curStepIndex];
        CurrentStep.Start();
    }
    #endregion
}
