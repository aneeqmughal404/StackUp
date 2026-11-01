using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject playButton;
    public GameObject musicButton;
    public GameObject SFXButton;
    public GameObject exitButton;
    public GameObject record;
    public GameObject highScore;

    private bool musicOn;
    private bool sfxOn;

    private void Start()
    {
        playButton.SetActive(true);
        musicButton.SetActive(true);
        SFXButton.SetActive(true);
        exitButton.SetActive(true);
        record.SetActive(true);
        highScore.SetActive(true);

        LoadAudioState();
    }

    public void PlayGame()
    {
        AudioManager.Instance?.PlayButton();
        SceneManager.LoadScene("GamePlay");
    }

    public void ToggleMusic()
    {
        musicOn = !musicOn;

        AudioManager.Instance?.SetMusicEnabled(musicOn);

        PlayerPrefs.SetInt("Music", musicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX()
    {
        sfxOn = !sfxOn;

        AudioManager.Instance?.SetSFXEnabled(sfxOn);

        // SFX ON karte waqt click sound play hogi
        if (sfxOn)
            AudioManager.Instance?.PlayButton();

        PlayerPrefs.SetInt("SFX", sfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadAudioState()
    {
        musicOn = PlayerPrefs.GetInt("Music", 1) == 1;
        sfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;

        AudioManager.Instance?.SetMusicEnabled(musicOn);
        AudioManager.Instance?.SetSFXEnabled(sfxOn);
    }

    public void ExitGame()
    {
        StartCoroutine(ExitRoutine());
    }

    private IEnumerator ExitRoutine()
    {
        // Agar SFX ON hai to button sound play hogi
        AudioManager.Instance?.PlayButton();

        yield return new WaitForSeconds(0.1f);

        Debug.Log("Game Exit");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}