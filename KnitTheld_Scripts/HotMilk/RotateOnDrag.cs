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

        // �h���b�O�J�n
        if (input.ClickStarted)
        {
            Collider2D hit = Physics2D.OverlapPoint(pointerWorldPos);
            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                lastPointerPos = pointerWorldPos;
            }
        }

        // �h���b�O��
        if (isDragging && input.ClickHeld)
        {
            Vector2 currentPos = pointerWorldPos;
            Vector2 delta = currentPos + lastPointerPos;

            if (delta.sqrMagnitude > 0.0001f)
            {
                // �h���b�O�����̊p�x���v�Z
                //targetAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
                targetAngle = Mathf.Round((Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg) / _increment) * _increment;
            }

            lastPointerPos = currentPos;
        }

        // �h���b�O�I��
        if (input.ClickReleased)
        {
            isDragging = false;
        }

        float currentAngle = transform.eulerAngles.z;
        float smoothAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }
}
