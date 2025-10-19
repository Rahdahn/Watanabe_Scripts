using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DragHandler : MonoBehaviour
{
    [Header("�h���b�O�ΏۂƂȂ�I�u�W�F�N�g")]
    [SerializeField] private List<GameObject> draggableObjects;

    [Header("�h���b�O�^�[�Q�b�g")]
    [SerializeField] private Transform dragTargetArea;

    [Tooltip("�h���b�O�������ɋ��e����鋗��")]
    [SerializeField] private float snapDistance = 1f;

    [Header("�C�x���g")]
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
                Debug.Log("�h���b�O�����I");
                onDragSuccess?.Invoke();
            }
            else
            {
                Debug.Log("�h���b�O���s�I");
                onDragFail?.Invoke();
            }

            currentDragging = null;
        }
    }
}
