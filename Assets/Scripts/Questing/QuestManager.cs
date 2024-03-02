using InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private List<BaseQuest> quests = new();
    private List<BaseQuest> completedQuests = new();

    public PlayerData playerData;
    public Inventory inventory;

    private void Start()
    {
        BaseQuest quest = new TutorialQuest(this);
        quest.OnCompleted += GiveRewards;
        quest.OnCompleted += UpdateLists;
        quests.Add(quest);
    }

    private void UpdateLists(BaseQuest quest)
    {
        quest.OnCompleted -= UpdateLists;
        quests.Remove(quest);
        completedQuests.Add(quest);
    }

    private void GiveRewards(BaseQuest completedQuest)
    {
        completedQuest.OnCompleted -= GiveRewards;
        foreach (QuestReward reward in completedQuest.rewards)
        {
            switch (reward.rewardType)
            {
                case RewardType.Item:
                    inventory.AddItem(reward.itemData, reward.amount);
                    break;
                case RewardType.Exp:
                    playerData.AddExp(reward.amount);
                    break;
                case RewardType.Gold:
                    inventory.AddItem(AllItems.allItems[AllItems.COIN], reward.amount);
                    break;
            }
        }
    }
}