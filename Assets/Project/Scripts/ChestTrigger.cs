using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChestTrigger : MonoBehaviour
{
    public ContainerInventory containerInventory;
    public DualInventoryUI dualInventoryUI;

    private bool playerInside = false;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            var receiver = GetComponent<IInventoryOpenReceiver>();
            dualInventoryUI.Open(containerInventory, receiver);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            dualInventoryUI.Close();
        }
    }
}
