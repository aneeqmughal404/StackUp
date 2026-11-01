using UnityEngine;

public static class GameplayUIService
{
    public static bool IgnoreNextGameplayInput { get; set; }

    public static void ShowGameplay()
    {
        InvokeOnActiveManagers(ui => ui.ShowGameplay(), ui => ui.ShowGameplay());
    }

    public static void ShowPauseMenu()
    {
        InvokeOnActiveManagers(ui => ui.ShowPauseMenu(), ui => ui.ShowPauseMenu());
    }

    public static void ShowPerfect()
    {
        InvokeOnActiveManagers(ui => ui.ShowPerfect(), ui => ui.ShowPerfect());
    }

    public static void ResumeGame()
    {
        InvokeOnActiveManagers(ui => ui.ResumeGame(), ui => ui.ResumeGame());
    }

    public static void ShowGameOver()
    {
        InvokeOnActiveManagers(ui => ui.ShowGameOver(), ui => ui.ShowGameOver());
    }

    // public static void ToggleSound()
    // {
    //     InvokeOnActiveManagers(ui => ui.ToggleSound(), ui => ui.ToggleSound());
    // }

    public static void HiddenGodModeTap()
    {
        InvokeOnActiveManagers(ui => ui.HiddenGodModeTap(), ui => ui.HiddenGodModeTap());
    }

    public static void GoHome()
    {
        InvokeOnActiveManagers(ui => ui.GoHome(), ui => ui.GoHome());
    }

    private static void InvokeOnActiveManagers(System.Action<GameplayUIManager> webAction, System.Action<GameplayUIManagerMobile> mobileAction)
    {
        GameplayUIManagerMobile mobileUi = Object.FindFirstObjectByType<GameplayUIManagerMobile>();
        if (mobileUi != null && mobileUi.isActiveAndEnabled && mobileUi.gameObject.activeInHierarchy)
        {
            mobileAction?.Invoke(mobileUi);
            return;
        }

        GameplayUIManager webUi = Object.FindFirstObjectByType<GameplayUIManager>();
        if (webUi != null && webUi.isActiveAndEnabled && webUi.gameObject.activeInHierarchy)
        {
            webAction?.Invoke(webUi);
        }
    }
}
