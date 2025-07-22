using UnityEngine;

public class ChestTrigger : MonoBehaviour
{
    public ContainerInventory containerInventory;
    public DualInventoryUI dualInventoryUI;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            dualInventoryUI.Open(containerInventory);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dualInventoryUI.Close();
        }
    }
}
