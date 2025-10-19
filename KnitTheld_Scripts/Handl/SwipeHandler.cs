using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SwipeHandler : MonoBehaviour
{
    public enum SwipeDirection { Up, Down, Left, Right }

    [Tooltip("スワイプ対象オブジェクト（順不同）")]
    [SerializeField] private GameObject[] swipeTargets;

    [Tooltip("要求されるスワイプ方向")]
    [SerializeField] private SwipeDirection requiredDirection = SwipeDirection.Up;

    [Tooltip("スワイプとみなす最低距離（ピクセル）")]
    [SerializeField] private float minSwipeDistance = 100f;

    [Tooltip("各スワイプ検出時に呼ばれるイベント")]
    public UnityEvent onEachSwiped;

    [Tooltip("全てスワイプ完了時に呼ばれるイベント")]
    public UnityEvent onAllSwiped;

    private HashSet<GameObject> swipedObjects = new HashSet<GameObject>();
    private Vector2 startPos;
    private bool tracking = false;

    void Update()
    {
        if (swipeTargets.Length == 0) return;

        if (PlayerInputReader.Instance.ClickStarted)
        {
            startPos = PlayerInputReader.Instance.PointerPosition;
            tracking = true;
        }

        if (PlayerInputReader.Instance.ClickReleased && tracking)
        {
            tracking = false;
            Vector2 endPos = PlayerInputReader.Instance.PointerPosition;
            Vector2 swipe = endPos - startPos;

            if (swipe.magnitude > minSwipeDistance)
            {
                SwipeDirection dir = DetectDirection(swipe);
                if (dir == requiredDirection)
                {
                    Vector2 worldStart = Camera.main.ScreenToWorldPoint(startPos);
                    Collider2D hit = Physics2D.OverlapPoint(worldStart);

                    if (hit != null)
                    {
                        foreach (GameObject target in swipeTargets)
                        {
                            if (hit.gameObject == target && !swipedObjects.Contains(target))
                            {
                                swipedObjects.Add(target);
                                Debug.Log($"Swiped: {target.name}");
                                onEachSwiped?.Invoke();
                                break;
                            }
                        }

                        if (swipedObjects.Count == swipeTargets.Length)
                        {
                            Debug.Log("全てのスワイプ完了！");
                            onAllSwiped?.Invoke();
                        }
                    }
                }
            }
        }
    }

    SwipeDirection DetectDirection(Vector2 swipe)
    {
        return Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y)
            ? (swipe.x > 0 ? SwipeDirection.Right : SwipeDirection.Left)
            : (swipe.y > 0 ? SwipeDirection.Up : SwipeDirection.Down);
    }
}
