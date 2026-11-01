using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BackgroundColorManager : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private float transitionSpeed = 2f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (ColorManager.Instance != null)
            cam.backgroundColor = ColorManager.Instance.GetBackgroundColor();
    }

    private void Update()
    {
        if (ColorManager.Instance == null)
            return;

        cam.backgroundColor = Color.Lerp(
            cam.backgroundColor,
            ColorManager.Instance.GetBackgroundColor(),
            transitionSpeed * Time.deltaTime);
    }
}