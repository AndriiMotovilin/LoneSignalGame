using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class PickableItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [TextArea(2, 5)]
    public string description;

    public GameObject droppedPrefab;

    [Header("Usage")]
    public bool isUsable = false;

    [Header("Healing")]
    public bool isHealingItem = false;
    public float healAmount = 0f;

    [Header("Keycard")]
    public bool isKeyCard = false;
    public string keyCardID;

}
