using UnityEngine;

public class ClearChecker : MonoBehaviour
{
    public static ClearChecker Instance { get; private set; }
    [SerializeField] private float cubeSize = 6f;

    private void Awake()
    {
        Instance = this;
    }

    private Vector3 center => new Vector3(0f, cubeSize / 2f, 0f);

    public void CheckClearCondition()
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("T");
        Bounds clearBounds = new Bounds(center, Vector3.one * cubeSize);

        foreach (GameObject piece in pieces)
        {
            if (!clearBounds.Contains(piece.transform.position))
            {
                Debug.Log("未クリア：ピースが範囲外です");
                return;
            }
        }

        Debug.Log("クリア");
    }
}
