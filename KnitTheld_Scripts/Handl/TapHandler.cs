using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class TapHandler : MonoBehaviour
{
    [Tooltip("�^�b�v�Ώۂ̃I�u�W�F�N�g")]
    [SerializeField] private GameObject[] tapTargets;

    [Tooltip("�e�I�u�W�F�N�g�̃^�b�v���ɌĂԃC�x���g")]
    public List<UnityEvent> onEachTapEvents = new List<UnityEvent>();

    [Tooltip("���ׂă^�b�v�����������Ƃ��̃C�x���g")]
    public UnityEvent onAllTapped;

    private HashSet<GameObject> tappedObjects = new HashSet<GameObject>();
    private bool isFinished = false;

    private void Start()
    {
        // �C�x���g���X�g�̐����^�b�v�Ώۂɍ��킹�Ē���
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
