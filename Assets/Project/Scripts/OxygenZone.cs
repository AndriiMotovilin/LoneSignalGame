using UnityEngine;

public class OxygenZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var oxygen = other.GetComponent<OxygenSystem>();
            if (oxygen != null) oxygen.SetInsideState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var oxygen = other.GetComponent<OxygenSystem>();
            if (oxygen != null) oxygen.SetInsideState(false);
        }
    }
}
