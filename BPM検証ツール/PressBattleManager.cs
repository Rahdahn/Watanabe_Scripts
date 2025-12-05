using DG.Tweening;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine;
using System.Threading;

public class PressBattleManager : MonoBehaviour
{
    [Header("UI関連")]
    [SerializeField] private Canvas _gameCanvas;                         // ゲーム開始・終了などの表示用Canvas
    [SerializeField] private TextMeshProUGUI _startText;

    [Header("オブジェクト")]
    [SerializeField] private Transform[] _ceilings;                      // 天井オブジェクト
    [SerializeField] private Transform[] _floors;                        // 床オブジェクト

    [Header("マップ設定")]
    [SerializeField] private string _mapDataFolder = "Assets/Scripts/PressBattle/MapData";
    [SerializeField] private int _totalRounds = 10;                      // 全ラウンド数

    [Header("ゲーム設定（BPM制御）")]
    [SerializeField] private float _defaultBPM = 100f;                   // Easy/Normalのテンポ
    [SerializeField] private float _hardBPM = 120f;                      // Hardのテンポ
    [SerializeField] private float _phase4Amount = 1.0f;                 // フェーズ4で天井を持ち上げる量
    [SerializeField] private float _basePressDistance = 5.0f;            // 天井を押し下げる距離
    [SerializeField] private float _upLiftAmount = 0.3f;                 // 押し下げ前に少し持ち上げる距離

    [Header("タイミング調整（秒）")]
    [Tooltip("落下開始音のズレ調整（マイナスで早める）")]
    [SerializeField] private float _pressSeOffset = 0.0f;
    [Tooltip("床衝突音のズレ調整（マイナスで早める）")]
    [SerializeField] private float _impactSeOffset = -0.05f;
    [Header("全体リズム調整")]
    [Tooltip("BGMの遅延補正（秒）。\nBGMが遅れて聞こえる場合はプラスの値（0.05〜0.1）を入れてください")]
    [SerializeField] private float _musicSyncOffset = 0.1f;

    private float _moveLength;

    [Header("難易度ごとの設定")]
    [Header("ラウンド数")]
    [SerializeField] private int _easyCount = 3;
    [SerializeField] private int _normalCount = 3;
    [SerializeField] private int _hardCount = 4;

    [Header("天井動作の拍設定")]
    [Tooltip("待機状態")]
    [SerializeField] private float _phase1WaitBeats = 4f;       // 待機状態
    [Tooltip("出題")]
    [SerializeField] private float _phase2JitterBeats = 4f;     // 出題
    [Tooltip("出題中")]
    [SerializeField] private float _phase3StaticBeats = 4f;     // 出題中
    [Tooltip("ギミック予兆")]
    [SerializeField] private float _phase4PreBeats = 2f;        // ギミック予兆
    [Tooltip("ギミック実行")]
    [SerializeField] private float _phase5ExecuteBeats = 2f;    // ギミック実行
    [Tooltip("プレス待機")]
    [SerializeField] private float _phase6PressedBeats = 1f;    // プレス待機
    [Tooltip("元の位置に戻る")]
    [SerializeField] private float _phase7ReturnBeats = 3f;     // 元の位置に戻る

    [Header("ガイド関連")]
    [SerializeField] private GameObject _guidePrefab;                    // ガイド用プレハブ
    [SerializeField] private int _guideRoundsCount = 2;                  // ガイドを出すラウンド数（最初のNラウンド）

    private Dictionary<Transform, Vector3> ceilingBasePositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Vector3> ceilingCurrentPositions = new Dictionary<Transform, Vector3>();
    private GameObject _currentGuideInstance = null;
    private string _lastLoadedMapFileName = "";
    private int _currentRoundIndex = 0;

    [SerializeField] private PressBattleAudioManager _audioManager;
    [SerializeField] private BeatCounter _beatCounter;

    // 11/18 00:40ごろ木村が追加
    [SerializeField]
    private PlayerPressedManager[] _pressResults;

    [SerializeField]
    private RoundCountManager _roundCountManager;

