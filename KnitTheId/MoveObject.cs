using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float duration = 1f;
    public UnityEvent onMoveComplete;

    [Tooltip("IMoveCompleteReceiver要実装")]
    public MonoBehaviour moveCompleteReceiver;

    public void MoveToTarget()
    {
        if (targetTransform == null) return;

        transform.DOMove(targetTransform.position, duration).OnComplete(() =>
        {
            onMoveComplete?.Invoke();

            if (moveCompleteReceiver is IMoveCompleteReceiver receiver)
            {
                receiver.NotifyMoved(this);
            }
        });
    }
}
