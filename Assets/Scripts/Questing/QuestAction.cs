using System;
using UnityEngine;

[Serializable]
public abstract class QuestAction
{
    #region Events
    /// <summary>
    /// Called after step is completed
    /// </summary>
    /// <inheritdoc/>
    public event Action<QuestAction> OnComplete;
    /// <summary>
    /// Provides references to useful objects
    /// </summary>
    /// <inheritdoc/>
    #endregion

    protected readonly QuestManager questManager;

    public QuestAction(QuestManager questManager)
    {
        this.questManager = questManager;
    }

    /// <summary>
    /// Called when the action is completed. Unsubscribe from events here.
    /// </summary>
    /// <inheritdoc/>
    protected virtual void Complete()
    {
        OnComplete?.Invoke(this);
    }

    /// <summary>
    /// Called when Quest starts this step. Subscribe to events here.
    /// </summary>
    /// <inheritdoc/>
    public abstract void Start();
}