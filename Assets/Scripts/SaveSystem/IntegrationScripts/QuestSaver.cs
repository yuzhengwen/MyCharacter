using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YuzuValen
{
    public class QuestSaver : MonoBehaviour, ISaveable
    {
        [SerializeField] private QuestJournal questJournal;
        private StoryManager storyManager;

        private void Start()
        {
            storyManager = questJournal.GetStoryManager();
        }

        public void Load(SaveData data)
        {
            var questData = data.questData;
            questJournal.LoadFromSave(questData.mainStory, questData.CurrentNode, questData.quests);
        }

        public void Save(SaveData data)
        {
            var questData = data.questData;
            questData.quests = questJournal.GetQuests().ToArray();
            questData.mainStory = storyManager.mainStory;
            questData.CurrentNode = storyManager.CurrentNode;
        }
    }
    [System.Serializable]
    public class QuestData
    {
        public Quest[] quests;
        public StoryNodeGroup[] mainStory;
        public StoryNode CurrentNode;
    }
}
