using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    [Header("目標ミルクを表示するスプライト")]
    [SerializeField] private SpriteRenderer _targetMilkRenderer;

    [Header("目標トッピングを表示するスプライト")]
    [SerializeField] private SpriteRenderer _targetToppingRenderer;

    [Header("ミルクの色候補 (MixColorManagerと同じ順)")]
    [SerializeField] private Sprite[] _milkColorSprites;

    [Header("トッピングの候補 (MixColorManagerの_onToppingsと同じ順)")]
    [SerializeField] private Sprite[] _toppingSprites;

    private int _targetColorIndex;       // 目標ミルクの色
    private int _targetToppingIndex;     // 目標トッピング

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GenerateRandomTarget();
    }

    /// <summary>
    /// ランダムに目標のミルクとトッピングを生成
    /// </summary>
    public void GenerateRandomTarget()
    {
        _targetColorIndex = Random.Range(0, _milkColorSprites.Length);
        _targetToppingIndex = Random.Range(0, _toppingSprites.Length);

        // 表示を更新
        UpdateTargetDisplay();
    }

    private void UpdateTargetDisplay()
    {
        // 目標ミルクのスプライトを表示
        _targetMilkRenderer.sprite = _milkColorSprites[_targetColorIndex];

        // 目標トッピングを上に重ねて表示
        if (_targetToppingRenderer != null && _targetToppingIndex < _toppingSprites.Length)
        {
            _targetToppingRenderer.sprite = _toppingSprites[_targetToppingIndex];
        }

        Debug.Log($"目標：色={_targetColorIndex}, トッピング={_targetToppingIndex}");
    }

    public bool CheckResult(int colorIndex, int toppingIndex)
    {
        return _targetColorIndex == colorIndex && _targetToppingIndex == toppingIndex;
    }

    public int TargetColorIndex => _targetColorIndex;
    public int TargetToppingIndex => _targetToppingIndex;
}