    private float TotalBeatsPerRound =>
        _phase1WaitBeats + _phase2JitterBeats + _phase3StaticBeats +
        _phase4PreBeats + _phase5ExecuteBeats + _phase6PressedBeats + _phase7ReturnBeats;   // 1ラウンドあたりの合計拍数を計算しておく

    private async void Start()
    {
        var token = this.GetCancellationTokenOnDestroy();
        await GameSequence(token);
    }

    #region BPM制御関連
    private float GetSecondsPerBeat(float bpm)
    {
        return 60f / bpm;
    }

    private float GetBPMForDifficulty(int difficulty)
    {
        if (difficulty == 3)
            return _hardBPM;
        else
            return _defaultBPM; // Easy・Normalは同じテンポ
    }
    #endregion

    #region ゲーム進行
    private async UniTask GameSequence(CancellationToken token)
    {
        // スタート表示
        _gameCanvas.enabled = true;
        _startText.text = "GAME START!";
        _startText.alpha = 0;
        _startText.DOFade(1, 0.5f);
        await UniTask.Delay(1500, cancellationToken: token);
        _startText.DOFade(0, 0.5f);
        await UniTask.Delay(500, cancellationToken: token);
        _gameCanvas.enabled = false;

        _beatCounter._targetBPM = GetBPMForDifficulty(GetDifficultyForRound(1));
        _beatCounter.StartCounting();
        _audioManager.PlayBGM();
        // 開始SE再生
        _audioManager.PlayStartSE();

        double gameStartTime = AudioSettings.dspTime;

        double accumulatedDuration = 0.0;

        for (int i = 0; i < _totalRounds; i++)
        {
            _currentRoundIndex = i + 1; // 1始まりにする
            _roundCountManager.RoundCountTextUpdate(_currentRoundIndex);
            int difficulty = GetDifficultyForRound(i);
            float bpm = GetBPMForDifficulty(difficulty);
            float spb = GetSecondsPerBeat(bpm);

            _beatCounter._targetBPM = bpm;

            double roundStartTime = gameStartTime + accumulatedDuration + _musicSyncOffset;
            // 天井の押下アニメーション
            await PressSequence(difficulty, spb, roundStartTime, token);


            accumulatedDuration += TotalBeatsPerRound * spb;

            // ラウンド終了処理
            EndRound();
        }

        // 終了表示
        _gameCanvas.enabled = true;
        _startText.text = "GAME CLEAR!";
        _startText.alpha = 0;
        _startText.DOFade(1, 1f);

        // 終了SE再生
        PressBattleAudioManager.Instance.PlayEndSE();

        // リザルト表示
        //PressBattleResult result = FindObjectOfType<PressBattleResult>();
        //if (result != null)
        //{
        //    result.ShowResult();
        //}

        /*PlayerPressedManager result = FindObjectOfType<PlayerPressedManager>();
        if (result != null)
        {
            result.EndPressBattle();
        }*/

        // 11/18 00:40ごろ木村修正
        // プレイヤーの数だけスコアをPlayerManagerDataに挿入
        foreach(var p in _pressResults)
        {
            p.EndPressBattle();
        }

        // 11/18 00:40ごろ木村修正
        // 時間経過でシーン移動(ミニゲームのリザルト)
        SceneTransitonManager.Instance.FadeSceneMove(SceneTransitonManager.MoveSeneName.MiniGameResult);

    }
    #endregion

