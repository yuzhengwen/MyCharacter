using InventorySystem;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Inventory inventory;

    [Header("Reward Items")]
    [SerializeField] private ItemDataSO gold;

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
                    inventory.AddItem(gold, reward.amount);
                    break;
            }
        }
    }
}
