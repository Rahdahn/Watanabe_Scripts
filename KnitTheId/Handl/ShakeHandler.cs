using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class ShakeHandler : MonoBehaviour
{
    [Header("シェイク感度")]
    [SerializeField] private float shakeThreshold = 3.0f;
    [SerializeField] private float minShakeInterval = 0.3f;

    [Header("シェイク回数")]
    [SerializeField] private int requiredShakeCount = 3;
    [SerializeField] private float resetTime = 10f;

    [Header("シェイク検出時のイベント")]
    public UnityEvent onShakeDetected;

    [Header("シェイク時に表示する画像")]
    [SerializeField] private Image shakeImage;

    private int shakeCounter = 0;
    private float lastShakeTime = 0f;
    private float firstShakeTime = 0f;

    private int tapCounter = 0;
    private float firstTapTime = 0f;
    private float tapResetTime = 1.0f;
    private int requiredTapCount = 3;

    void Start()
    {
        // イメージは最初非表示にしておく
        if (shakeImage != null)
        {
            shakeImage.gameObject.SetActive(false);
            shakeImage.color = new Color(shakeImage.color.r, shakeImage.color.g, shakeImage.color.b, 0f);
            // 1秒後に表示
            Invoke(nameof(ShowShakeImage), 1f);
        }
    }

    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        float accelMagnitude = acceleration.sqrMagnitude;

        if (accelMagnitude >= shakeThreshold * shakeThreshold)
        {
            float currentTime = Time.time;

            if (currentTime - lastShakeTime >= minShakeInterval)
            {
                lastShakeTime = currentTime;

                if (shakeCounter == 0)
                    firstShakeTime = currentTime;

                shakeCounter++;

                if (currentTime - firstShakeTime > resetTime)
                {
                    shakeCounter = 1;
                    firstShakeTime = currentTime;
                }

                if (shakeCounter >= requiredShakeCount)
                {
                    onShakeDetected?.Invoke();
                    HideShakeImage(); // シェイク終了でイメージを消す
                    shakeCounter = 0;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            onShakeDetected?.Invoke();
            HideShakeImage();
        }

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    float currentTime = Time.time;

                    if (tapCounter == 0)
                        firstTapTime = currentTime;

                    if (currentTime - firstTapTime > tapResetTime)
                    {
                        tapCounter = 1;
                        firstTapTime = currentTime;
                    }
                    else
                    {
                        tapCounter++;
                    }

                    if (tapCounter >= requiredTapCount)
                    {
                        onShakeDetected?.Invoke();
                        HideShakeImage();
                        tapCounter = 0;
                    }
                }
            }
        }
    }

    private void ShowShakeImage()
    {
        if (shakeImage == null) return;

        shakeImage.gameObject.SetActive(true);
        shakeImage.DOFade(1f, 0.5f);
    }

    private void HideShakeImage()
    {
        if (shakeImage == null) return;

        shakeImage.gameObject.SetActive(false);
        shakeImage.color = new Color(shakeImage.color.r, shakeImage.color.g, shakeImage.color.b, 0f);
    }
}
