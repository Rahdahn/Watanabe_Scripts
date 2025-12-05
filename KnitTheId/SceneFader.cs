using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        fadeImage.color = new Color(255, 255, 255, 1f);
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private void FadeIn()
    {
        fadeImage.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad);
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        yield return fadeImage.DOFade(1f, fadeDuration).SetEase(Ease.InQuad).WaitForCompletion();

        yield return SceneManager.LoadSceneAsync(sceneName);

        fadeImage.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad);
    }
}
