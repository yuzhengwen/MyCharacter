using System;
using UnityEngine;
using static QuestInventoryTracker;

[Serializable]
public abstract class Objective

{
    public string objectiveName = "Default Objective Name";

    public ObjectiveState currentState = ObjectiveState.NotStarted;
    private Quest parent;

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
    public virtual void Start(Quest parent)
    {
        this.parent = parent;
        currentState = ObjectiveState.InProgress;
    }

    /// <summary>
    /// Called by self when the objective is completed. 
    /// </summary>
    /// <inheritdoc/>
    protected virtual void Complete()
    {
        Debug.Log($"Objective Complete: {objectiveName}");
        currentState = ObjectiveState.Completed;
        parent.OnObjectiveCompleted(this);
    }
    public virtual Objective DeepCopy(Quest parent)
    {
        Objective newObj =  (Objective)MemberwiseClone();
        newObj.objectiveName = string.Copy(objectiveName);
        newObj.currentState = ObjectiveState.NotStarted;
        newObj.parent = parent;
        return newObj;
    }
}
public enum ObjectiveState
{
    NotStarted,
    InProgress,
    Completed
}

[Serializable]
public class MessageObjective : Objective, IQuestEventResponder
{
    [SerializeField] private string messageToReceive = "Default Message";

    public void OnEventTrigger(string eventName, EventArgs args = null)
    {
        if (eventName == messageToReceive)
            MessageReceived(args);
    }
    protected virtual void MessageReceived(EventArgs args = null)
    {
        Complete();
    }
    public override Objective DeepCopy(Quest parent)
    {
        MessageObjective newObj = (MessageObjective)base.DeepCopy(parent);
        newObj.messageToReceive = string.Copy(messageToReceive);
        return newObj;
    }
}
[Serializable]
public class CounterObjective : MessageObjective
{
    [SerializeField] private int targetCount = 1;
    private int currentCount = 0;

    public override void Start(Quest parent)
    {
        base.Start(parent);
        currentCount = 0;
    }
    protected override void MessageReceived(EventArgs args = null)
    {
        int amount = (args as IntEventArgs)?.value ?? 1;
        Increment(amount);
    }
    public void Increment(int amount)
    {
        currentCount += amount;
        if (currentCount >= targetCount)
        {
            currentCount = targetCount;
            Complete();
        }
    }
    public override Objective DeepCopy(Quest parent)
    {
        CounterObjective newObj = (CounterObjective)base.DeepCopy(parent);
        newObj.targetCount = targetCount;
        newObj.currentCount = 0;
        return newObj;
    }
}
[Serializable]
public class TimerObjective : Objective
{
    [SerializeField] private float targetTime = 30f;
    private float currentTime = 0.0f;
    [SerializeField] private ObjectiveTimer timer;

    public override void Start(Quest parent)
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
    public override Objective DeepCopy(Quest parent)
    {
        TimerObjective newObj = (TimerObjective)base.DeepCopy(parent);
        newObj.targetTime = targetTime;
        newObj.currentTime = 0.0f;
        return newObj;
    }
}
[Serializable]
public class TalkToNPCObjective : Objective
{
    // QuestNPC class is responsible for completing this objective
    [SerializeField] private QuestNPC npc;

    public override void Start(Quest parent)
    {
        base.Start(parent);
        npc.Activate(this);
    }
    public void TalkedToNPC()
    {
        Complete();
    }
    public override Objective DeepCopy(Quest parent)
    {
        TalkToNPCObjective newObj = (TalkToNPCObjective)base.DeepCopy(parent);
        newObj.npc = npc;
        return newObj;
    }
}
