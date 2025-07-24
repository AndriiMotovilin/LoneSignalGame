using UnityEngine;

[RequireComponent(typeof(ItemWorld))]
public class QuestPickupItem : MonoBehaviour
{
    public Quest linkedQuest;
    public int objectiveIndex;

    private ItemWorld itemWorld;
    private bool playerNearby = false;

    void Start()
    {
        itemWorld = GetComponent<ItemWorld>();
        if (itemWorld == null)
            Debug.LogError("[QuestPickupItem] ❌ Не найден компонент ItemWorld!");
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }

    void TryPickup()
    {
        if (!itemWorld.canBePickedUp) return;

        Inventory.Instance.AddItem(itemWorld.itemData);
        Debug.Log($"🧤 Предмет подобран: {itemWorld.itemData.itemName}");

        if (linkedQuest != null && linkedQuest.state == QuestState.Active)
        {
            QuestManager.Instance.CompleteObjective(linkedQuest, objectiveIndex);
            Debug.Log($"🎯 Цель выполнена: {linkedQuest.objectives[objectiveIndex].description}");
        }

        Destroy(gameObject);
    }
}
