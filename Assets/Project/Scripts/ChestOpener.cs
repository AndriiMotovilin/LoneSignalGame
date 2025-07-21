using UnityEngine;

public class ChestOpener : MonoBehaviour
{
    public DualInventoryUI inventoryUI;

    private bool playerNearby = false;
    private ContainerInventory container;

    void Start()
    {
        container = GetComponent<ContainerInventory>();
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            inventoryUI.Open(container);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inventoryUI.Close();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}
