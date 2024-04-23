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

    #region Main Story
    private StoryNode CurrentStory => storyManager.CurrentNode;
    [ReadOnlyInspector][SerializeField] private Quest currentStoryQuest;
    #endregion

    #region Quests
    [SerializeField] private List<QuestData> initialQuests = new();
    [ReadOnlyInspector][SerializeField] private List<Quest> quests = new();
    #endregion

    private void OnEnable()
    {
        Quest.OnComplete += GiveRewards;
        Quest.OnComplete += ProgressStory;
        /*
        Quest.OnProgressed += (q, o) => Debug.Log($"Quest Progressed: {q.data.questName} - {o.objectiveName}");
        Quest.OnStateChanged += (q, s) => Debug.Log($"Quest State Changed: {q.data.questName} - {s}");
        Quest.OnComplete += (q) => Debug.Log($"Quest Complete: {q.data.questName}");
        */
        storyManager.OnStoryComplete += ()=> Debug.Log("CONGRATS! STORY COMPLETED");
        storyManager.OnStoryNodeComplete += (n) => Debug.Log($"Story Node Complete: {n.title}");
    }
    private void OnDisable()
    {
        Quest.OnComplete -= GiveRewards;
        Quest.OnComplete -= ProgressStory;
    }
    private void Start()
    {
        // try to load quests from save data
        // if no save data, start quests from scratch
        quests = CreateNewQuests(initialQuests);
        foreach (var quest in quests)
        {
            quest.Init();
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

    private void GiveRewards(Quest quest)
    {
        Reward[] rewards = quest.data.rewards;
        rewardSystem.GiveRewards(rewards);
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
        currentStoryQuest.Init();
    }

    private void ProgressStory(Quest quest)
    {
        if (quest != currentStoryQuest)
            return;

        var nextQuestData = CurrentStory.NextQuest(out bool isNodeComplete);
        if (isNodeComplete)
        {
            storyManager.NextStoryNode(out bool isStoryComplete);
            if (isStoryComplete)
                return;
            StartStory();
            return;
        }
        currentStoryQuest = new Quest(nextQuestData);
        currentStoryQuest.Init();
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
    public List<Quest> GetAllQuests()
    {
        return quests;
    }
    public List<Quest> GetAllNotCompleted()
    {
        return quests.FindAll(q => q.currentState != QuestState.Completed);
    }
    public List<Quest> GetQuestByState(QuestState state)
    {
        return quests.FindAll(q => q.currentState == state);
    }
}
