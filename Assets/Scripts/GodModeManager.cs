using UnityEngine;

public class GodModeManager : MonoBehaviour
{
    public static GodModeManager Instance { get; private set; }

    public bool IsGodMode { get; private set; }

    [SerializeField] private float duration = 5f;

    private float timer;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!IsGodMode)
            return;

        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            IsGodMode = false;
            Debug.Log("God Mode Disabled");
        }
    }

    public void EnableGodMode()
    {
        IsGodMode = true;
        timer = duration;

        Debug.Log("God Mode Enabled");
    }
}