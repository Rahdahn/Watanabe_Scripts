using UnityEngine;
using UnityEngine.Events;

public class ShakeHandler : MonoBehaviour
{
    [Header("シェイク感度")]
    [SerializeField] private float shakeThreshold = 3.0f; // 加速度の閾値
    [SerializeField] private float minShakeInterval = 0.3f; // 連続検出を防ぐ最小間隔

    [Header("シェイク回数")]
    [SerializeField] private int requiredShakeCount = 3;
    [SerializeField] private float resetTime = 2.0f; // この時間内にrequiredShakeCountに達しない場合リセット

    [Header("シェイク検出時のイベント")]
    public UnityEvent onShakeDetected;

    private int shakeCounter = 0;
    private float lastShakeTime = 0f;
    private float firstShakeTime = 0f;

    void Update()
    {
        Vector3 acceleration = Input.acceleration;

        // 加速度の大きさを計算
        float accelMagnitude = acceleration.sqrMagnitude;

        if (accelMagnitude >= shakeThreshold * shakeThreshold)
        {
            float currentTime = Time.time;

            // 連続判定防止
            if (currentTime - lastShakeTime >= minShakeInterval)
            {
                lastShakeTime = currentTime;

                if (shakeCounter == 0)
                {
                    firstShakeTime = currentTime;
                }

                shakeCounter++;

                // リセット時間を超えた場合はカウントを最初から
                if (currentTime - firstShakeTime > resetTime)
                {
                    shakeCounter = 1;
                    firstShakeTime = currentTime;
                }

                if (shakeCounter >= requiredShakeCount)
                {
                    onShakeDetected?.Invoke();
                    shakeCounter = 0;
                }
            }
        }
    }
}
