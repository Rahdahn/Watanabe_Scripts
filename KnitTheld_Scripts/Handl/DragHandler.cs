using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DragHandler : MonoBehaviour
{
    [Header("ドラッグ対象となるオブジェクト")]
    [SerializeField] private List<GameObject> draggableObjects;

    [Header("ドラッグターゲット")]
    [SerializeField] private Transform dragTargetArea;

    [Tooltip("ドラッグ成功時に許容される距離")]
    [SerializeField] private float snapDistance = 1f;

    [Header("イベント")]
    public UnityEvent onDragStart;
    public UnityEvent onDragEnd;
    public UnityEvent onDragSuccess;
    public UnityEvent onDragFail;

    private GameObject currentDragging;
    private bool dragging;

    void Update()
    {
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);

        if (PlayerInputReader.Instance.ClickStarted && !dragging)
        {
            foreach (GameObject obj in draggableObjects)
            {
                if (obj == null) continue;

                Collider2D col = obj.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(pointerWorldPos))
                {
                    currentDragging = obj;
                    dragging = true;
                    onDragStart?.Invoke();
                    break;
                }
            }
        }

        if (PlayerInputReader.Instance.ClickHeld && dragging && currentDragging != null)
        {
            currentDragging.transform.position = pointerWorldPos;
        }

        if (PlayerInputReader.Instance.ClickReleased && dragging && currentDragging != null)
        {
            dragging = false;

            float distance = Vector2.Distance(currentDragging.transform.position, dragTargetArea.position);
            if (distance <= snapDistance)
            {
                Debug.Log("ドラッグ成功！");
                onDragSuccess?.Invoke();
            }
            else
            {
                Debug.Log("ドラッグ失敗！");
                onDragFail?.Invoke();
            }

            currentDragging = null;
        }
    }
}
