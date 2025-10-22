using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DragScrollManager : MonoBehaviour
{
    [Header("�J�����ݒ�")]
    [SerializeField] private Camera mainCamera;
    [Tooltip("�J�������y�[�W�ړ����鑬�x")]
    [SerializeField] private float scrollSpeed = 5f;
    [Range(0.7f, 0.99f)]
    [SerializeField] private float edgeThreshold = 0.9f;

    [Header("�h���b�O�Ώېݒ�")]
    [Tooltip("�h���b�O�\�ȃI�u�W�F�N�g�𕡐��ݒ�")]
    [SerializeField] private List<Transform> draggableObjects = new List<Transform>();
    [Tooltip("�㉺�����̈ړ��𐧌�����")]
    [SerializeField] private bool restrictVerticalMovement = false;

    [Header("�h���b�v�G���A�ݒ�")]
    [Tooltip("�h���b�v��������p��Transform")]
    [SerializeField] private Transform dropZoneTransform;
    [Tooltip("�h���b�v����̋���")]
    [SerializeField] private float dropThreshold = 0.5f;

    [Header("�C�x���g")]
    public UnityEvent onDragAndMoveSuccess;

    // �������
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

        // �J��������
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
            // �h���b�v����
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

        // �ړ������
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

        // �E�[�Ńy�[�W����
        if (viewPos.x > edgeThreshold && currentPage == 0)
        {
            currentPage = 1;
            targetCamPos = baseCamPos + new Vector3(currentPage * cameraWidth, 0, 0);
            isCameraMoving = true;
        }
        // ���[�Ŗ߂�
        else if (viewPos.x < 1f - edgeThreshold && currentPage == 1)
        {
            currentPage = 0;
            targetCamPos = baseCamPos + new Vector3(currentPage * cameraWidth, 0, 0);
            isCameraMoving = true;
        }
    }
}
