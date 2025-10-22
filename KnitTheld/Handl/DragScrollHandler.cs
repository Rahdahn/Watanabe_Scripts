using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DragScrollManager : MonoBehaviour
{
    [Header("カメラ設定")]
    [SerializeField] private Camera mainCamera;
    [Tooltip("カメラがページ移動する速度")]
    [SerializeField] private float scrollSpeed = 5f;
    [Range(0.7f, 0.99f)]
    [SerializeField] private float edgeThreshold = 0.9f;

    [Header("ドラッグ対象設定")]
    [Tooltip("ドラッグ可能なオブジェクトを複数設定")]
    [SerializeField] private List<Transform> draggableObjects = new List<Transform>();
    [Tooltip("上下方向の移動を制限する")]
    [SerializeField] private bool restrictVerticalMovement = false;

    [Header("ドロップエリア設定")]
    [Tooltip("ドロップ成功判定用のTransform")]
    [SerializeField] private Transform dropZoneTransform;
    [Tooltip("ドロップ判定の距離")]
    [SerializeField] private float dropThreshold = 0.5f;

    [Header("イベント")]
    public UnityEvent onDragAndMoveSuccess;

    // 内部状態
    private Transform currentDraggingObject;
    private Vector3 dragOffset;
    private bool isDragging = false;

    private int currentPage = 0;
    private bool isCameraMoving = false;
    private Vector3 baseCamPos;
    private Vector3 targetCamPos;
    private float cameraWidth;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        baseCamPos = mainCamera.transform.position;

        // カメラ横幅
        cameraWidth = mainCamera.orthographicSize * 2f * mainCamera.aspect;

        targetCamPos = baseCamPos;

    }

    void Update()
    {
        HandleDrag();
        HandleCameraScroll();
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            foreach (var obj in draggableObjects)
            {
                if (obj == null) continue;
                Collider2D col = obj.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(mouseWorld))
                {
                    currentDraggingObject = obj;
                    isDragging = true;
                    dragOffset = obj.position - new Vector3(mouseWorld.x, mouseWorld.y, obj.position.z);
                    break;
                }
            }
        }

        if (isDragging && currentDraggingObject != null && Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = new Vector3(
                mouseWorld.x + dragOffset.x,
                restrictVerticalMovement ? currentDraggingObject.position.y : mouseWorld.y + dragOffset.y,
                currentDraggingObject.position.z
            );
            currentDraggingObject.position = newPos;
        }

        if (Input.GetMouseButtonUp(0) && currentDraggingObject != null)
        {
            // ドロップ判定
            if (dropZoneTransform != null)
            {
                float distance = Vector3.Distance(currentDraggingObject.position, dropZoneTransform.position);
                if (distance <= dropThreshold)
                {
                    onDragAndMoveSuccess?.Invoke();
                }
            }

            isDragging = false;
            currentDraggingObject = null;
        }
    }

    void HandleCameraScroll()
    {
        if (currentDraggingObject == null || !isDragging) return;

        // 移動中補間
        if (isCameraMoving)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCamPos, Time.deltaTime * scrollSpeed);
            if (Vector3.Distance(mainCamera.transform.position, targetCamPos) < 0.01f)
            {
                mainCamera.transform.position = targetCamPos;
                isCameraMoving = false;
            }
            return;
        }

        Vector3 viewPos = mainCamera.WorldToViewportPoint(currentDraggingObject.position);

        // 右端でページ送り
        if (viewPos.x > edgeThreshold && currentPage == 0)
        {
            currentPage = 1;
            targetCamPos = baseCamPos + new Vector3(currentPage * cameraWidth, 0, 0);
            isCameraMoving = true;
        }
        // 左端で戻る
        else if (viewPos.x < 1f - edgeThreshold && currentPage == 1)
        {
            currentPage = 0;
            targetCamPos = baseCamPos + new Vector3(currentPage * cameraWidth, 0, 0);
            isCameraMoving = true;
        }
    }
}
