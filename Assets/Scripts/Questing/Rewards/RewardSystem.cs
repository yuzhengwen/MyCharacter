using InventorySystem;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Inventory inventory;

    public void GiveRewards(params Reward[] rewards)
    {
        foreach (var reward in rewards)
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
                    inventory.AddItem(ItemUtils.Instance.GetItemDataByName("Coin"), reward.amount);
                    break;
            }
        }
    }
}
