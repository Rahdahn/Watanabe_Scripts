using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private bool isFading = false;

    private Rigidbody2D[] pausedBodies;
    private Dictionary<Rigidbody2D, RigidbodyType2D> originalBodyTypes = new();

    private readonly string[] playerTags = { "Player1", "Player2", "Player3", "Player4" };

    private void PausePhysics()
    {
        originalBodyTypes.Clear();

        foreach (string tag in playerTags)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject player in players)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    originalBodyTypes[rb] = rb.bodyType;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
            }
        }
    }

    private void ResumePhysics()
    {
        foreach (var kvp in originalBodyTypes)
        {
            if (kvp.Key != null)
            {
                kvp.Key.bodyType = kvp.Value;
            }
        }

        originalBodyTypes.Clear();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;
    }

    public void FadeToScene(string sceneName, Action onFadeInComplete = null)
    {
        if (!isFading)
        {
            StartCoroutine(FadeOutAndLoadScene(sceneName, onFadeInComplete));
        }
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName, Action onFadeInComplete)
    {
        isFading = true;

        PlayerView.IsInputLocked = true;
        PausePhysics();

        yield return fadeImage.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();
        yield return SceneManager.LoadSceneAsync(sceneName);

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);

        ResumePhysics();
        PlayerView.IsInputLocked = false;

        yield return fadeImage.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).WaitForCompletion();

        isFading = false;
        onFadeInComplete?.Invoke();
    }


}