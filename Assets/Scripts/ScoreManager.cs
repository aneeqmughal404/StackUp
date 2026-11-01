using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnHighScoreChanged;

    private const string HighScoreKey = "HighScore";

    private int score;
    private int highScore;

    //==================================================
    // Unity
    //==================================================

    private void Awake()
    {
        // PlayerPrefs.DeleteKey("HighScore");
        // PlayerPrefs.Save();

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        score = 0;
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (Debug.isDebugBuild)
            Debug.Log($"High Score Loaded: {highScore}");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    //==================================================
    // Score
    //==================================================

    public void AddScore(int amount)
    {
        score += amount;

        if (Debug.isDebugBuild)
        {
            Debug.Log($"+{amount}");
            Debug.Log($"Score: {score}");
        }

        OnScoreChanged?.Invoke(score);

        if (score > highScore)
        {
            highScore = score;

            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();

            if (Debug.isDebugBuild)
                Debug.Log($"NEW HIGH SCORE: {highScore}");

            OnHighScoreChanged?.Invoke(highScore);
        }
    }

    public void ResetScore()
    {
        score = 0;

        if (Debug.isDebugBuild)
            Debug.Log("Score Reset");

        OnScoreChanged?.Invoke(score);
    }

    //==================================================
    // Getters
    //==================================================

    public int GetScore()
    {
        return score;
    }

    public int GetHighScore()
    {
        return highScore;
    }
}