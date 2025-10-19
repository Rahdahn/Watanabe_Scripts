using UnityEngine;
using UnityEngine.UI;

public class ClickZoom : MonoBehaviour
{
    public Camera mainCamera;
    public float zoomSpeed = 2f;
    public float targetOrthographicSize = 2f;
    public Image backgroundOverlay;
    private float originalOrthographicSize;
    private Vector3 targetPosition;
    private bool isZooming = false;
    private GetGameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GetGameManager>();
        if (mainCamera != null)
        {
            originalOrthographicSize = mainCamera.orthographicSize;
        }
    }

    void Update()
    {
        if (isZooming)
        {
            ZoomToTarget();
        }
    }

    public void StartZoom(Vector3 position)
    {
        targetPosition = position;
        isZooming = true;

        // 画像を表示し、Order in Layerを90に設定
        backgroundOverlay.gameObject.SetActive(true);
        backgroundOverlay.canvas.sortingOrder = 90;
    }

    private void ZoomToTarget()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z), zoomSpeed * Time.deltaTime);
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, zoomSpeed * Time.deltaTime);

        if (Vector3.Distance(mainCamera.transform.position, new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z)) < 0.01f &&
            Mathf.Abs(mainCamera.orthographicSize - targetOrthographicSize) < 0.01f)
        {
            isZooming = false;
            gameManager.OnZoomComplete();
        }
    }

    public void ResetZoom()
    {
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = originalOrthographicSize;
            mainCamera.transform.position = new Vector3(0, 0, mainCamera.transform.position.z);

            // 画像を非表示にし、Order in Layerをリセット
            backgroundOverlay.gameObject.SetActive(false);
        }
    }
}
