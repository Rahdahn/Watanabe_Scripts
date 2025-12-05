using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public string stepName;
        public UnityEvent onStepStart;    // ステップ開始時のイベント
        public UnityEvent onStepComplete; // ステップ完了後に実行されるイベント
    }

    public List<Step> steps = new List<Step>();
    private int currentStepIndex = -1;

    [Header("次のシーン設定")]
    [SceneName] public string nextScene;        // 通常のシーン遷移
    [SceneName] public string finalScene;       // Day 4になったときに移動するシーン

    [SerializeField] private float sceneChangeDelay = 1.5f;

    [Header("日付管理")]
    [SerializeField] private bool _endDay = false;

    public static int Day { get; private set; } = 1;

    void Start()
    {
        AdvanceStep(); // 最初のステップから開始
    }

    public void AdvanceStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("全ステップ完了！");

            // 日付を進める処理
            if (_endDay)
            {
                Day++;
                Debug.Log($"Dayが進みました。現在のDay: {Day}");
            }

            // Day 4になったら finalScene に移動
            if (Day >= 4 && !string.IsNullOrEmpty(finalScene))
            {
                StartCoroutine(DelayedSceneChange(finalScene));
            }
            else if (!string.IsNullOrEmpty(nextScene))
            {
                StartCoroutine(DelayedSceneChange(nextScene));
            }

            return;
        }

        Step current = steps[currentStepIndex];
        Debug.Log($"ステップ開始: {current.stepName}");

        current.onStepStart?.Invoke();
    }

    public void CompleteCurrentStep()
    {
        Step current = steps[currentStepIndex];
        Debug.Log($"ステップ完了: {current.stepName}");

        current.onStepComplete?.Invoke();

        AdvanceStep(); // 次のステップへ
    }

    private IEnumerator DelayedSceneChange(string sceneName)
    {
        yield return new WaitForSeconds(sceneChangeDelay);

        SceneFader.Instance.FadeToScene(sceneName);
    }
}
