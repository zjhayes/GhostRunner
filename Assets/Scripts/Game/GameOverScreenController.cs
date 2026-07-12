using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreenController : MonoBehaviour
{
    private const string GameScene = "GameScene";

    [SerializeField] private FlipbookAnimator flipbookAnimator;
    [SerializeField] private FlipbookAnimation gameOverAnimation;
    [SerializeField] private CanvasGroupFade canvasGroupFade;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private Light lanternLight;
    [SerializeField] private Button restartButton;
    [SerializeField] private Transform movingObject;
    [SerializeField] private float startZ = 20f;
    [SerializeField] private float endZ = 4f;

    private float animationDuration;

    private void Awake()
    {
        if (flipbookAnimator == null)
        {
            flipbookAnimator = GetComponentInChildren<FlipbookAnimator>();
        }

        if (lanternLight != null)
            lanternLight.color = SceneController.GameOverLanternColor.ToColor();
    }

    private void OnEnable()
    {
        if (flipbookAnimator != null)
        {
            flipbookAnimator.AnimationCompleted += HandleAnimationCompleted;
        }

        restartButton?.onClick.AddListener(RestartGame);
    }

    private void OnDisable()
    {
        if (flipbookAnimator != null)
        {
            flipbookAnimator.AnimationCompleted -= HandleAnimationCompleted;
        }

        restartButton?.onClick.RemoveListener(RestartGame);
    }

    private void Start()
    {
        if (flipbookAnimator == null || gameOverAnimation == null)
        {
            Debug.LogWarning($"{nameof(GameOverScreenController)} needs a flipbook animator and game over animation.", this);
            return;
        }

        animationDuration = GetAnimationDuration();

        SetMovingObjectZ(startZ);
        flipbookAnimator.Play(gameOverAnimation, false);
        StartCoroutine(MoveObjectRoutine());
    }

    private IEnumerator MoveObjectRoutine()
    {
        if (movingObject == null)
        {
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = animationDuration > 0f
                ? Mathf.Clamp01(elapsedTime / animationDuration)
                : 1f;
            SetMovingObjectZ(Mathf.Lerp(startZ, endZ, progress));
            yield return null;
        }

        SetMovingObjectZ(endZ);
    }

    private void HandleAnimationCompleted()
    {
        SetMovingObjectZ(endZ);
        canvasGroupFade?.FadeIn();
        musicManager?.Play();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(GameScene);
    }

    private float GetAnimationDuration()
    {
        if (gameOverAnimation.frameCount <= 1 || gameOverAnimation.framesPerSecond <= 0f)
        {
            return 0f;
        }

        return (gameOverAnimation.frameCount - 1) / gameOverAnimation.framesPerSecond;
    }

    private void SetMovingObjectZ(float z)
    {
        if (movingObject == null)
        {
            return;
        }

        Vector3 position = movingObject.position;
        position.z = z;
        movingObject.position = position;
    }
}
