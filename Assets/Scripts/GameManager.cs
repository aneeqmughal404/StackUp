using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    WaitingToStart,
    Playing,
    GameOver,
    Paused
}

public class GameManager : MonoBehaviour
{
    private const string GameplaySceneName = "GamePlay";

    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.WaitingToStart;

    private void Awake()
    {
        // Keep this manager local to the gameplay scene so a restart reinitializes it cleanly.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void StartGame()
    {
        if (CurrentState == GameState.Playing)
            return;

        CurrentState = GameState.Playing;
        Debug.Log("GameManager: StartGame — state set to Playing");
    }

    public void GameOver()
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentState = GameState.GameOver;

        GameplayUIService.ShowGameOver();
    }

    public void RestartGame()
    {
        Debug.Log("GameManager: RestartGame invoked");

        AudioManager.Instance?.PlayButton();
        Time.timeScale = 1f;

        SceneManager.LoadScene(GameplaySceneName);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
            CurrentState = GameState.Paused;
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
            CurrentState = GameState.Playing;
    }
}