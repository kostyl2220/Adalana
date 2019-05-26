using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialFill : MonoBehaviour
{

    // Public UI References
    public Image fillImage;
    public Text displayText;
    public Text levelText;

    // Trackers for min/max values
    protected float maxValue = 2f, minValue = 0f;

    // Create a property to handle the slider's value
    private float currentValue = 0f;

    public void SetCurrentValue(float value, float maxV)
    {
        maxValue = maxV;
        // Ensure the passed value falls within min/max range
        currentValue = Mathf.Clamp(value, minValue, maxValue);

        // Calculate the current fill percentage and display it
        float fillPercentage = currentValue / maxValue;
        fillImage.fillAmount = fillPercentage;
        displayText.text = currentValue + "/" + maxValue;
    }

    protected void Start()
    {
        SetCurrentValue(0, maxValue);
    }
}
