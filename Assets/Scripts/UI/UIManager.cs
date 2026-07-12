using System.Collections;
using UnityEngine;

public class UIManager : GameBehaviour
{
    [SerializeField] private CanvasGroupFade blackoutFade;

    private void Awake()
    {
        if (blackoutFade == null)
            blackoutFade = GetComponent<CanvasGroupFade>();

        blackoutFade?.SetAlpha(1f);
    }

    private void Start()
    {
        blackoutFade?.FadeOut();
    }

    public IEnumerator FadeToBlack()
    {
        if (blackoutFade == null)
            yield break;

        bool completed = false;
        void HandleFadeCompleted() => completed = true;

        blackoutFade.FadeCompleted += HandleFadeCompleted;
        blackoutFade.FadeIn();

        while (!completed)
            yield return null;

        blackoutFade.FadeCompleted -= HandleFadeCompleted;
    }
}
