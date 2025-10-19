using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class TapHandler : MonoBehaviour
{
    [Tooltip("タップ対象のオブジェクト")]
    [SerializeField] private GameObject[] tapTargets;

    [Tooltip("各オブジェクトのタップ時に呼ぶイベント")]
    public List<UnityEvent> onEachTapEvents = new List<UnityEvent>();

    [Tooltip("すべてタップが完了したときのイベント")]
    public UnityEvent onAllTapped;

    private HashSet<GameObject> tappedObjects = new HashSet<GameObject>();
    private bool isFinished = false;

    private void Start()
    {
        // イベントリストの数をタップ対象に合わせて調整
        while (onEachTapEvents.Count < tapTargets.Length)
        {
            onEachTapEvents.Add(new UnityEvent());
        }
    }

    void Update()
    {
        if (isFinished || tapTargets.Length == 0) return;

        if (PlayerInputReader.Instance.ClickStarted)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);
            Collider2D hit = Physics2D.OverlapPoint(pos);

            if (hit != null)
            {
                for (int i = 0; i < tapTargets.Length; i++)
                {
                    GameObject target = tapTargets[i];
                    if (hit.gameObject == target && !tappedObjects.Contains(target))
                    {
                        tappedObjects.Add(target);
                        if (i < onEachTapEvents.Count)
                            onEachTapEvents[i]?.Invoke();

                        break;
                    }
                }

                if (tappedObjects.Count == tapTargets.Length)
                {
                    isFinished = true;
                    onAllTapped?.Invoke();
                }
            }
        }
    }
}
