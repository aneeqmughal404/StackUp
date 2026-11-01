using UnityEngine;

public class FPSManager : MonoBehaviour
{
    private void Awake()
    {
        ApplyFrameRate();
    }

    private void OnEnable()
    {
        ApplyFrameRate();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            ApplyFrameRate();
    }

    private void ApplyFrameRate()
    {
        // Disable VSync to strictly adhere to the targetFrameRate
        QualitySettings.vSyncCount = 0;
        
        // Dynamically get the device's screen refresh rate
#if UNITY_2022_2_OR_NEWER
        Application.targetFrameRate = (int)Mathf.Round((float)Screen.currentResolution.refreshRateRatio.value);
#else
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
#endif
        Debug.Log("Target Frame Rate applied: " + Application.targetFrameRate);
    }
}