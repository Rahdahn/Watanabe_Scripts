using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class SwipeLoopHandler : MonoBehaviour
{
    [Header("スワイプ設定")]
    [SerializeField] private int requiredSwipes = 3; // 往復回数
    [SerializeField] private float swipeThreshold = 50f;

    [Header("操作対象")]
    [SerializeField] private Transform swipeTarget;       // プレイヤーが触る対象
    [SerializeField] private Transform flipVisualTarget;  // 見た目を反転する対象

    [Header("ビジュアル設定")]
    [SerializeField] private bool Flip = true;

    [Header("イベント")]
    public UnityEvent onSwipesComplete;
    public UnityEvent onSwipe;

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
                onSwipe?.Invoke();
                Debug.Log($"→スワイプ {swipeCount}/{requiredSwipes * 2}");
            }
            else if (!expectingRight && deltaX < -swipeThreshold)
            {
                swipeCount++;
                expectingRight = true;
                lastSwipePos = current;
                if (Flip) FlipVisual(0f);
                onSwipe?.Invoke();
                Debug.Log($"←スワイプ {swipeCount}/{requiredSwipes * 2}");
            }

            if (swipeCount >= requiredSwipes * 2)
            {
                isCompleted = true;
                isSwiping = false;
                Debug.Log("スワイプ完了！");
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
