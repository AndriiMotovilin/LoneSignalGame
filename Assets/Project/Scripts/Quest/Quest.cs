using System.Collections.Generic;
using UnityEngine;

public enum QuestState { Inactive, Active, Completed, Failed }

[System.Serializable]
public class QuestObjective
{
    public string description;
    public bool isCompleted;
}

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string questDescription;
    public QuestState state = QuestState.Inactive;

    public List<QuestObjective> objectives = new();

    public void Activate()
    {
        state = QuestState.Active;
    }

    public void CompleteObjective(int index)
    {
        if (index < 0 || index >= objectives.Count) return;
        objectives[index].isCompleted = true;
        CheckCompletion();
    }

    public void CheckCompletion()
    {
        if (objectives.TrueForAll(o => o.isCompleted))
            state = QuestState.Completed;
    }
}
