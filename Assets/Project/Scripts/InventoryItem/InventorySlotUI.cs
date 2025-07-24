using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public Image icon;
    public GameObject selectionHighlight;

    [HideInInspector] public int index;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    private List<PickableItem> itemList;
    private DualInventoryUI dualUI;
    private InventoryUI singleUI;
    private bool isPlayer;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        SetSelected(false);
    }

    public void Setup(List<PickableItem> items, int i, DualInventoryUI uiRef, bool fromPlayer)
    {
        itemList = items;
        index = i;
        dualUI = uiRef;
        singleUI = null;
        isPlayer = fromPlayer;

        ApplyVisual();
    }

    public void Setup(List<PickableItem> items, int i, InventoryUI uiRef)
    {
        itemList = items;
        index = i;
        singleUI = uiRef;
        dualUI = null;
        isPlayer = true;

        ApplyVisual();
    }

    private void ApplyVisual()
    {
        var item = (index < itemList.Count) ? itemList[index] : null;
        icon.sprite = item != null ? item.icon : null;
        icon.color = item != null ? Color.white : new Color(1, 1, 1, 0);
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(selected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemList == null || index < 0 || index >= itemList.Count) return;

        if (dualUI != null)
            dualUI.SelectItem(itemList, index, isPlayer);
        else if (singleUI != null)
            singleUI.SelectItem(index);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;

        if (dualUI != null) dualUI.HideDetails();
        if (singleUI != null) singleUI.HideItemDetailPanel();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.position = originalPosition;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (dragged == null || dragged == this) return;

        while (itemList.Count <= index)
            itemList.Add(null);
        while (dragged.itemList.Count <= dragged.index)
            dragged.itemList.Add(null);

        var a = dragged.itemList[dragged.index];
        var b = itemList[index];

        // Обновляем списки
        dragged.itemList[dragged.index] = b;
        itemList[index] = a;

        // 🔁 Синхронизируем с Inventory.Instance, если работаем с его списком
        if (itemList == Inventory.Instance.items || dragged.itemList == Inventory.Instance.items)
        {
            Inventory.Instance.inventoryUI.Refresh(Inventory.Instance.items);
        }

        // 🔄 Обновляем Dual UI
        if (dualUI != null)
        {
            dualUI.Refresh();
            dualUI.HideDetails();
        }
        else if (singleUI != null)
        {
            singleUI.Refresh(itemList);
            singleUI.HideItemDetailPanel();
        }
    }


    public void DropThisItem()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector3 dropPos = player.transform.position + player.transform.forward * 1.5f;
            Inventory.Instance.DropItem(index, dropPos);
        }
    }
}
