using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Locker : MonoBehaviour
{
    public Transform doorTransform;
    public Vector3 closedRotation = Vector3.zero;
    public Vector3 openRotation = new Vector3(0, 100f, 0);
    public float rotationSpeed = 5f;
    public float pickupEnableThreshold = 0.99f; // Порог открытия для активации подбора

    private bool isOpen = false;
    private bool playerNearby = false;
    private Transform player;
    private bool pickupEnabled = false;

    void Update()
    {
        Quaternion targetRot = Quaternion.Euler(isOpen ? openRotation : closedRotation);
        doorTransform.localRotation = Quaternion.Lerp(doorTransform.localRotation, targetRot, Time.deltaTime * rotationSpeed);

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            float angleDelta = Quaternion.Angle(doorTransform.localRotation, Quaternion.Euler(openRotation));

            if (!isOpen)
            {
                isOpen = true;
                pickupEnabled = false;
                SetItemsPickupState(false);
            }
            else if (isOpen && !pickupEnabled && angleDelta < (1.0f - pickupEnableThreshold) * 100f)
            {
                SetItemsPickupState(true);
                pickupEnabled = true;
            }

            // Удалён блок, позволяющий закрытие
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            player = null;
        }
    }

    private void SetItemsPickupState(bool allow)
    {
        float radius = 2f;
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            ItemWorld item = hit.GetComponent<ItemWorld>();
            if (item != null)
            {
                item.canBePickedUp = allow;
                if (!allow && item.pickupHintUI != null)
                    item.pickupHintUI.SetActive(false);
            }
        }
    }
}