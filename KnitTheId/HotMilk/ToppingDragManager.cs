using UnityEngine;
using System.Collections.Generic;

public class ToppingDragManager : MonoBehaviour
{
    [System.Serializable]
    public class ToppingData
    {
        [Header("トッピング本体（Collider2Dを持つ必要あり）")]
        public GameObject toppingObject;

        [Header("トッピングID（0～）")]
        public int toppingIndex;

        [Header("装飾トッピングか？（false = 色を変えるトッピング）")]
        public bool isDecoration = false;

        [Header("元の位置として使うTransform（未設定なら自身）")]
        public Transform originTransform;

        [HideInInspector] public Vector3 originalLocalPos;
        [HideInInspector] public bool isDragging;
    }

    [Header("管理する全トッピングリスト")]
    [SerializeField] private List<ToppingData> toppings = new List<ToppingData>();

    [Header("トッピングの親オブジェクト（回転対象）")]
    [SerializeField] private Transform toppingParent;

    [Header("親をドラッグで回転させるコライダー")]
    [SerializeField] private Collider2D rotationCollider;

    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float rotationIncrement = 60f;

    [Header("ドラッグ判定の基準Transform")]
    [SerializeField] private Transform dragReference;

    private Camera _mainCam;
    private bool _isRotating = false;
    private bool _isAnyDragging = false;
    private Vector2 _lastPointerPos;
    private float _targetAngle;

    void Start()
    {
        _mainCam = Camera.main;
        foreach (var t in toppings)
        {
            if (t.toppingObject == null) continue;
            t.originalLocalPos = t.originTransform != null ? t.originTransform.localPosition : t.toppingObject.transform.localPosition;
        }
    }

    void Update()
    {
        HandleInput();
        UpdateRotation();
    }

    private void HandleInput()
    {
        if (_mainCam == null) return;

        Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            // トッピングドラッグ開始対象を決める
            ToppingData closestTopping = null;
            float closestDist = float.MaxValue;

            foreach (var t in toppings)
            {
                if (t.toppingObject == null) continue;

                // 基準Transformとの距離で判定
                float dist = dragReference != null
                    ? Vector2.Distance(dragReference.position, t.toppingObject.transform.position)
                    : 0f;

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTopping = t;
                }
            }

            if (closestTopping != null && !_isRotating)
            {
                Collider2D col = closestTopping.toppingObject.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(mouseWorld))
                {
                    closestTopping.isDragging = true;
                    _isAnyDragging = true;
                    return; // ドラッグ開始
                }
            }

            // トッピングに当たらなかった場合のみ回転開始
            if (!_isAnyDragging && rotationCollider != null && rotationCollider.OverlapPoint(mouseWorld))
            {
                _isRotating = true;
                _lastPointerPos = mouseWorld;
            }
        }

        // ドラッグ中
        if (Input.GetMouseButton(0))
        {
            foreach (var t in toppings)
            {
                if (t.isDragging && t.toppingObject != null)
                {
                    t.toppingObject.transform.position = mouseWorld;
                }
            }
        }

        // ドラッグ終了
        if (Input.GetMouseButtonUp(0))
        {
            foreach (var t in toppings)
            {
                if (t.isDragging && t.toppingObject != null)
                {
                    Vector2 dropPos = t.toppingObject.transform.position;
                    bool overMilk = MixColorManager.Instance.IsOverMilk(dropPos);

                    if (overMilk)
                        MixColorManager.Instance.TryAddTopping(t.toppingIndex, t.isDecoration);

                    t.toppingObject.transform.localPosition = t.originalLocalPos;
                    t.isDragging = false;
                }
            }

            _isAnyDragging = false;
            _isRotating = false;
        }
    }

    private void UpdateRotation()
    {
        if (!_isRotating || toppingParent == null || _mainCam == null || _isAnyDragging) return;

        Vector2 pointerWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 parentPos = toppingParent.position;

        // 親中心からのベクトル
        Vector2 dir = pointerWorld - parentPos;

        // 角度計算
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // スナップ
        _targetAngle = Mathf.Round(angle / rotationIncrement) * rotationIncrement;

        // スムーズ回転
        float currentAngle = toppingParent.eulerAngles.z;
        float smoothAngle = Mathf.MoveTowardsAngle(currentAngle, _targetAngle, rotationSpeed * Time.deltaTime);
        toppingParent.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }
}
