using UnityEngine;

public class BatterySocket : MonoBehaviour
{
    public string requiredItemName = "Battery";
    public GameObject targetToActivate; // например, дверь
    private bool isActivated = false;
    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            int index = FindItemInInventory(requiredItemName);
            if (index != -1)
            {
                Inventory.Instance.items[index] = null;
                Inventory.Instance.SendMessage("RefreshUI", SendMessageOptions.DontRequireReceiver);

                Activate();
            }
            else
            {
                Debug.Log("У игрока нет нужной батареи.");
            }
        }
    }

    void Activate()
    {
        isActivated = true;
        Debug.Log("Батарея вставлена. Активация.");
        if (targetToActivate != null)
            targetToActivate.SendMessage("OnBatteryInserted", SendMessageOptions.DontRequireReceiver);
    }

    int FindItemInInventory(string itemName)
    {
        for (int i = 0; i < Inventory.Instance.items.Count; i++)
        {
            if (Inventory.Instance.items[i] != null && Inventory.Instance.items[i].itemName == itemName)
                return i;
        }
        return -1;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}
