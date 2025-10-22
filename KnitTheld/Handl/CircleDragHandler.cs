using UnityEngine;
using UnityEngine.Events;

public class CircleDragHandler : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private float requiredLoops = 1;
    [SerializeField] private UnityEvent onCompleteCircle;

    private float angleSum = 0f;
    private Vector2 lastPos;
    private bool dragging = false;

    void Update()
    {
        Vector2 current = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);

        if (PlayerInputReader.Instance.ClickStarted)
        {
            dragging = true;
            angleSum = 0f;
            lastPos = current;
        }

        if (PlayerInputReader.Instance.ClickHeld && dragging)
        {
            float angle = Vector2.SignedAngle(lastPos - (Vector2)center.position, current - (Vector2)center.position);
            angleSum += Mathf.Abs(angle);
            lastPos = current;

            if (angleSum >= 360f * requiredLoops)
            {
                dragging = false;
                onCompleteCircle?.Invoke();
            }
        }

        if (PlayerInputReader.Instance.ClickReleased)
        {
            dragging = false;
        }
    }
}
