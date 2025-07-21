using UnityEngine;
using UnityEngine.UI;

public class OxygenSystem : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float oxygenConsumptionRate = 5f;
    public float oxygenRegenRate = 10f;
    public Slider oxygenSlider;

    [Header("Environment")]
    public bool isInside = false; // Выставляется триггерами

    void Start()
    {
        currentOxygen = maxOxygen;
        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;
        }
    }

    void Update()
    {
        if (isInside)
        {
            currentOxygen += oxygenRegenRate * Time.deltaTime;
        }
        else
        {
            currentOxygen -= oxygenConsumptionRate * Time.deltaTime;
        }

        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        // Реакция на отсутствие кислорода
        if (currentOxygen <= 0f)
        {
            Debug.LogWarning("Игрок задыхается!");
            // TODO: Урон или смерть игрока
        }
    }

    public void SetInsideState(bool value)
    {
        isInside = value;
    }
}
