using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

public class MixColorManager : MonoBehaviour
{
    public static MixColorManager Instance { get; private set; }

    [Header("ミルクのSpriteRenderer")]
    [SerializeField] private SpriteRenderer _milk;

    [Header("ミルクのコライダー")]
    [SerializeField] private Collider2D _milkCollider;

    [Header("単色スプライト（ジャム・オリーブオイル・はちみつの順番）")]
    [SerializeField] private Sprite[] _singleColorSprites;

    [Header("混色スプライト（1+2, 1+3, 2+3）")]
    [SerializeField] private Sprite[] _mixedSprites;

    [Header("デコトッピングスプライト（色は変わらないトッピング）")]
    [SerializeField] private Sprite[] _decorationSprites;

    [Header("ミルク上に表示する専用スプライト")]
    [SerializeField] private Sprite[] _milkTopSprites;

    [Header("ミルク上にトッピングを表示する為のGameObject")]
    [SerializeField] private GameObject[] _toppingObjects;

    [Header("IH電源ボタン（Collider付き）")]
    [SerializeField] private GameObject _ihPowerButton;

    [Header("IH電源が押されたときに呼ばれるイベント")]
    public UnityEvent onPowerOn;

    [Header("色フェード時間")]
    [SerializeField] private float _fadeDuration = 1.0f;

    private bool _ihPowerOn = false;
    private bool _isMixing = false;

    private List<int> _addedColors = new List<int>();
    private int _finalColorIndex = -1;
    private int _finalToppingIndex = -1;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        TargetManager.Instance.GenerateRandomTarget();
    }

    void Update()
    {
        if (PlayerInputReader.Instance.ClickStarted)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);
            Collider2D hit = Physics2D.OverlapPoint(pos);

            if (hit != null && hit.gameObject == _ihPowerButton)
                PowerOnIH();
        }
    }

    private void PowerOnIH()
    {
        if (_ihPowerOn) return;
        _ihPowerOn = true;
        Debug.Log("IH 電源ON");
        onPowerOn?.Invoke();
    }

    #region トッピング追加処理

    public bool TryAddTopping(int toppingIndex, bool isDecoration)
    {
        if (isDecoration)
        {
            return TryAddOnTopTopping(toppingIndex);
        }
        else
        {
            return TryAddColor(toppingIndex);
        }
    }

    private bool TryAddColor(int colorIndex)
    {
        if (_addedColors.Contains(colorIndex))
        {
            Debug.Log("同じ色は2回入れられません。");
            return false;
        }

        _addedColors.Add(colorIndex);
        _milk.sprite = _singleColorSprites[colorIndex];
        _finalColorIndex = colorIndex;
        Debug.Log($"トッピング追加: 色 {colorIndex}");
        return true;
    }

    private bool TryAddOnTopTopping(int toppingIndex)
    {
        if (_finalToppingIndex == toppingIndex)
        {
            Debug.Log("同じデコトッピングは2回追加できません。");
            return false;
        }

        _finalToppingIndex = toppingIndex;
        Debug.Log($"上に乗せるトッピング追加: {toppingIndex}");

        if (_toppingObjects != null && toppingIndex < _toppingObjects.Length)
        {
            SpriteRenderer sr = _toppingObjects[toppingIndex].GetComponent<SpriteRenderer>();
            if (sr != null && toppingIndex < _milkTopSprites.Length)
            {
                sr.sprite = _milkTopSprites[toppingIndex];
            }
        }

        return true;
    }

    public bool IsOverMilk(Vector2 worldPos)
    {
        return _milkCollider != null && _milkCollider.OverlapPoint(worldPos);
    }

    #endregion

    #region 混色処理

    public void MixColors()
    {
        if (!_ihPowerOn)
        {
            Debug.Log("IHをONにしてください。");
            return;
        }

        if (_addedColors.Count < 2)
        {
            Debug.Log("2色以上で混ぜてください。");
            return;
        }

        if (_isMixing) return;
        _isMixing = true;

        int first = _addedColors[_addedColors.Count - 2];
        int second = _addedColors[_addedColors.Count - 1];
        Sprite result = GetMixedSprite(first, second);
        int mixedIndex = GetMixedColorIndex(first, second);

        if (result == null)
        {
            _isMixing = false;
            return;
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(_milk.DOColor(Color.white, _fadeDuration * 0.3f))
           .AppendCallback(() => _milk.sprite = result)
           .Append(_milk.DOColor(Color.gray, _fadeDuration * 0.3f))
           .Append(_milk.DOColor(Color.white, _fadeDuration * 0.4f))
           .OnComplete(() =>
           {
               _finalColorIndex = mixedIndex;
               _isMixing = false;
               CheckGoal();
           });
    }

    private Sprite GetMixedSprite(int first, int second)
    {
        if ((first == 0 && second == 1) || (first == 1 && second == 0))
            return _mixedSprites.Length > 0 ? _mixedSprites[0] : null;
        else if ((first == 0 && second == 2) || (first == 2 && second == 0))
            return _mixedSprites.Length > 1 ? _mixedSprites[1] : null;
        else if ((first == 1 && second == 2) || (first == 2 && second == 1))
            return _mixedSprites.Length > 2 ? _mixedSprites[2] : null;
        else
            return _singleColorSprites[second];
    }

    private int GetMixedColorIndex(int first, int second)
    {
        if ((first == 0 && second == 1) || (first == 1 && second == 0)) return 3;
        if ((first == 0 && second == 2) || (first == 2 && second == 0)) return 4;
        if ((first == 1 && second == 2) || (first == 2 && second == 1)) return 5;
        return second;
    }

    #endregion

    private void CheckGoal()
    {
        bool success = TargetManager.Instance.CheckResult(_finalColorIndex, _finalToppingIndex);
        Debug.Log(success ? "完成品が目標と一致しました" : "完成品が目標と一致しません");
    }
}
