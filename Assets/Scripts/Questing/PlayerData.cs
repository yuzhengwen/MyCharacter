using System;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public event Action<int, int> OnLevelUp;

    public int Level { get; private set; } = 1;
    public int Exp { get; private set; } = 0;
    public int ExpToNextLevel { get; private set; } = 0;

    private void Start()
    {
        ExpToNextLevel = GetExpToNextLevel(Level);
    }

    public void AddExp(int amount)
    {
        Exp += amount;
        while (Exp > ExpToNextLevel)
        {
            Exp = Exp - ExpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        OnLevelUp?.Invoke(Level, Level + 1);
        Level++;
        ExpToNextLevel = GetExpToNextLevel(Level);
    }

    private int GetExpToNextLevel(int level)
    {
        return 200 + level * 10;
    }
}