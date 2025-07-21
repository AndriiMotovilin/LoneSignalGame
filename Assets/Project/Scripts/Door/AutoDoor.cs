using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AutoDoor : MonoBehaviour
{
    public Transform doorTransform;
    public Vector3 openOffset = new Vector3(0, 3f, 0);
    public float openCloseSpeed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool playerNearby = false;

    void Start()
    {
        if (doorTransform == null) doorTransform = transform;
        closedPos = doorTransform.position;
        openPos = closedPos + openOffset;
    }

    void Update()
    {
        Vector3 targetPos = playerNearby ? openPos : closedPos;
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
}
