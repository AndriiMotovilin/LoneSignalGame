using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<Quest> activeQuests = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddQuest(Quest quest)
    {
        if (!activeQuests.Contains(quest))
        {
            quest.Activate();
            activeQuests.Add(quest);
            Debug.Log($"🧭 Новый квест: {quest.questName}");
        }
    }

    public void CompleteObjective(Quest quest, int index)
    {
        if (activeQuests.Contains(quest))
        {
            quest.CompleteObjective(index);
            Debug.Log($"✅ Цель выполнена: {quest.objectives[index].description}");
        }
    }

    public bool IsObjectiveCompleted(Quest quest, int index)
    {
        return quest.objectives[index].isCompleted;
    }
}
