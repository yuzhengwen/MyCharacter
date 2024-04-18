using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public StoryNodeGroup[] storyGroups;
    private Dictionary<StoryNodeGroup, bool> storyProgress= new();

    private void Awake()
    {
        UpdateDictionary();
    }

    private void UpdateDictionary()
    {
        foreach (var group in storyGroups)
        {
            storyProgress.Add(group, false);
        }
    }
}