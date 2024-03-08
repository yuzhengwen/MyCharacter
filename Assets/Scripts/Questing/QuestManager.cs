using InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<BaseQuest> quests = new();
    [SerializeField] private List<BaseQuest> completedQuests = new();

    public PlayerStats playerStats;
    public Inventory inventory;

    private void Awake()
    {
        playerStats = new PlayerStats();
    }
    private void Start()
    {
        AddQuest(new TutorialQuest(this));
    }
    private void AddQuest(BaseQuest quest)
    {
        quest.OnComplete += GiveRewards;
        quest.OnComplete += UpdateLists;
        quests.Add(quest);
    }

    private void UpdateLists(BaseQuest quest)
    {
        quest.OnComplete -= UpdateLists;
        quests.Remove(quest);
        completedQuests.Add(quest);
    }

    private void GiveRewards(BaseQuest completedQuest)
    {
        completedQuest.OnComplete -= GiveRewards;
        foreach (QuestReward reward in completedQuest.rewards)
        {
            switch (reward.rewardType)
            {
                case RewardType.Item:
                    inventory.AddItem(reward.itemData, reward.amount);
                    break;
                case RewardType.Exp:
                    playerStats.AddExp(reward.amount);
                    break;
                case RewardType.Gold:
                    inventory.AddItem(ItemDB.allItems[ItemDB.COIN], reward.amount);
                    break;
            }
        }
    }
    /// <summary>
    /// Triggers a quest event to all quests that are in progress or have requirements not met<br/>
    /// Objective must implement IQuestEventResponder to respond to the event
    /// </summary>
    /// <param name="eventName">Name of event</param>
    /// <param name="args">EventArgs for passing arguments</param>
    public void TriggerQuestEvent(string eventName, EventArgs args)
    {
        foreach (BaseQuest quest in quests)
        {
            if (quest.CurrentState == ObjectiveState.RequirementsNotMet)
            {
                IQuestEventResponder[] responders = Array.FindAll(quest.requirements,
                    (obj) => !obj.IsComplete && obj is IQuestEventResponder).Cast<IQuestEventResponder>().ToArray();

                foreach (IQuestEventResponder responder in responders)
                    responder.OnQuestEvent(eventName, args);
            }
            else if (quest.CurrentState == ObjectiveState.InProgress)
            {
                IQuestEventResponder responder = quest.CurrentStep as IQuestEventResponder;
                responder?.OnQuestEvent(eventName, args);
            }
        }
    }
}

public interface IQuestEventResponder
{
    void OnQuestEvent(string eventName, EventArgs args);
}