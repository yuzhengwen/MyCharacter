using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelReached : BaseObjective
{
    private readonly int levelToReach;
    private readonly PlayerStats playerStats;

    public PlayerLevelReached(BaseQuest parent, PlayerStats playerStats, int levelToReach) : base(parent, "Reached Level")
    {
        this.levelToReach = levelToReach;
        this.playerStats = playerStats;
    }
    public void CheckLevel(int prev, int cur)
    {
        if (cur >= levelToReach)
            Complete();
    }
    public override void Start()
    {
        playerStats.OnLevelUp += CheckLevel;
    }
    public override void Complete()
    {
        base.Complete();
        playerStats.OnLevelUp -= CheckLevel;
    }
}
