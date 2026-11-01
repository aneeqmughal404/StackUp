using System.Collections;
using UnityEngine;

public class PerfectUI : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private float moveDistance = 100f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 startPosition;
    private Coroutine animationCoroutine;

    //==================================================

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        startPosition = rectTransform.anchoredPosition;

        gameObject.SetActive(false);
    }

    //==================================================

    public void Show()
    {
        gameObject.SetActive(true);

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(PlayAnimation());
    }

    //==================================================

    private IEnumerator PlayAnimation()
    {
        gameObject.SetActive(true);

        rectTransform.anchoredPosition = startPosition;
        canvasGroup.alpha = 1f;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPosition,
                             startPosition + Vector2.up * moveDistance,
                             t);

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        gameObject.SetActive(false);
        animationCoroutine = null;
    }
}