using System;
using UnityEngine;

[Serializable]
public struct Reward
{
    public RewardType rewardType;
    public int amount;
    public ItemDataSO itemData;

    public Reward(RewardType rewardType, int amount, ItemDataSO itemData = null)
    {
        this.rewardType = rewardType;
        this.amount = amount;
        this.itemData = itemData;
    }
}
public enum RewardType
{
    Gold, Exp, Item
}