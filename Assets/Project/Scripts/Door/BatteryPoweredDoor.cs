using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BatteryPoweredDoor : MonoBehaviour
{
    [Header("Дверь")]
    public Transform doorTransform;
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float openCloseSpeed = 2f;

    [Header("Требуемая батарея")]
    public PickableItem requiredBattery;
    public GameObject visualBatteryPrefab;
    public Transform batteryInsertPoint;

    [Header("Состояние")]
    private bool batteryInserted = false;
    private bool playerNearby = false;
    private GameObject batteryInstance;

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
        if (!batteryInserted && playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TryInsertBattery();
        }

        if (batteryInserted)
        {
            Vector3 target = playerNearby ? openPos : closedPos;
            doorTransform.position = Vector3.Lerp(doorTransform.position, target, Time.deltaTime * openCloseSpeed);
        }
    }

    void TryInsertBattery()
    {
        int index = FindBatteryInInventory();
        if (index != -1)
        {
            Inventory.Instance.items[index] = null;
            Inventory.Instance.SendMessage("RefreshUI", SendMessageOptions.DontRequireReceiver);

            batteryInserted = true;
            Debug.Log("✅ Батарея вставлена. Автодверь активна.");

            SpawnBatteryModel();
        }
        else
        {
            Debug.Log("🚫 Нет нужной батареи в инвентаре.");
        }
    }

    void SpawnBatteryModel()
    {
        if (visualBatteryPrefab != null && batteryInsertPoint != null)
        {
            batteryInstance = Instantiate(visualBatteryPrefab, batteryInsertPoint.position, batteryInsertPoint.rotation);
            batteryInstance.transform.SetParent(batteryInsertPoint);
        }
    }

    int FindBatteryInInventory()
    {
        for (int i = 0; i < Inventory.Instance.items.Count; i++)
        {
            if (Inventory.Instance.items[i] == requiredBattery)
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
