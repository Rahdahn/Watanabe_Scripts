using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class SwipeLoopHandler : MonoBehaviour
{
    [Header("�X���C�v�ݒ�")]
    [SerializeField] private int requiredSwipes = 3; // ������
    [SerializeField] private float swipeThreshold = 50f;

    [Header("����Ώ�")]
    [SerializeField] private Transform swipeTarget;       // �v���C���[���G��Ώ�
    [SerializeField] private Transform flipVisualTarget;  // �����ڂ𔽓]����Ώ�

    [Header("�r�W���A���ݒ�")]
    [SerializeField] private bool Flip = true;

    [Header("�����C�x���g")]
    public UnityEvent onSwipesComplete;

    private int swipeCount = 0;
    private Vector2 lastSwipePos;
    private bool expectingRight = true;
    private bool isSwiping = false;
    private bool isCompleted = false;

    void Update()
    {
        if (PlayerInputReader.Instance == null || swipeTarget == null || isCompleted)
            return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);

        if (!isSwiping && PlayerInputReader.Instance.ClickStarted)
        {
            Collider2D hit = Physics2D.OverlapPoint(worldPos);
            if (hit != null && hit.transform == swipeTarget)
            {
                isSwiping = true;
                lastSwipePos = PlayerInputReader.Instance.PointerPosition;
            }
        }

        if (PlayerInputReader.Instance.ClickHeld && isSwiping)
        {
            Vector2 current = PlayerInputReader.Instance.PointerPosition;
            float deltaX = current.x - lastSwipePos.x;

            if (expectingRight && deltaX > swipeThreshold)
            {
                swipeCount++;
                expectingRight = false;
                lastSwipePos = current;
                if (Flip) FlipVisual(180f);
                Debug.Log($"���X���C�v {swipeCount}/{requiredSwipes * 2}");
            }
            else if (!expectingRight && deltaX < -swipeThreshold)
            {
                swipeCount++;
                expectingRight = true;
                lastSwipePos = current;
                if (Flip) FlipVisual(0f);
                Debug.Log($"���X���C�v {swipeCount}/{requiredSwipes * 2}");
            }

            if (swipeCount >= requiredSwipes * 2)
            {
                isCompleted = true;
                isSwiping = false;
                Debug.Log("�X���C�v�����I");
                onSwipesComplete?.Invoke();
            }
        }

        if (PlayerInputReader.Instance.ClickReleased)
        {
            isSwiping = false;
        }
    }

    private void FlipVisual(float yRotation)
    {
        if (flipVisualTarget != null)
        {
            flipVisualTarget.DORotate(new Vector3(0, yRotation, 0), 0.3f)
                .SetEase(Ease.OutQuad);
        }
    }

    public void ResetSwipe()
    {
        swipeCount = 0;
        isCompleted = false;
        isSwiping = false;
        expectingRight = true;
        if (Flip) FlipVisual(0f);
    }
}
