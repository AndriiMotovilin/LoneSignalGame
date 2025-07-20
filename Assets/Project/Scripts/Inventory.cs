using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<PickableItem> items = new List<PickableItem>();
    public int maxSlots = 5;
    public InventoryUI inventoryUI;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(PickableItem item)
    {
        // Проверяем наличие свободной (null) ячейки
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                inventoryUI.Refresh(items);
                return;
            }
        }

        // Если пустых ячеек нет — добавляем в конец, если не превышен лимит
        if (items.Count < maxSlots)
        {
            items.Add(item);
            inventoryUI.Refresh(items);
        }
        else
        {
            Debug.LogWarning($"[Inventory] Инвентарь полон. Не удалось добавить {item.name}");
        }
    }


    public void SwapItems(int indexA, int indexB)
    {
        while (items.Count <= Mathf.Max(indexA, indexB))
            items.Add(null);

        // Меняем местами
        var temp = items[indexA];
        items[indexA] = items[indexB];
        items[indexB] = temp;

        inventoryUI.Refresh(items);
    }

    public void DropItem(int index, Vector3 dropPosition)
    {
        if (index < 0 || index >= items.Count || items[index] == null) return;

        PickableItem item = items[index];
        items[index] = null;

        if (item.droppedPrefab != null)
        {
            Instantiate(item.droppedPrefab, dropPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError($"[Inventory] ❌ У предмета '{item.name}' не задан droppedPrefab!", this);
        }

        inventoryUI.Refresh(items);
    }


}
