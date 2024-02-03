using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelReached : QuestAction
{
    private readonly int levelToReach;
    private readonly QuestManager qm;

    public PlayerLevelReached(QuestManager qm, int levelToReach) : base(qm)
    {
        this.levelToReach = levelToReach;
    }
    private void CheckLevel(int prev, int cur)
    {
        if (cur >= levelToReach)
            Complete();
    }
    public override void Start()
    {
        qm.playerData.OnLevelUp += CheckLevel;
    }
    protected override void Complete()
    {
        base.Complete();
        qm.playerData.OnLevelUp -= CheckLevel;
    }
}
