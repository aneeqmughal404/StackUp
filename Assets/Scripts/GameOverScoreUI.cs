using UnityEngine;
using UnityEngine.UI;

public class GameOverScoreUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private void OnEnable()
    {
        if (ScoreManager.Instance != null)
            scoreText.text = ScoreManager.Instance.GetScore().ToString();
    }
}