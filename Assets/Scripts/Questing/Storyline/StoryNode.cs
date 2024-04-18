using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StoryNode 
{
    public string title;
    public string description;
    public int part;
    public int index;
    public StoryNode next;
    public QuestData[] mainQuests;
}
