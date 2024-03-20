using System;
using UnityEngine;

[Serializable]
public abstract class ObjectiveData
{
    public string objectiveName = "Default Objective Name";

    public ObjectiveState currentState = ObjectiveState.NotStarted;
    private QuestData parent;

    /// <summary>
    /// Reset the objective to its initial state
    /// In future, this will be used to load objective from save data
    /// </summary>
    public void Init()
    {
        currentState = ObjectiveState.NotStarted;
    }
    /// <summary>
    /// Called when Quest starts this step. 
    /// </summary>
    /// <inheritdoc/>
    public virtual void Start(QuestData parent)
    {
        this.parent = parent;
        currentState = ObjectiveState.InProgress;
    }

    /// <summary>
    /// Called by self when the objective is completed. 
    /// </summary>
    /// <inheritdoc/>
    public virtual void Complete()
    {
        currentState = ObjectiveState.Completed;
        parent.OnObjectiveCompleted(this);
        Debug.Log($"{objectiveName} completed");
    }
}
public enum ObjectiveState
{
    NotStarted,
    InProgress,
    Completed
}

[Serializable]
public class MessageObjective : ObjectiveData, IQuestEventResponder
{
    [SerializeField] private string messageToReceive = "Default Message";

    public void OnEventTrigger(string eventName, EventArgs args = null)
    {
        Debug.Log($"Message Received({eventName}) on Objective: {objectiveName}");
        if (eventName == messageToReceive)
            MessageReceived(args);
    }
    protected virtual void MessageReceived(EventArgs args = null)
    {
        Complete();
    }
}
[Serializable]
public class CounterObjective : MessageObjective
{
    [SerializeField] private int targetCount = 1;
    [SerializeField] private int incrementAmount = 1;
    private int currentCount = 0;

    public override void Start(QuestData parent)
    {
        base.Start(parent);
        currentCount = 0;
    }
    protected override void MessageReceived(EventArgs args = null)
    {
        Increment();
    }
    public void Increment()
    {
        currentCount += incrementAmount;
        if (currentCount >= targetCount)
        {
            currentCount = targetCount;
            Complete();
        }
    }
}
[Serializable]
public class TimerObjective : ObjectiveData
{
    [SerializeField] private float targetTime = 30f;
    private float currentTime = 0.0f;
    [SerializeField] private ObjectiveTimer timer;

    public override void Start(QuestData parent)
    {
        base.Start(parent);
        currentTime = 0.0f;
        timer.OnUpdate += Update;
    }

    public void Update(float deltaTime)
    {
        currentTime += deltaTime;
        if (currentTime >= targetTime)
        {
            currentTime = targetTime;
            timer.OnUpdate -= Update;
            Complete();
        }
    }
}
[Serializable]
public class TalkToNPCObjective : ObjectiveData
{
    // QuestNPC class is responsible for completing this objective
    [SerializeField] private QuestNPC npc;

    public override void Start(QuestData parent)
    {
        base.Start(parent);
        npc.Activate(this);
    }
}
