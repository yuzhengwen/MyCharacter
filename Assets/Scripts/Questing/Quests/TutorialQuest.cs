using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest : BaseQuest
{
    public TutorialQuest(QuestManager questManager) : base(questManager)
    {
        questName = "Tutorial Quest";
        questId = 0;
    }
    protected override void AddActions()
    {
        requirements = new QuestAction[]
        {
            //new PlayerLevelReached(questManager, 2),
            new CollectItemAction(questManager, AllItems.allItems[AllItems.CRYSTAL], 1)
        };

        questSteps = new QuestAction[]
        {
            new CollectItemAction(questManager, AllItems.allItems[AllItems.CRYSTAL], 1)
        };

        rewards = new QuestReward[]
        {
            new(RewardType.Gold, 100),
            new(RewardType.Exp, 200)
        };
    }

    protected override void CompleteQuest()
    {
        base.CompleteQuest();
        Debug.Log("Tutorial Quest Complete");
    }
}
