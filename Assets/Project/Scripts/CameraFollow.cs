using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -10f);
    public float smoothSpeed = 5f;

    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;

        if (useBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.fixedDeltaTime);
    }

}
