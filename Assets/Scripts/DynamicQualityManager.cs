using System.Collections;
using UnityEngine;

public class DynamicQualityManager : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private int minimumTarget = 60;
    [SerializeField] private float checkInterval = 1f;
    [SerializeField] private float lowerThresholdSeconds = 2f;
    [SerializeField] private float raiseThresholdSeconds = 4f;

    private float belowTimer;
    private float aboveTimer;
    private int currentQuality;

    private void Start()
    {
        currentQuality = QualitySettings.GetQualityLevel();
        StartCoroutine(CheckQualityRoutine());
    }

    private IEnumerator CheckQualityRoutine()
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(checkInterval);

        while (true)
        {
            yield return wait;

            float fps = 1f / Time.unscaledDeltaTime;

            if (fps < minimumTarget)
            {
                belowTimer += checkInterval;
                aboveTimer = 0f;
            }
            else
            {
                aboveTimer += checkInterval;
                belowTimer = 0f;
            }

            if (belowTimer >= lowerThresholdSeconds)
            {
                int nextQuality = Mathf.Max(0, currentQuality - 1);
                if (nextQuality != currentQuality)
                {
                    QualitySettings.SetQualityLevel(nextQuality, true);
                    currentQuality = nextQuality;
                }

                belowTimer = 0f;
            }

            if (aboveTimer >= raiseThresholdSeconds)
            {
                int maxQuality = QualitySettings.names.Length - 1;
                int nextQuality = Mathf.Min(maxQuality, currentQuality + 1);
                if (nextQuality != currentQuality)
                {
                    QualitySettings.SetQualityLevel(nextQuality, true);
                    currentQuality = nextQuality;
                }

                aboveTimer = 0f;
            }
        }
    }
}
