using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using YuzuValen;
using YuzuValen.Utils;

public class QuestJournal : MonoBehaviour
{
    [SerializeField] private RewardSystem rewardSystem;
    [SerializeField] private StoryManager storyManager;

    [SerializeField] private List<QuestData> initialQuests = new();
    [ReadOnlyInspector][SerializeField] private List<Quest> quests = new();

    #region Main Story
    private StoryNode CurrentStory => storyManager.CurrentNode;
    [ReadOnlyInspector][SerializeField] private Quest currentStoryQuest;
    #endregion

    private void Start()
    {
        // try to load quests from save data
        // if no save data, start quests from scratch
        quests = CreateNewQuests(initialQuests);
        foreach (var quest in quests)
        {
            StartQuest(quest);
        }
        StartStory();
    }
    public void LoadFromSave(StoryNodeGroup[] mainStory, StoryNode currentNode, Quest[] quests)
    {
        storyManager.mainStory = mainStory;
        storyManager.CurrentNode = currentNode;
        this.quests = quests.ToList();
    }

    private List<Quest> CreateNewQuests(List<QuestData> datas)
    {
        List<Quest> quests = new();
        for (int i = 0; i < datas.Count; i++)
        {
            quests.Add(new Quest(datas[i]));
        }
        return quests;
    }

    private Quest StartQuest(Quest quest)
    {
        quest.OnComplete += GiveRewards;
        quest.Init();
        return quest;
    }

    private void GiveRewards(Quest quest)
    {
        Reward[] rewards = quest.data.rewards;
        rewardSystem.GiveRewards(rewards);
        quest.OnComplete -= GiveRewards;
    }

    /// <summary>
    /// Triggers a quest event to all quests that are in progress or have requirements not met<br/>
    /// Objective must implement IQuestEventResponder to respond to the event
    /// </summary>
    /// <param name="eventName">Name of event</param>
    /// <param name="args">EventArgs for passing arguments</param>
    public void TriggerEvent(string eventName, EventArgs args = null)
    {
        foreach (var quest in quests)
        {
            quest.TriggerEvent(eventName, args);
        }
        currentStoryQuest.TriggerEvent(eventName, args);
    }

    private void StartStory()
    {
        currentStoryQuest = (new Quest(CurrentStory.Start()));
        StartQuest(currentStoryQuest);
        currentStoryQuest.OnComplete += ProgressStory;
    }

    private void ProgressStory(Quest quest)
    {
        Debug.Log($"Story Quest Complete: {quest.data.questName}");
        quest.OnComplete -= ProgressStory;
        var nextQuestData = CurrentStory.NextQuest(out bool isNodeComplete);
        Debug.Log($"Node complete: {isNodeComplete}");
        if (isNodeComplete)
        {
            storyManager.NextStoryNode(out bool isStoryComplete);
            if (isStoryComplete)
            {
                Debug.Log("CONGRATS! Story Complete");
                return;
            }
            StartStory();
            return;
        }
        currentStoryQuest = StartQuest(new Quest(nextQuestData));
        currentStoryQuest.OnComplete += ProgressStory;
    }

    [ContextMenu("Reset Story")]
    private void ResetStory()
    {
        storyManager.Reset();
        StartStory();
    }

    public StoryManager GetStoryManager()
    {
        return storyManager;
    }
    public List<Quest> GetQuests()
    {
        return quests;
    }
}
