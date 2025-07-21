using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeycardDoor : MonoBehaviour
{
    public Transform doorTransform;
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float openCloseSpeed = 2f;

    public string requiredKeyID;
    private bool playerNearby = false;

    private Vector3 closedPos;
    private Vector3 openPos;

    void Start()
    {
        if (doorTransform == null) doorTransform = transform;
        closedPos = doorTransform.position;
        openPos = closedPos + openOffset;
    }

    void Update()
    {
        bool hasKey = HasRequiredKeycard();

        Vector3 targetPos = (playerNearby && hasKey) ? openPos : closedPos;
        doorTransform.position = Vector3.Lerp(doorTransform.position, targetPos, Time.deltaTime * openCloseSpeed);
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

    private bool HasRequiredKeycard()
    {
        foreach (var item in Inventory.Instance.items)
        {
            if (item != null && item.isKeyCard && item.keyCardID == requiredKeyID)
                return true;
        }
        return false;
    }
}
