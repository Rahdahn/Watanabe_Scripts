using UnityEngine;
using UnityEngine.Events;

public class SpinHandler : MonoBehaviour
{
    [SerializeField] private Transform center;               // ��]�̒��S
    [SerializeField] private Transform rotateTarget;         // �����ڂƂ��ĉ�]���������I�u�W�F�N�g
    [SerializeField] private Transform handleColliderTarget; // �v���C���[���G��R���C�_�[�̂��镔��
    [SerializeField] private float requiredTotalAngle = 360f;

    public UnityEvent onSpinComplete;

    private float totalRotation = 0f;
    private Vector2 prevPos;
    private bool spinning = false;

    void Update()
    {
        Vector2 current = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);

        if (PlayerInputReader.Instance.ClickStarted)
        {
            Collider2D hit = Physics2D.OverlapPoint(current);
            if (hit != null && hit.transform == handleColliderTarget)
            {
                spinning = true;
                prevPos = current;
                totalRotation = 0f;
            }
        }

        if (PlayerInputReader.Instance.ClickHeld && spinning)
        {
            float angle = Vector2.SignedAngle(prevPos - (Vector2)center.position, current - (Vector2)center.position);
            totalRotation += Mathf.Abs(angle);
            prevPos = current;

            rotateTarget.Rotate(Vector3.forward, angle);

            if (totalRotation >= requiredTotalAngle)
            {
                spinning = false;
                onSpinComplete?.Invoke();
            }
        }

        if (PlayerInputReader.Instance.ClickReleased)
        {
            spinning = false;
        }
    }
}
