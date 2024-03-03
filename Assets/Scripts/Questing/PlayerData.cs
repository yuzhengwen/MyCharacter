using System;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public event Action<int, int> OnLevelUp;

    public int level { get; private set; } = 1;
    public int exp { get; private set; } = 0;
    public int expToNextLevel { get; private set; }

    private void Start()
    {
        expToNextLevel = GetExpToNextLevel(level);
    }

    public void AddExp(int amount)
    {
        exp += amount;
        while (exp > expToNextLevel)
        {
            exp = exp - expToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        OnLevelUp?.Invoke(level, level + 1);
        level++;
        expToNextLevel = GetExpToNextLevel(level);
    }

    private int GetExpToNextLevel(int level)
    {
        return 200 + level * 10;
    }
}