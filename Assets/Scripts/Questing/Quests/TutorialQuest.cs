using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest : BaseQuest
{
    public TutorialQuest(QuestManager questManager) : base(questManager)
    {
        name = "Tutorial Quest";
        id = 0;
    }
    protected override void AddActions()
    {
        requirements = new BaseObjective[]
        {
            new PlayerLevelReached(this, questManager.playerStats, 2),
            new CollectItemAction(this, questManager, ItemDB.GetItemData(ItemDB.CRYSTAL), 1)
        };

        questSteps = new BaseObjective[]
        {
            new CollectItemAction(this, questManager, ItemDB.GetItemData(ItemDB.CRYSTAL), 1)
        };

        rewards = new QuestReward[]
        {
            new(RewardType.Gold, 100),
            new(RewardType.Exp, 200)
        };
    }

    public override void Complete()
    {
        base.Complete();
        Debug.Log("Tutorial Quest Complete");
    }
}
