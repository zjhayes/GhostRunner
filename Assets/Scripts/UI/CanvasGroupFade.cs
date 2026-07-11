using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField, Min(0f)] private float duration = 0.5f;
    [SerializeField] private bool useUnscaledTime = true;

    private Coroutine fadeRoutine;

    public event Action FadeCompleted;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void FadeIn()
    {
        FadeTo(1f);
    }

    public void FadeOut()
    {
        FadeTo(0f);
    }

    public void FadeTo(float targetAlpha)
    {
        if (canvasGroup == null)
        {
            Debug.LogWarning($"{nameof(CanvasGroupFade)} needs a canvas group.", this);
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeRoutine(Mathf.Clamp01(targetAlpha)));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        fadeRoutine = null;
        FadeCompleted?.Invoke();
    }
}