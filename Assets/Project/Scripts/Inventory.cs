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
    public bool CanAddItem()
    {
        foreach (var item in items)
        {
            if (item == null) return true;
        }
        return items.Count < maxSlots;
    }


    public void AddItem(PickableItem item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                RefreshUI();
                return;
            }
        }

        if (items.Count < maxSlots)
        {
            items.Add(item);
            RefreshUI();
        }
        else
        {
            Debug.LogWarning($"[Inventory] Инвентарь полон. Не удалось добавить {item.name}");
        }
    }

    private void RefreshUI()
    {
        if (inventoryUI != null)
            inventoryUI.Refresh(items);
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