    #region 天井押下アニメーション
    private async UniTask<double> PressSequence(int difficulty, float spb, double startDspTime, CancellationToken token)
    {
        double currentDspTime = startDspTime;

        // フェーズ1: 待機
        currentDspTime += (_phase1WaitBeats * spb);
        await WaitUntilDspTime(currentDspTime, token);

        // フェーズ2: 出題予兆＋マップデータ読み込み
        ObjectPositionList mapData = GetRandomMapData(difficulty);
        float phase2Duration = _phase2JitterBeats * spb;

        Phase2Animation(mapData, phase2Duration);

        if (_currentRoundIndex <= _guideRoundsCount)
        {
            LoadGuideForCurrentMap();
        }
        currentDspTime += phase2Duration;
        await WaitUntilDspTime(currentDspTime, token);

        // フェーズ3: 出題中
        currentDspTime += (_phase3StaticBeats * spb);
        await WaitUntilDspTime(currentDspTime, token);

        // フェーズ4: ギミック予兆
        float phase4Duration = _phase4PreBeats * spb;
        Phase4Animation(phase4Duration);

        currentDspTime += phase4Duration;
        await WaitUntilDspTime(currentDspTime, token);

        // フェーズ5: ギミック実行
        float phase5Duration = _phase5ExecuteBeats * spb;
        Phase5Animation(phase5Duration);

        double fallStartTime = currentDspTime + (phase5Duration * 0.5) + _pressSeOffset;
        double impactTime = currentDspTime + phase5Duration + _impactSeOffset;

        // 落下開始タイミングでSE再生
        ScheduleAction(fallStartTime, () => _audioManager.PlayPressSE(), token).Forget();
        // 着地タイミングでSE再生
        ScheduleAction(impactTime, () => _audioManager.PlayImpactSE(), token).Forget();

        currentDspTime += phase5Duration;
        await WaitUntilDspTime(currentDspTime, token);

        // フェーズ6: プレス待機
        currentDspTime += (_phase6PressedBeats * spb);
        await WaitUntilDspTime(currentDspTime, token);

        // フェーズ7: 元の位置に戻る
        float phase7Duration = _phase7ReturnBeats * spb;

        Phase7Animation(phase7Duration);
        currentDspTime += phase7Duration;
        await WaitUntilDspTime(currentDspTime, token);

        if (_currentRoundIndex <= _guideRoundsCount)
        {
            DeleteGuide();
        }

        return currentDspTime;
    }

    private async UniTask ScheduleAction(double targetTime, System.Action action, CancellationToken token)
    {
        await WaitUntilDspTime(targetTime, token);
        action?.Invoke();
    }

