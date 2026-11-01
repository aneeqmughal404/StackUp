using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class WebAndroidMenu : MonoBehaviour
{
    public GameObject MobileCanvas; // Assign your on-screen buttons in the Unity Inspector
    public GameObject webCanvas; // Assign your on-screen buttons in the Unity Inspector

    // [SerializeField] private GameplayUIManagerMobile mobileUIManager;
    // [SerializeField] private GameplayUIManager webUIManager;

    void Start()
    {
        if (isMobile())
        {
            MobileCanvas.SetActive(true);
            webCanvas.SetActive(false);

            // mobileUIManager.enabled = true;
            // webUIManager.enabled = false;
        }
        else
        {
            MobileCanvas.SetActive(false);
            webCanvas.SetActive(true);

            // mobileUIManager.enabled = false;
            // webUIManager.enabled = true;
        }
    }

    #region WebGL is on mobile check

    #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool IsMobile();
    #else
        private static bool IsMobile() { return false; } // Stub for non-WebGL platforms
    #endif

    public bool isMobile()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            return IsMobile();
        #else
            return false;
        #endif
    }
    
    #endregion
}