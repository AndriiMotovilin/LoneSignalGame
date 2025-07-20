using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public Image icon;
    public int index;

    public GameObject selectionHighlight;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(selected);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;

        // Скрыть описание предмета при начале перетаскивания
        Inventory.Instance.inventoryUI.HideItemDetailPanel();
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
        var draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (draggedSlot != null && draggedSlot != this)
        {
            Inventory.Instance.SwapItems(index, draggedSlot.index);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Instance.inventoryUI.SelectItem(index);
        }
    }

    // Метод DropThisItem оставлен для кнопки "Выбросить"
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
