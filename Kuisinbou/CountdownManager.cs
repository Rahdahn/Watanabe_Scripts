using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float textScaleUp = 3f;
    [SerializeField] private float duration = 0.6f;

    private readonly string[] countdownStrings = { "3", "2", "1", "GO" };

    public static bool IsCountingDown { get; private set; }

    private void Start()
    {
        StartCountdown();
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        Time.timeScale = 0f;
        IsCountingDown = true;

        foreach (string count in countdownStrings)
        {
            yield return new WaitForSecondsRealtime(0.5f);

            countdownText.gameObject.SetActive(true);
            countdownText.text = count;

            countdownText.transform.localScale = Vector3.zero;
            countdownText.DOFade(1f, 0f).SetUpdate(true);
            countdownText.transform.DOScale(textScaleUp, duration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
            countdownText.DOFade(0f, duration)
                .SetEase(Ease.InQuad)
                .SetDelay(0.2f)
                .SetUpdate(true);

            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1f;
        IsCountingDown = false;
    }
}
