using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField, Range(0f, 1f)] private float gameplayMusicVolume = 0.35f;
    [SerializeField, Range(0f, 1f)] private float pauseMusicVolume = 0.15f;
    [SerializeField] private float fadeSpeed = 3f;

    [Header("SFX")]
    [SerializeField] private AudioClip placeClip;
    [SerializeField] private AudioClip perfectClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip buttonClip;

    private float targetVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Load saved states
        bool musicEnabled = PlayerPrefs.GetInt("Music", 1) == 1;
        bool sfxEnabled = PlayerPrefs.GetInt("SFX", 1) == 1;

        SetMusicEnabled(musicEnabled);
        SetSFXEnabled(sfxEnabled);

        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            targetVolume = gameplayMusicVolume;
            musicSource.volume = gameplayMusicVolume;

            if (!musicSource.isPlaying)
                musicSource.Play();
        }
    }

    private void Update()
    {
        if (musicSource == null)
            return;

        musicSource.volume = Mathf.MoveTowards(
            musicSource.volume,
            targetVolume,
            fadeSpeed * Time.unscaledDeltaTime);
    }

    #region Music

    public void PauseMusic()
    {
        targetVolume = pauseMusicVolume;
    }

    public void ResumeMusic()
    {
        targetVolume = gameplayMusicVolume;
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }

    public void SetMusicEnabled(bool enabled)
    {
        if (musicSource != null)
            musicSource.mute = !enabled;
    }

    public bool IsMusicEnabled()
    {
        return musicSource != null && !musicSource.mute;
    }

    #endregion

    #region SFX

    public void SetSFXEnabled(bool enabled)
    {
        if (SFXSource != null)
            SFXSource.mute = !enabled;
    }

    public bool IsSFXEnabled()
    {
        return SFXSource != null && !SFXSource.mute;
    }

    public void PlayPlace()
    {
        if (SFXSource != null && placeClip != null)
            SFXSource.PlayOneShot(placeClip);
    }

    public void PlayPerfect()
    {
        if (SFXSource != null && perfectClip != null)
            SFXSource.PlayOneShot(perfectClip);
    }

    public void PlayGameOver()
    {
        if (SFXSource != null && gameOverClip != null)
            SFXSource.PlayOneShot(gameOverClip);
    }

    public void PlayButton()
    {
        if (SFXSource != null && buttonClip != null && !SFXSource.mute)
            SFXSource.PlayOneShot(buttonClip);
    }

    #endregion
}