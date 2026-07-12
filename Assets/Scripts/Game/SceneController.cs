using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : GameBehaviour
{
    private const string GameOverScene = "GameOver";

    [SerializeField, Min(0f)] private float lanternFadeDuration = 0.5f;

    private bool isTransitioning;

    public static LanternColor GameOverLanternColor { get; private set; } = LanternColor.DEFAULT;

    private void Start()
    {
        gameManager.OnGameOver += GameOver;
    }

    private void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnGameOver -= GameOver;
    }

    private void GameOver()
    {
        if (isTransitioning)
            return;

        isTransitioning = true;
        StartCoroutine(GameOverTransition());
    }

    private IEnumerator GameOverTransition()
    {
        gameManager.Player.Movement.Stop();
        gameManager.Player.enabled = false;

        if (gameManager.Player.Lantern != null)
            GameOverLanternColor = gameManager.Player.Lantern.Color;

        yield return FadeLantern();
        if (gameManager.UI != null)
            yield return gameManager.UI.FadeToBlack();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(GameOverScene);
        while (loadOperation != null && !loadOperation.isDone)
            yield return null;
    }

    private IEnumerator FadeLantern()
    {
        Light lantern = gameManager.Player.Lantern?.Light;
        if (lantern == null)
            yield break;

        float startIntensity = lantern.intensity;
        float elapsed = 0f;

        while (elapsed < lanternFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = lanternFadeDuration > 0f
                ? Mathf.Clamp01(elapsed / lanternFadeDuration)
                : 1f;
            lantern.intensity = Mathf.Lerp(startIntensity, 0f, progress);
            yield return null;
        }

        lantern.intensity = 0f;
    }
}
