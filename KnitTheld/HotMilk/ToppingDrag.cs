using UnityEngine;

public class ToppingDrag : MonoBehaviour
{
    [Header("トッピングID（0-）")]
    public int toppingIndex;

    [Header("装飾トッピングか？（false = 色を変えるトッピング）")]
    public bool isDecoration = false;

    private Vector3 _startPos;
    private bool _isDragging;

    void Start()
    {
        _startPos = transform.position;
    }

    void OnMouseDown()
    {
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;
        _isDragging = false;

        Vector2 dropPos = transform.position;
        bool overMilk = MixColorManager.Instance.IsOverMilk(dropPos);

        if (overMilk)
        {
            bool added = MixColorManager.Instance.TryAddTopping(toppingIndex, isDecoration);
            Debug.Log(added ? $"トッピング {toppingIndex} 追加成功" : $"トッピング {toppingIndex} 失敗");
        }

        // 成功・失敗に関わらず元の位置に戻す
        transform.position = _startPos;
    }
}
