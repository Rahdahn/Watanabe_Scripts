using UnityEngine;

public class PieceDragger : MonoBehaviour
{
    [SerializeField] private LayerMask draggableLayer; // Inspector�� DraggableCollider ���w��
    [SerializeField] private float rotationSpeed = 90f; // 1�b�Ԃɉ��x��]���邩

    private Camera mainCamera;
    private GameObject selectedPiece;
    private Vector3 offset;
    private float zDistance;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleSelection();
        HandleRotation();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, draggableLayer))
            {
                selectedPiece = hit.collider.transform.root.gameObject;

                zDistance = Vector3.Distance(mainCamera.transform.position, selectedPiece.transform.position);
                Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance));
                offset = selectedPiece.transform.position - worldPoint;
            }
        }

        if (Input.GetMouseButton(0) && selectedPiece != null)
        {
            Vector3 mouseWorldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistance));
            Vector3 targetPosition = mouseWorldPoint + offset;

            // WASD�ŉ�]�i���̂܂܁j
            float moveSpeed = 2f;

            // Q / E �Ńs�[�X���J�����O���։����o���iZ�������j
            Vector3 forward = mainCamera.transform.forward;
            forward.y = 0f; // �����O�i�����ɐ���
            forward.Normalize();

            if (Input.GetKey(KeyCode.Q))
                targetPosition += -forward * moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.E))
                targetPosition += forward * moveSpeed * Time.deltaTime;

            // Y���W�����i�n�ʂɖ��܂�Ȃ��j
            targetPosition.y = Mathf.Max(0f, targetPosition.y);

            selectedPiece.transform.position = targetPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectedPiece != null)
            {
                // �h���b�O�I�����ɃN���A����
                ClearChecker.Instance.CheckClearCondition();
            }
            selectedPiece = null;
        }
    }

    void HandleRotation()
    {
        if (selectedPiece == null || !Input.GetMouseButton(0)) return;

        float rotateY = 0f;
        float rotateX = 0f;

        if (Input.GetKey(KeyCode.A)) rotateY = -1f;
        if (Input.GetKey(KeyCode.D)) rotateY = 1f;
        if (Input.GetKey(KeyCode.W)) rotateX = -1f;
        if (Input.GetKey(KeyCode.S)) rotateX = 1f;

        Vector3 rotation = new Vector3(rotateX, rotateY, 0f) * rotationSpeed * Time.deltaTime;
        selectedPiece.transform.Rotate(rotation, Space.World);
    }
}
