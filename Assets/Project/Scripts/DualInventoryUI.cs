using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DualInventoryUI : MonoBehaviour
{
    public GameObject panel;

    [Header("Player Inventory")]
    public Transform playerSlotsParent;
    public List<DualInventorySlotUI> playerSlotUIs;

    [Header("Container Inventory")]
    public Transform containerSlotsParent;
    public List<DualInventorySlotUI> containerSlotUIs;

    [Header("Item Detail Panel")]
    public GameObject itemDetailPanel;
    public Text itemNameText;
    public Text itemDescriptionText;
    public Button actionButton;

    private ContainerInventory currentContainer;
    private int selectedIndex = -1;
    private bool selectedFromContainer = false;

    public void Open(ContainerInventory container)
    {
        currentContainer = container;
        panel.SetActive(true);

        RefreshPlayerInventory();
        RefreshContainerInventory();

        itemDetailPanel.SetActive(false);
    }

    public void Close()
    {
        panel.SetActive(false);
        currentContainer = null;
    }

    public void RefreshPlayerInventory()
    {
        var items = Inventory.Instance.items;
        for (int i = 0; i < playerSlotUIs.Count; i++)
        {
            var slot = playerSlotUIs[i];
            slot.Setup(this, i, false);
            slot.SetItem(i < items.Count ? items[i] : null);
        }
    }

    public void RefreshContainerInventory()
    {
        var items = currentContainer.items;
        for (int i = 0; i < containerSlotUIs.Count; i++)
        {
            var slot = containerSlotUIs[i];
            slot.Setup(this, i, true);
            slot.SetItem(i < items.Count ? items[i] : null);
        }
    }

    public void SelectItem(int index, bool fromContainer)
    {
        selectedIndex = index;
        selectedFromContainer = fromContainer;

        PickableItem item = fromContainer ? currentContainer.items[index] : Inventory.Instance.items[index];
        if (item == null)
        {
            itemDetailPanel.SetActive(false);
            return;
        }

        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;

        actionButton.onClick.RemoveAllListeners();
        if (fromContainer)
        {
            actionButton.GetComponentInChildren<Text>().text = "???????";
            actionButton.onClick.AddListener(() => TakeFromContainer(index));
        }
        else
        {
            actionButton.GetComponentInChildren<Text>().text = "????????";
            actionButton.onClick.AddListener(() => PutToContainer(index));
        }

        itemDetailPanel.SetActive(true);
    }

    void TakeFromContainer(int index)
    {
        if (index < 0 || index >= currentContainer.items.Count) return;

        var item = currentContainer.items[index];
        if (item == null) return;

        if (Inventory.Instance.CanAddItem())
        {
            Inventory.Instance.AddItem(item);
            currentContainer.items[index] = null;
            RefreshContainerInventory();
            RefreshPlayerInventory();
            itemDetailPanel.SetActive(false);
        }
    }

    void PutToContainer(int index)
    {
        if (index < 0 || index >= Inventory.Instance.items.Count) return;

        var item = Inventory.Instance.items[index];
        if (item == null) return;

        if (currentContainer.CanAddItem())
        {
            currentContainer.AddItem(item);
            Inventory.Instance.items[index] = null;
            RefreshPlayerInventory();
            RefreshContainerInventory();
            itemDetailPanel.SetActive(false);
        }
    }
}
