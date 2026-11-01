using UnityEngine;
using UnityEngine.UI;

public class MainMenuHighScoreUI : MonoBehaviour
{
    [SerializeField] private Text highScoreText;

    private void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highScore.ToString();
    }
}