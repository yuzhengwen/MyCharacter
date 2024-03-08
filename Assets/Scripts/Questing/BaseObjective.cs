using System;
using UnityEngine;

[Serializable]
public abstract class BaseObjective : IObjective
{
    public readonly string name;
    private readonly BaseQuest parent;
    public bool IsComplete { get; protected set; } = false;

    public BaseObjective(BaseQuest parent, string name = "Default Objective Name")
    {
        this.parent = parent;
        this.name = name;
    }

    /// <summary>
    /// Called when Quest starts this step. Subscribe to events here.
    /// </summary>
    /// <inheritdoc/>
    public virtual void Start()
    {
    }

    /// <summary>
    /// Called by self when the objective is completed. Unsubscribe from events here.
    /// </summary>
    /// <inheritdoc/>
    public virtual void Complete()
    {
        parent?.OnObjectiveCompleted(this);
        IsComplete = true;
    }
}
public interface IObjective
{
    void Start();
    void Complete();
}