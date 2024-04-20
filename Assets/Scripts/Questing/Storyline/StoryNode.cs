using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryNode
{
    public string title = "New Story Node";
    public string description = "";
    public StoryIndex index;
    public StoryNode next;
    public QuestData[] mainQuestData;
    private int questIndex = 0;

    public NodeState state = NodeState.NotReached;

    public QuestData NextQuest(out bool isComplete)
    {
        questIndex++;
        if (questIndex >= mainQuestData.Length)
        {
            isComplete = true;
            state = NodeState.Complete;
            return null;
        }
        isComplete = false;
        return mainQuestData[questIndex];
    }
    public QuestData Start()
    {
        questIndex = 0;
        state = NodeState.Active;
        return mainQuestData[0];
    }
    public QuestData GetCurrentQuest()
    {
        return mainQuestData[questIndex];
    }
}
public enum NodeState
{
    NotReached,
    Active,
    Complete
}
