using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    [Header("�ڕW�~���N��\������X�v���C�g")]
    [SerializeField] private SpriteRenderer _targetMilkRenderer;

    [Header("�ڕW�g�b�s���O��\������X�v���C�g")]
    [SerializeField] private SpriteRenderer _targetToppingRenderer;

    [Header("�~���N�̐F��� (MixColorManager�Ɠ�����)")]
    [SerializeField] private Sprite[] _milkColorSprites;

    [Header("�g�b�s���O�̌�� (MixColorManager��_onToppings�Ɠ�����)")]
    [SerializeField] private Sprite[] _toppingSprites;

    private int _targetColorIndex;       // �ڕW�~���N�̐F
    private int _targetToppingIndex;     // �ڕW�g�b�s���O

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
    /// �����_���ɖڕW�̃~���N�ƃg�b�s���O�𐶐�
    /// </summary>
    public void GenerateRandomTarget()
    {
        _targetColorIndex = Random.Range(0, _milkColorSprites.Length);
        _targetToppingIndex = Random.Range(0, _toppingSprites.Length);

        // �\�����X�V
        UpdateTargetDisplay();
    }

    private void UpdateTargetDisplay()
    {
        // �ڕW�~���N�̃X�v���C�g��\��
        _targetMilkRenderer.sprite = _milkColorSprites[_targetColorIndex];

        // �ڕW�g�b�s���O����ɏd�˂ĕ\��
        if (_targetToppingRenderer != null && _targetToppingIndex < _toppingSprites.Length)
        {
            _targetToppingRenderer.sprite = _toppingSprites[_targetToppingIndex];
        }

        Debug.Log($"�ڕW�F�F={_targetColorIndex}, �g�b�s���O={_targetToppingIndex}");
    }

    public bool CheckResult(int colorIndex, int toppingIndex)
    {
        return _targetColorIndex == colorIndex && _targetToppingIndex == toppingIndex;
    }

    public int TargetColorIndex => _targetColorIndex;
    public int TargetToppingIndex => _targetToppingIndex;
}
