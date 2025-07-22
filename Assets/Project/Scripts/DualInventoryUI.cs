using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DualInventoryUI : MonoBehaviour
{
    public GameObject panel;

    [Header("Player Inventory")]
    public List<InventorySlotUI> playerSlots;

    [Header("Chest Inventory")]
    public List<InventorySlotUI> containerSlots;

    [Header("Detail Panel")]
    public GameObject detailPanel;
    public Text detailName;
    public Text detailDesc;
    public Button actionButton;

    private ContainerInventory currentContainer;
    private int selectedIndex = -1;
    private bool selectedFromPlayer;

    public void Open(ContainerInventory container)
    {
        currentContainer = container;
        panel.SetActive(true);
        Refresh();
        HideDetails();
    }

    public void Close()
    {
        currentContainer = null;
        panel.SetActive(false);
    }

    public void Refresh()
    {
        var playerItems = Inventory.Instance.items;
        var chestItems = currentContainer.items;

        for (int i = 0; i < playerSlots.Count; i++)
        {
            playerSlots[i].Setup(playerItems, i, this, true);
            playerSlots[i].SetSelected(selectedFromPlayer && i == selectedIndex);
        }

        for (int i = 0; i < containerSlots.Count; i++)
        {
            containerSlots[i].Setup(chestItems, i, this, false);
            containerSlots[i].SetSelected(!selectedFromPlayer && i == selectedIndex);
        }
    }

    public void SelectItem(List<PickableItem> source, int index, bool fromPlayer)
    {
        if (index < 0 || index >= source.Count || source[index] == null) return;

        var item = source[index];
        selectedIndex = index;
        selectedFromPlayer = fromPlayer;

        detailPanel.SetActive(true);
        detailName.text = item.itemName;
        detailDesc.text = item.description;

        actionButton.onClick.RemoveAllListeners();

        var buttonText = actionButton.GetComponentInChildren<Text>();
        if (buttonText != null)
            buttonText.text = fromPlayer ? "Положить" : "Забрать";

        if (fromPlayer)
            actionButton.onClick.AddListener(() => TransferToChest(index));
        else
            actionButton.onClick.AddListener(() => TransferToPlayer(index));

        // Обновим подсветку
        Refresh();
    }

    public void HideDetails()
    {
        detailPanel.SetActive(false);
        selectedIndex = -1;

        foreach (var slot in playerSlots)
            slot.SetSelected(false);

        foreach (var slot in containerSlots)
            slot.SetSelected(false);
    }

    private void TransferToChest(int index)
    {
        var item = Inventory.Instance.items[index];
        if (item == null) return;

        Inventory.Instance.items[index] = null;
        currentContainer.AddItem(item);

        Refresh();
        HideDetails();
    }

    private void TransferToPlayer(int index)
    {
        var item = currentContainer.items[index];
        if (item == null) return;

        currentContainer.items[index] = null;
        Inventory.Instance.AddItem(item);

        Refresh();
        HideDetails();
    }
}
