using UnityEngine;

public class PieceDragger : MonoBehaviour
{
    [SerializeField] private LayerMask draggableLayer; // Inspectorで DraggableCollider を指定
    [SerializeField] private float rotationSpeed = 90f; // 1秒間に何度回転するか

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

            // WASDで回転（そのまま）
            float moveSpeed = 2f;

            // Q / E でピースをカメラ前方へ押し出す（Z軸方向）
            Vector3 forward = mainCamera.transform.forward;
            forward.y = 0f; // 水平前進だけに制限
            forward.Normalize();

            if (Input.GetKey(KeyCode.Q))
                targetPosition += -forward * moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.E))
                targetPosition += forward * moveSpeed * Time.deltaTime;

            // Y座標制限（地面に埋まらない）
            targetPosition.y = Mathf.Max(0f, targetPosition.y);

            selectedPiece.transform.position = targetPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectedPiece != null)
            {
                // ドラッグ終了時にクリア判定
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
