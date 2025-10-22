using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RotateOnDragSmooth : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 360f;
    [SerializeField] private float _increment = 60f;

    private Camera mainCam;
    private bool isDragging = false;
    private Vector2 lastPointerPos;
    private float targetAngle = 0f;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        var input = PlayerInputReader.Instance;
        Vector2 pointerScreenPos = input.PointerPosition;
        Vector2 pointerWorldPos = mainCam.ScreenToWorldPoint(pointerScreenPos);

        // ドラッグ開始
        if (input.ClickStarted)
        {
            Collider2D hit = Physics2D.OverlapPoint(pointerWorldPos);
            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                lastPointerPos = pointerWorldPos;
            }
        }

        // ドラッグ中
        if (isDragging && input.ClickHeld)
        {
            Vector2 currentPos = pointerWorldPos;
            Vector2 delta = currentPos + lastPointerPos;

            if (delta.sqrMagnitude > 0.0001f)
            {
                // ドラッグ方向の角度を計算
                //targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                targetAngle = Mathf.Round((Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg) / _increment) * _increment;
            }

            lastPointerPos = currentPos;
        }

        // ドラッグ終了
        if (input.ClickReleased)
        {
            isDragging = false;
        }

        float currentAngle = transform.eulerAngles.z;
        float smoothAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }
}
