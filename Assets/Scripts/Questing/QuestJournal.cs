using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestJournal : MonoBehaviour
{
    [SerializeField] private List<Quest> quests = new();

    [SerializeField] private RewardSystem rewardSystem;

    private void Start()
    {
        LinkQuestRewards();
    }
    private void LinkQuestRewards()
    {
        foreach (Quest quest in quests)
        {
            quest.OnComplete += GiveRewards;
        }
    }

    private void GiveRewards(Quest quest)
    {
        Reward[] rewards = quest.rewards;
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
        foreach (Quest quest in quests)
        {
            quest.TriggerEvent(eventName, args);
        }
    }
}
