using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUIManagerMobile : MonoBehaviour
{
    [Header("Gameplay UI")]
    public GameObject gamePlay;
    public GameObject pauseButton;
    public GameObject perfectText;
    
    [Header("Pause")]
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public GameObject restartButton;
    public GameObject pauseSoundButton;

    [Header("Game Over")]
    public GameObject gameOver;
    public GameObject restartGameButton;
    public GameObject result;
    public GameObject score;
    public static bool IgnoreNextGameplayInput
    {
        get => GameplayUIService.IgnoreNextGameplayInput;
        set => GameplayUIService.IgnoreNextGameplayInput = value;
    }

    private int godModeTapCount;
    private float firstTapTime;

    //==================================================

    private void Start()
    {
        Time.timeScale = 1f;
        ShowGameplay();
    }

    //==================================================

    public void ShowGameplay()
    {
        gamePlay.SetActive(true);
        pauseButton.SetActive(true);
        SetPauseMenuVisible(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;

        IgnoreNextGameplayInput = false;

        GameManager.Instance?.ResumeGame();
        AudioManager.Instance?.ResumeMusic();
    }

    //==================================================

    public void ShowPauseMenu()
    {
        IgnoreNextGameplayInput = true;

        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState != GameState.Playing)
            return;

        GameManager.Instance?.PauseGame();

        SetPauseMenuVisible(true);

        Time.timeScale = 0f;

        AudioManager.Instance?.PauseMusic();
    }

    public void ShowPerfect()
    {
        perfectText.SetActive(true);

        PerfectUI perfectUI = perfectText.GetComponent<PerfectUI>();

        if (perfectUI != null)
            perfectUI.Show();
    }

    //==================================================

    public void ResumeGame()
    {
        GameManager.Instance?.ResumeGame();

        SetPauseMenuVisible(false);

        Time.timeScale = 1f;

        AudioManager.Instance?.ResumeMusic();
    }

    //==================================================

    public void ShowGameOver()
    {
        SetPauseMenuVisible(false);

        gamePlay.SetActive(false);

        pauseButton.SetActive(false);

        
        gameOver.SetActive(true);
        result.SetActive(true);
        score.SetActive(true);

        Time.timeScale = 0f;

        AudioManager.Instance?.PauseMusic();
    }

    private void SetPauseMenuVisible(bool isVisible)
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(isVisible);

        if (pauseButton != null)
            pauseButton.SetActive(!isVisible);

        Canvas.ForceUpdateCanvases();
    }

    // public void ToggleSound()
    // {
    //     AudioManager.Instance?.ToggleSound();
    // }

    public void HiddenGodModeTap()
    {
        if (godModeTapCount == 0)
            firstTapTime = Time.unscaledTime;

        if (Time.unscaledTime - firstTapTime > 2f)
        {
            godModeTapCount = 0;
            firstTapTime = Time.unscaledTime;
        }

        godModeTapCount++;

        if (godModeTapCount >= 5)
        {
            GodModeManager.Instance.EnableGodMode();

            godModeTapCount = 0;
        }

        Debug.Log($"GodMode: {godModeTapCount}/5");
    }

    public void GoHome()
    {
        AudioManager.Instance?.PlayButton();

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}