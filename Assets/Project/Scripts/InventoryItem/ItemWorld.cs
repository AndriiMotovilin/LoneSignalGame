using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public PickableItem itemData;
    public Renderer itemRenderer;
    public Material highlightMat;
    public GameObject pickupHintUI;
    public bool canBePickedUp = true;


    private Material defaultMat;

    void Start()
    {
        if (itemRenderer != null)
            defaultMat = itemRenderer.material;

        if (pickupHintUI != null)
            pickupHintUI.SetActive(false);
    }

    public void Highlight(bool on)
    {
        if (itemRenderer != null && highlightMat != null && defaultMat != null)
            itemRenderer.material = on ? highlightMat : defaultMat;

        if (pickupHintUI != null)
            pickupHintUI.SetActive(on);
    }
}
