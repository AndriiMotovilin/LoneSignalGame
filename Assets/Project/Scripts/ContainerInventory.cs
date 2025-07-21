using System.Collections.Generic;
using UnityEngine;

public class ContainerInventory : MonoBehaviour
{
    public List<PickableItem> items = new List<PickableItem>();
    public int maxSlots = 8;

    public bool AddItem(PickableItem item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                return true;
            }
        }

        if (items.Count < maxSlots)
        {
            items.Add(item);
            return true;
        }

        Debug.Log("[Container] Контейнер полон");
        return false;
    }

    public bool CanAddItem()
    {
        foreach (var item in items)
        {
            if (item == null) return true;
        }
        return items.Count < maxSlots;
    }
}
