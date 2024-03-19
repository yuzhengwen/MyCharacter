using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : MonoBehaviour
{
    private TalkToNPCObjective obj;
    public QuestNPCState state = QuestNPCState.Inactive; // used to display UI elements

    // UI
    [SerializeField] private SpriteRenderer indicator;
    private void Start()
    {
        UpdateIndicator(state);
    }

    private void UpdateIndicator(QuestNPCState state)
    {
        switch (state)
        {
            case QuestNPCState.Inactive:
            case QuestNPCState.Completed:
                indicator.enabled = false;
                break;
            case QuestNPCState.ReadyToStart:
                indicator.enabled = true;
                indicator.color = Color.yellow;
                break;
            case QuestNPCState.InProgress:
                indicator.color = Color.blue;
                break;
            case QuestNPCState.ReadyToComplete:
                indicator.color = Color.green;
                break;
        }
    }
    public void SetState(QuestNPCState state)
    {
        this.state = state;
        UpdateIndicator(state);
    }

    public void Activate(TalkToNPCObjective obj)
    {
        this.obj = obj;
        SetState(QuestNPCState.ReadyToStart);
    }

    public void CompleteObjective()
    {
        obj.Complete();
        SetState(QuestNPCState.Completed);
    }
}
public enum QuestNPCState
{
    Inactive, ReadyToStart, InProgress, ReadyToComplete, Completed
}
