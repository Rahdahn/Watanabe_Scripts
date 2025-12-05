using UnityEngine;
using UnityEngine.Events;

public class DelayEvent : MonoBehaviour
{
    [Header("待機時間（秒）")]
    [SerializeField] private float delayTime = 2f;

    [Header("時間経過後に呼ぶイベント")]
    public UnityEvent onDelayedEvent;

    void Start()
    {
        StartCoroutine(WaitAndInvoke());
    }

    private System.Collections.IEnumerator WaitAndInvoke()
    {
        yield return new WaitForSeconds(delayTime);
        onDelayedEvent?.Invoke();
    }
}
