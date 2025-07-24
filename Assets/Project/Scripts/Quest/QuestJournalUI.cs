using UnityEngine;
using UnityEngine.UI;

public class QuestJournalUI : MonoBehaviour
{
    public GameObject panel;
    public Text questText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            panel.SetActive(!panel.activeSelf);
            Refresh();
        }
    }

    void Refresh()
    {
        questText.text = "";
        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            questText.text += $"<b>{quest.questName}</b> [{quest.state}]\n";
            foreach (var obj in quest.objectives)
                questText.text += $" - {(obj.isCompleted ? "<color=green>" : "")}{obj.description}{(obj.isCompleted ? "</color>" : "")}\n";
            questText.text += "\n";
        }
    }
}
