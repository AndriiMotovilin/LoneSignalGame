using System.Collections.Generic;
using UnityEngine;

public class ContainerInventory : MonoBehaviour
{
    public List<PickableItem> items = new List<PickableItem>();
    public int maxSlots = 10;

    public void AddItem(PickableItem item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                return;
            }
        }

        if (items.Count < maxSlots)
            items.Add(item);
    }

    public void RemoveItem(int index)
    {
        if (index >= 0 && index < items.Count)
            items[index] = null;
    }
}
