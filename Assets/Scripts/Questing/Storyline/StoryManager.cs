﻿using System;
using System.Collections.Generic;
using UnityEngine;
using YuzuValen.Utils;

public partial class StoryManager : MonoBehaviour
{
    public StoryNodeGroup[] mainStory;

    public event Action<StoryNode> OnStoryNodeStart, OnStoryNodeComplete;
    public event Action<StoryNodeGroup> OnStoryGroupStart, OnStoryGroupComplete;
    public event Action OnStoryComplete;

    [ReadOnlyInspector] public StoryNode CurrentNode;

    private void Awake()
    {
        AutoSetIndexes(mainStory);
        CurrentNode = GetNode(StoryIndex.Zero);
    }

    private void AutoSetIndexes(StoryNodeGroup[] mainStory)
    {
        for (int i = 0; i < mainStory.Length; i++)
        {
            for (int j = 0; j < mainStory[i].nodes.Length; j++)
            {
                mainStory[i].nodes[j].index = new StoryIndex(i, j);
            }
        }
    }
    /// <summary>
    /// Move to the next story node <br/>
    /// If current node is last node of story group, move to the first node of the next story group
    /// </summary>
    /// <param name="isComplete">Whether the entire story is complete</param>
    /// <returns></returns>
    internal StoryNode NextStoryNode(out bool isComplete)
    {
        OnStoryNodeComplete?.Invoke(CurrentNode);

        StoryIndex nextIndex = NextIndex(CurrentNode.index, out isComplete);
        if (isComplete)
        {
            OnStoryComplete?.Invoke();
            return null;
        }
        if (nextIndex.index == 0)
        {
            OnStoryGroupComplete?.Invoke(mainStory[CurrentNode.index.part]);
            OnStoryGroupStart?.Invoke(mainStory[nextIndex.part]);
        }
        CurrentNode = GetNode(nextIndex);
        OnStoryNodeStart?.Invoke(CurrentNode);
        return CurrentNode;
    }
    internal StoryIndex NextIndex(StoryIndex index, out bool isComplete)
    {
        // if reached the end of the last part and last node
        if (index.part >= mainStory.Length - 1 && index.index >= mainStory[^1].nodes.Length - 1)
        {
            isComplete = true;
            return new StoryIndex(mainStory.Length - 1, mainStory[^1].nodes.Length - 1);
        }
        isComplete = false;
        // go to next part if reached the end of the current part
        if (index.index >= mainStory[index.part].nodes.Length - 1)
        {
            return new StoryIndex(index.part + 1, 0);
        }
        // go to next node in the current part
        if (index.index < mainStory[index.part].nodes.Length - 1)
        {
            return new StoryIndex(index.part, index.index + 1);
        }
        // should never reach here
        return StoryIndex.Zero;
    }
    public StoryNode GetNode(StoryIndex index)
    {
        if (index.index >= mainStory[index.part].nodes.Length)
        {
            Debug.LogError("Story index out of range: " + index.part + "." + index.index);
            return null;
        }
        if (index.part >= mainStory.Length)
        {
            Debug.LogError("Story part out of range: " + index.part);
            return null;
        }
        return mainStory[index.part].nodes[index.index];
    }
    internal void Reset()
    {
        foreach (var group in mainStory)
        {
            foreach (var node in group.nodes)
            {
                node.state = NodeState.NotReached;
            }
        }
        CurrentNode = GetNode(StoryIndex.Zero);
    }
    [ContextMenu("Log Story Progress")]
    public void LogStoryProgress()
    {
        Debug.Log("Current Story Node: " + CurrentNode.title);
        Debug.Log("Current Story Index: " + CurrentNode.index);
    }

    public bool IsCompleted(StoryIndex index) => CurrentNode.index > index;
}