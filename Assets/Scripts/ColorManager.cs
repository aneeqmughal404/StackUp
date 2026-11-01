using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    [System.Serializable]
    public class ColorTheme
    {
        public Color background;
        public Color block;
    }

    [Header("Themes")]
    [SerializeField] private ColorTheme[] themes = new ColorTheme[0];
    [SerializeField] private int blocksPerTheme = 6;

    private int currentTheme;
    private int blockCounter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log($"ColorManager: Awake - themes.Length={themes.Length}");

        currentTheme = themes.Length > 0 ? Random.Range(0, themes.Length) : 0;
        blockCounter = 0;
    }

    public Color GetBlockColor()
    {
        Color color = themes[currentTheme].block;

        blockCounter++;

        if (blockCounter >= blocksPerTheme)
        {
            blockCounter = 0;
            currentTheme++;

            if (currentTheme >= themes.Length)
                currentTheme = 0;
        }

        return color;
    }

    public Color GetBackgroundColor()
    {
        return themes[currentTheme].background;
    }
}