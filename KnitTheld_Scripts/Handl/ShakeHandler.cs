using UnityEngine;
using UnityEngine.Events;

public class ShakeHandler : MonoBehaviour
{
    [Header("�V�F�C�N���x")]
    [SerializeField] private float shakeThreshold = 3.0f; // �����x��臒l
    [SerializeField] private float minShakeInterval = 0.3f; // �A�����o��h���ŏ��Ԋu

    [Header("�V�F�C�N��")]
    [SerializeField] private int requiredShakeCount = 3;
    [SerializeField] private float resetTime = 2.0f; // ���̎��ԓ���requiredShakeCount�ɒB���Ȃ��ꍇ���Z�b�g

    [Header("�V�F�C�N���o���̃C�x���g")]
    public UnityEvent onShakeDetected;

    private int shakeCounter = 0;
    private float lastShakeTime = 0f;
    private float firstShakeTime = 0f;

    void Update()
    {
        Vector3 acceleration = Input.acceleration;

        // �����x�̑傫�����v�Z
        float accelMagnitude = acceleration.sqrMagnitude;

        if (accelMagnitude >= shakeThreshold * shakeThreshold)
        {
            float currentTime = Time.time;

            // �A������h�~
            if (currentTime - lastShakeTime >= minShakeInterval)
            {
                lastShakeTime = currentTime;

                if (shakeCounter == 0)
                {
                    firstShakeTime = currentTime;
                }

                shakeCounter++;

                // ���Z�b�g���Ԃ𒴂����ꍇ�̓J�E���g���ŏ�����
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
