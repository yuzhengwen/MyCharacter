using System;
using UnityEngine;

[Serializable]
public abstract class Objective
{
    public string objectiveName = "Default Objective Name";

    public ObjectiveState currentState = ObjectiveState.NotStarted;
    private Quest parent;

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
    public virtual void Complete()
    {
        currentState = ObjectiveState.Completed;
        parent.OnObjectiveCompleted(this);
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
            Complete();
    }
}
[Serializable]
public class CounterObjective : MessageObjective
{
    [SerializeField] private int targetCount = 1;
    [SerializeField] private int incrementAmount = 1;
    private int currentCount = 0;

    public override void Start(Quest parent)
    {
        base.Start(parent);
        currentCount = 0;
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
}
