using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    public GameObject panel;
    public List<InventorySlotUI> slotUIs;

    [Header("Item Detail Panel")]
    public GameObject itemDetailPanel;
    public Text detailNameText;
    public Text detailDescriptionText;
    public Button useButton;
    public Button dropButton;

    [Header("UI Coordination")]
    public DualInventoryUI dualInventoryUI; // <- Добавлено

    private bool isVisible = false;
    private int selectedIndex = -1;

    void Start()
    {
        itemDetailPanel.SetActive(false);

        useButton.onClick.AddListener(() => UseSelectedItem());
        dropButton.onClick.AddListener(() => DropSelectedItem());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (dualInventoryUI != null && dualInventoryUI.IsOpen())
            {
                Debug.Log("[InventoryUI] Нельзя открыть: DualInventory открыт");
                return;
            }

            Toggle();
        }
    }

    public void ForceClose()
    {
        if (isVisible)
        {
            isVisible = false;
            panel.SetActive(false);
            HideItemDetailPanel();
        }
    }


    public void Toggle()
    {
        isVisible = !isVisible;
        panel.SetActive(isVisible);
        if (!isVisible)
        {
            itemDetailPanel.SetActive(false);
        }
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= Inventory.Instance.items.Count) return;

        var item = Inventory.Instance.items[index];
        if (item == null) return;

        selectedIndex = index;

        detailNameText.text = item.itemName;
        detailDescriptionText.text = item.description;
        useButton.gameObject.SetActive(item.isUsable);
        itemDetailPanel.SetActive(true);

        for (int i = 0; i < slotUIs.Count; i++)
        {
            slotUIs[i].SetSelected(i == index);
        }
    }

    private void UseSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= Inventory.Instance.items.Count) return;

        var item = Inventory.Instance.items[selectedIndex];
        if (item == null || !item.isUsable) return;

        Debug.Log($"Использован предмет: {item.itemName}");

        if (item.isHealingItem && item.healAmount > 0f)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.currentHealth += item.healAmount;
                    playerController.currentHealth = Mathf.Clamp(playerController.currentHealth, 0f, playerController.maxHealth);

                    if (playerController.healthSlider != null)
                        playerController.healthSlider.value = playerController.currentHealth;
                }
            }
        }

        Inventory.Instance.items[selectedIndex] = null;
        Refresh(Inventory.Instance.items);
        itemDetailPanel.SetActive(false);
    }

    private void DropSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= Inventory.Instance.items.Count) return;

        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector3 dropPos = player.transform.position + player.transform.forward * 1.5f;
        Inventory.Instance.DropItem(selectedIndex, dropPos);
        itemDetailPanel.SetActive(false);
    }

    public void HideItemDetailPanel()
    {
        itemDetailPanel.SetActive(false);

        foreach (var slot in slotUIs)
            slot.SetSelected(false);

        selectedIndex = -1;
    }

    public void Refresh(List<PickableItem> items)
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            var slot = slotUIs[i];
            slot.Setup(items, i, this);
            slot.SetSelected(false);
        }
    }
}
