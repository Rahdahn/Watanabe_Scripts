using UnityEngine;
using UnityEngine.Events;

public class RotateHandler : MonoBehaviour
{
    [Header("回転させるオブジェクト")]
    [SerializeField] private GameObject rotatingObject;

    [Header("回転を認識するエリア")]
    [SerializeField] private Collider2D rotationArea;

    [Header("必要な回転数")]
    [SerializeField] private int requiredRotations;

    [Header("回転完了時に許容する誤差角度")]
    [SerializeField] private float angleThreshold = 15f;

    [Header("イベント")]
    public UnityEvent onRotateStart;
    public UnityEvent onRotateProgress;
    public UnityEvent onRotateSuccess;
    public UnityEvent onRotateFail;

    private bool isRotating = false;
    private float lastAngle = 0f;
    private int rotationCount = 0;
    private bool inArea = false;

    void Update()
    {
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);

        // 回転対象が設定されていない場合は何もしない
        if (rotatingObject == null || rotationArea == null)
            return;

        // ポインタが回転エリア内にあるか確認
        inArea = rotationArea.OverlapPoint(pointerWorldPos);

        // 開始
        if (PlayerInputReader.Instance.ClickStarted && inArea)
        {
            isRotating = true;
            rotationCount = 0;
            lastAngle = GetPointerAngle(pointerWorldPos);
            onRotateStart?.Invoke();
        }

        // 回転中
        if (isRotating && PlayerInputReader.Instance.ClickHeld)
        {
            float currentAngle = GetPointerAngle(pointerWorldPos);
            float delta = Mathf.DeltaAngle(lastAngle, currentAngle);

            // 一定以上の角度変化を検知したら加算
            if (Mathf.Abs(delta) > angleThreshold)
            {
                rotationCount++;
                onRotateProgress?.Invoke();
                lastAngle = currentAngle;

                // 目標回転数に到達したら成功
                if (rotationCount >= requiredRotations)
                {
                    isRotating = false;
                    onRotateSuccess?.Invoke();
                    Debug.Log($"回転成功！ {rotationCount} 回転");
                }
            }
        }

        // 回転キャンセル
        if (PlayerInputReader.Instance.ClickReleased && isRotating)
        {
            isRotating = false;

            if (rotationCount < requiredRotations)
            {
                onRotateFail?.Invoke();
                Debug.Log($"回転失敗（{rotationCount}/{requiredRotations}）");
            }
        }
    }

    /// <summary>
    /// ポインタの角度を取得
    /// </summary>
    private float GetPointerAngle(Vector2 pointerPos)
    {
        Vector2 center = rotationArea.bounds.center;
        Vector2 dir = pointerPos - center;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
