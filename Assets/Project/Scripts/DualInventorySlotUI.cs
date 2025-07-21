using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DualInventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public int index;
    public bool isFromContainer;

    private DualInventoryUI manager;

    public void Setup(DualInventoryUI uiManager, int slotIndex, bool fromContainer)
    {
        manager = uiManager;
        index = slotIndex;
        isFromContainer = fromContainer;
    }

    public void SetItem(PickableItem item)
    {
        if (item != null)
        {
            icon.sprite = item.icon;
            icon.color = Color.white;
        }
        else
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Нажат слот: {index}, из контейнера: {isFromContainer}");
        manager.SelectItem(index, isFromContainer);
    }

}