    private async UniTask WaitUntilDspTime(double targetDspTime, CancellationToken token)
    {
        while (AudioSettings.dspTime < targetDspTime)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
    #endregion

    #region アニメーション処理
    //マップ読み込み
    private void Phase2Animation(ObjectPositionList mapData, float phase2Duration)
    {
        foreach (Transform ceiling in _ceilings)
        {
            ObjectPositionData data = FindDataByName(mapData, ceiling.name);
            if (data != null)
            {
                ceilingBasePositions[ceiling] = data.position;
                ceiling.DOMove(data.position, phase2Duration).SetEase(Ease.InOutSine);
            }
        }

        foreach (Transform floor in _floors)
        {
            ObjectPositionData data = FindDataByName(mapData, floor.name);
            if (data != null)
            {
                floor.DOMove(data.position, phase2Duration).SetEase(Ease.OutQuad);
            }
        }
    }

    // 天井をちょっと持ち上げる
    private void Phase4Animation(float phase4Duration)
    {
        foreach (Transform ceiling in _ceilings)
        {
            if (ceilingBasePositions.ContainsKey(ceiling))
            {
                ceilingBasePositions[ceiling] = ceiling.position;
            }
            Vector3 targetUpPosition = ceilingBasePositions[ceiling] + Vector3.up * _phase4Amount;
            ceilingCurrentPositions[ceiling] = targetUpPosition;
            ceiling.DOMove(targetUpPosition, phase4Duration).SetEase(Ease.OutSine);
        }
    }

    // 天井押下
    private void Phase5Animation(float phase5Duration)
    {
        float upDuration = phase5Duration * 0.5f;
        float downDuration = phase5Duration * 0.5f;

        foreach (Transform ceiling in _ceilings)
        {
            if (ceilingCurrentPositions.ContainsKey(ceiling))
            {
                ceilingCurrentPositions[ceiling] = ceiling.position;
            }
            Vector3 startPos = ceilingCurrentPositions[ceiling];
            Vector3 upPos = startPos + Vector3.up * _upLiftAmount;
            Vector3 downPos = upPos + Vector3.down * (_basePressDistance + _upLiftAmount + _phase4Amount);

            ceilingCurrentPositions[ceiling] = downPos;

            Sequence cellingSeq = DOTween.Sequence();
            cellingSeq.Append(ceiling.DOMove(upPos, upDuration).SetEase(Ease.OutSine));
            cellingSeq.Append(ceiling.DOMove(downPos, downDuration).SetEase(Ease.InExpo));
        }
    }

    // 天井を元の位置に戻す
    private void Phase7Animation(float phase7Duration)
    {
        foreach (Transform ceiling in _ceilings)
        {
            if (ceilingBasePositions.TryGetValue(ceiling, out Vector3 targetPos))
            {
                ceiling.DOMove(targetPos, phase7Duration).SetEase(Ease.OutCubic);
            }
            else
            {
                // フォールバック
                ceiling.DOMove(ceiling.position, phase7Duration).SetEase(Ease.OutCubic);
            }
        }
    }
    #endregion

    #region ガイド関連
    private void LoadGuideForCurrentMap()
    {
        string guidePath = Path.Combine(_mapDataFolder, "Guid", _lastLoadedMapFileName + ".json");

        if (!File.Exists(guidePath))
        {
            return;
        }

        string json = File.ReadAllText(guidePath);
        ObjectPositionList guideData = JsonUtility.FromJson<ObjectPositionList>(json);

        _currentGuideInstance = new GameObject("GuideRoot_" + _lastLoadedMapFileName);
        foreach (var data in guideData.positions)
        {
            GameObject guideObj = Instantiate(_guidePrefab, data.position, Quaternion.identity, _currentGuideInstance.transform);
            guideObj.name = data.name;
        }

        Debug.Log($"ガイド生成");
    }

    private void DeleteGuide()
    {
        if (_currentGuideInstance != null)
        {
            Destroy(_currentGuideInstance);
            _currentGuideInstance = null;
            Debug.Log("ガイド削除");
        }
    }
    #endregion

    #region マップデータ関連
    private ObjectPositionData FindDataByName(ObjectPositionList mapData, string name)
    {
        foreach (var data in mapData.positions)
        {
            if (data.name == name)
                return data;
        }
        return null;
    }

    private ObjectPositionList GetRandomMapData(int difficulty)
    {
        string difficultyName = difficulty switch
        {
            1 => "Easy",
            2 => "Normal",
            3 => "Hard",
            _ => "Easy"
        };

        // MapDataFolder 以下のすべてのフォルダからマップデータを検索
        string[] files = Directory.GetFiles(
            _mapDataFolder,
            difficultyName + "_*.json",
            SearchOption.AllDirectories
        );

        if (files.Length == 0)
        {
            Debug.LogError($"難易度 {difficultyName} のマップデータが見つかりません {_mapDataFolder}");
            return null;
        }

        // 前回ロードしたマップを除外
        List<string> candidateFiles = new List<string>();
        foreach (var file in files)
        {
            if (Path.GetFileNameWithoutExtension(file) != _lastLoadedMapFileName)
                candidateFiles.Add(file);
        }

        string filePath = candidateFiles[Random.Range(0, candidateFiles.Count)];
        string json = File.ReadAllText(filePath);
        ObjectPositionList map = JsonUtility.FromJson<ObjectPositionList>(json);

        _lastLoadedMapFileName = Path.GetFileNameWithoutExtension(filePath);

        return map;
    }
    #endregion

    #region 難易度判定
    private int GetDifficultyForRound(int roundIndex)
    {
        if (roundIndex < _easyCount) return 1;
        else if (roundIndex < _easyCount + _normalCount) return 2;
        else return 3;
    }
    #endregion

    #region ラウンド終了処理
    private void EndRound()
    {
        PlayerPressedManager[] players = FindObjectsOfType<PlayerPressedManager>();
        foreach (var player in players)
        {
            player.PlayerAlive();
        }
    }
    #endregion
}
