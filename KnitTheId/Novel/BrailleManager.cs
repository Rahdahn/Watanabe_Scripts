using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BrailleManager : MonoBehaviour
{
    [Header("日別テキストデータ")]
    [SerializeField] private BrailleTextData _textDataDay1;
    [SerializeField] private BrailleTextData _textDataDay2;
    [SerializeField] private BrailleTextData _textDataDay3;

    private BrailleTextData _textData;

    [Header("Prefab関連 (1-48)")]
    [SerializeField] private List<GameObject> _braillePrefabs = new List<GameObject>();
    private Dictionary<int, GameObject> _prefabDict = new Dictionary<int, GameObject>();

    [Header("出現位置リスト (1-7)")]
    [SerializeField] private List<Transform> _spawnPositions = new List<Transform>();

    [Header("入力設定")]
    [SerializeField] private float _inputTimeLimit = 0.5f;
    [SerializeField] private float _moveDuration = 0.5f;

    [Header("入力完了時イベント")]
    [SerializeField] private UnityEvent _onAllInputComplete;

    [SerializeField] private bool[] _dots = new bool[6];
    private int _currentIndex = 0;
    private float _inputTimer = 0f;
    private bool _isInputting = false;

    // 現在画面にあるPrefabのリスト（左から右＝1→7）
    private List<GameObject> _activePrefabs = new List<GameObject>();

    // Prefab番号と6点パターン
    private Dictionary<int, bool[]> _prefabDotMap;

    private bool[] _inputDotsBuffer = new bool[6];

    private void Awake()
    {
        InitializeDotMap();
    }

    private void Start()
    {
        switch (GameManager.Day)
        {
            case 1: _textData = _textDataDay1; break;
            case 2: _textData = _textDataDay2; break;
            case 3: _textData = _textDataDay3; break;
            default:
                _textData = _textDataDay1;
                break;
        }

        if (_textData == null)
        {
            Debug.LogError("BrailleTextData が設定されていません！");
            return;
        }

        // Prefab辞書を構築
        for (int i = 0; i < _braillePrefabs.Count; i++)
        {
            if (_braillePrefabs[i] != null)
                _prefabDict[i + 1] = _braillePrefabs[i];
        }

        // 初期4つ生成（4→3→2→1）
        StartCoroutine(SpawnInitialSequence());
    }

    private void InitializeDotMap()
    {
        _prefabDotMap = new Dictionary<int, bool[]>();

        // ひらがな
        _prefabDotMap[1] = new bool[] { true, false, false, false, false, false }; // あ
        _prefabDotMap[2] = new bool[] { true, true, false, false, false, false };  // い
        _prefabDotMap[3] = new bool[] { true, false, false, true, false, false };  // う
        _prefabDotMap[4] = new bool[] { true, false, false, true, true, false };   // え
        _prefabDotMap[5] = new bool[] { true, true, false, true, false, false };   // お

        _prefabDotMap[6] = new bool[] { true, false, false, false, false, true };  // か
        _prefabDotMap[7] = new bool[] { true, true, false, false, false, true };   // き
        _prefabDotMap[8] = new bool[] { true, false, false, true, false, true };   // く
        _prefabDotMap[9] = new bool[] { true, false, false, true, true, true };    // け
        _prefabDotMap[10] = new bool[] { true, true, false, true, false, true };   // こ

        _prefabDotMap[11] = new bool[] { true, false, false, false, true, true };   // さ
        _prefabDotMap[12] = new bool[] { true, true, false, false, true, true };   // し
        _prefabDotMap[13] = new bool[] { true, false, false, true, true, true };    // す
        _prefabDotMap[14] = new bool[] { true, false, false, true, true, true };    // せ
        _prefabDotMap[15] = new bool[] { true, true, false, true, true, true };     // そ

        _prefabDotMap[16] = new bool[] { true, false, true, false, true, false };   // た
        _prefabDotMap[17] = new bool[] { true, true, true, false, true, false };    // ち
        _prefabDotMap[18] = new bool[] { true, false, true, true, true, false };    // つ
        _prefabDotMap[19] = new bool[] { true, true, true, true, true, false };     // て
        _prefabDotMap[20] = new bool[] { false, true, true, true, true, false };    // と

        _prefabDotMap[21] = new bool[] { true, false, true, false, false, false }; // な
        _prefabDotMap[22] = new bool[] { true, true, true, false, false, false };  // に
        _prefabDotMap[23] = new bool[] { true, false, true, true, false, false };  // ぬ
        _prefabDotMap[24] = new bool[] { true, true, true, true, false, false };   // ね
        _prefabDotMap[25] = new bool[] { false, true, true, true, false, false };  // の

        _prefabDotMap[26] = new bool[] { true, false, true, false, false, true };  // は
        _prefabDotMap[27] = new bool[] { true, true, true, false, false, true };   // ひ
        _prefabDotMap[28] = new bool[] { true, false, true, true, false, true };   // ふ
        _prefabDotMap[29] = new bool[] { true, true, true, true, false, true };    // へ
        _prefabDotMap[30] = new bool[] { false, true, true, true, false, true };   // ほ

        _prefabDotMap[31] = new bool[] { true, false, true, false, true, true };  // ま
        _prefabDotMap[32] = new bool[] { true, true, true, false, true, true };   // み
        _prefabDotMap[33] = new bool[] { true, false, true, true, true, true };   // む
        _prefabDotMap[34] = new bool[] { true, true, true, true, true, true };   // め
        _prefabDotMap[35] = new bool[] { false, true, true, true, true, true };   // も

        _prefabDotMap[36] = new bool[] { false, false, true, true, false, false };  // や
        _prefabDotMap[37] = new bool[] { false, false, true, true, false, true };   // ゆ
        _prefabDotMap[38] = new bool[] { false, false, true, true, true, false };   // よ

        _prefabDotMap[39] = new bool[] { true, false, false, false, true, false }; // ら
        _prefabDotMap[40] = new bool[] { true, true, false, false, true, false };  // り
        _prefabDotMap[41] = new bool[] { true, false, false, true, true, false };  // る
        _prefabDotMap[42] = new bool[] { true, true, false, true, true, false };   // れ
        _prefabDotMap[43] = new bool[] { false, true, false, true, true, false };  // ろ

        _prefabDotMap[44] = new bool[] { false, false, true, false, false, false }; // わ
        _prefabDotMap[45] = new bool[] { false, false, true, false, true, false };  // を
        _prefabDotMap[46] = new bool[] { false, false, true, false, true, true };   // ん

        // 濁音・読点
        _prefabDotMap[47] = new bool[] { false, false, false, false, true, false }; // 濁点
        _prefabDotMap[48] = new bool[] { false, false, false, false, true, true }; // 読点
    }

    // タッチ入力用
    public void OnDotButtonPressed(int dotIndex)
    {
        if (dotIndex < 0 || dotIndex >= 6) return;

        _inputDotsBuffer[dotIndex] = true;

        if (!_isInputting)
        {
            _isInputting = true;
            _inputTimer = 0f;
        }
    }


    private IEnumerator SpawnInitialSequence()
    {
        int[] order = { 4, 3, 2, 1 };
        foreach (int posIndex in order)
        {
            if (_currentIndex >= _textData.prefabNumbers.Count) yield break;

            int prefabNumber = _textData.prefabNumbers[_currentIndex];
            if (_prefabDict.TryGetValue(prefabNumber, out GameObject prefab))
            {
                GameObject instance = Instantiate(prefab, _spawnPositions[posIndex - 1].position, Quaternion.identity);
                _activePrefabs.Insert(0, instance);
            }

            _currentIndex++;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private int _currentTargetNumber = -1;

    private void Update()
    {
        // 点字キー入力
        _dots[0] = Input.GetKey(KeyCode.F);
        _dots[1] = Input.GetKey(KeyCode.D);
        _dots[2] = Input.GetKey(KeyCode.S);
        _dots[3] = Input.GetKey(KeyCode.J);
        _dots[4] = Input.GetKey(KeyCode.K);
        _dots[5] = Input.GetKey(KeyCode.L);

        // 現在の中央Prefabを取得
        GameObject centerPrefab = GetCenterPrefab();
        if (centerPrefab != null)
        {
            _currentTargetNumber = ParsePrefabNumber(centerPrefab.name);
            Debug.Log($"入力待ちのPrefab番号: {_currentTargetNumber}");
        }

        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(GetKeyCode(i)))
            {
                _inputDotsBuffer[i] = true;
            }
        }

        if (!_isInputting && AnyKeyPressed())
        {
            _isInputting = true;
            _inputTimer = 0f;

            for (int i = 0; i < 6; i++)
                _inputDotsBuffer[i] = _dots[i];
        }

        if (_isInputting)
        {
            _inputTimer += Time.deltaTime;
            if (_inputTimer >= _inputTimeLimit)
            {
                ConfirmInput();
                _isInputting = false;
            }
        }
    }


    private KeyCode GetKeyCode(int index)
    {
        switch (index)
        {
            case 0: return KeyCode.F;
            case 1: return KeyCode.D;
            case 2: return KeyCode.S;
            case 3: return KeyCode.J;
            case 4: return KeyCode.K;
            case 5: return KeyCode.L;
            default: return KeyCode.None;
        }
    }

    private bool AnyKeyPressed()
    {
        for (int i = 0; i < 6; i++) if (_dots[i]) return true;
        return false;
    }

    private int ParsePrefabNumber(string prefabName)
    {
        // Clone がついていても対応
        prefabName = prefabName.Replace("(Clone)", "").Trim();

        string[] parts = prefabName.Split('_');
        if (parts.Length < 2) return -1;

        if (int.TryParse(parts[1], out int num))
            return num;

        return -1;
    }

    private void ConfirmInput()
    {
        GameObject centerPrefab = GetCenterPrefab();
        if (centerPrefab == null) return;

        // 中央Prefab番号を取得
        int centerNumber = ParsePrefabNumber(centerPrefab.name);

        // 判定
        bool isCorrect = CheckInput(_inputDotsBuffer, centerNumber);

        // ImageChange と赤色
        centerPrefab.GetComponent<ImageChange>()?.OnImageChange();
        if (isCorrect == false)
        {
            SetObjectColor(centerPrefab, Color.red);
            Debug.Log("入力が間違っています");
        }
        // 左端に次の文字を生成して右にスライド
        if (_currentIndex < _textData.prefabNumbers.Count)
        {
            int nextPrefabNumber = _textData.prefabNumbers[_currentIndex];
            SpawnNewLetter(nextPrefabNumber);
            _currentIndex++;
        }

        // 入力完了イベント
        if (_currentIndex >= _textData.prefabNumbers.Count)
            _onAllInputComplete?.Invoke();

        for (int i = 0; i < 6; i++) _inputDotsBuffer[i] = false;

        ResetDots();
    }

    private void ResetDots()
    {
        for (int i = 0; i < 6; i++) _dots[i] = false;
    }

    private bool CheckInput(bool[] dots, int prefabNumber)
    {
        bool[] correctDots = _prefabDotMap[prefabNumber];

        for (int i = 0; i < 6; i++)
            if (dots[i] != correctDots[i])
                return false;

        return true;
    }


    private GameObject GetCenterPrefab()
    {
        if (_activePrefabs.Count == 0) return null;

        Transform centerPos = _spawnPositions[3];
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var obj in _activePrefabs)
        {
            float dist = Vector3.Distance(obj.transform.position, centerPos.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }

        return closest;
    }

    private void SpawnNewLetter(int prefabNumber)
    {
        if (!_prefabDict.TryGetValue(prefabNumber, out GameObject prefab)) return;

        // 右にスライド
        StartCoroutine(ShiftPrefabsRight());
        GameObject instance = Instantiate(prefab, _spawnPositions[0].position, Quaternion.identity);
        _activePrefabs.Insert(0, instance);
    }


    private IEnumerator ShiftPrefabsRight()
    {
        List<GameObject> toRemove = new List<GameObject>();
        for (int i = 0; i < _activePrefabs.Count; i++)
        {
            Transform t = _activePrefabs[i].transform;
            int nextIndex = i + 1;

            if (nextIndex >= _spawnPositions.Count)
            {
                toRemove.Add(_activePrefabs[i]);
                continue;
            }

            Vector3 start = t.position;
            Vector3 end = _spawnPositions[nextIndex].position;

            StartCoroutine(MoveOverTime(t, start, end, _moveDuration));
        }

        yield return new WaitForSeconds(_moveDuration);
        foreach (var obj in toRemove)
        {
            _activePrefabs.Remove(obj);
            Destroy(obj);
        }
    }

    private IEnumerator MoveOverTime(Transform obj, Vector3 start, Vector3 end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            obj.position = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
    }

    private void SetObjectColor(GameObject obj, Color color)
    {
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;

        var img = obj.GetComponent<Image>();
        if (img != null) img.color = color;
    }
}
