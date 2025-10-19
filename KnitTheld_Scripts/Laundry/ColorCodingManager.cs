using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColorCodingManager : MonoBehaviour
{
    [Header("�h���b�v�^�[�Q�b�g�G���A�i�F�ʁj")]
    [SerializeField] private Transform whiteTarget;
    [SerializeField] private Transform blueTarget;
    [SerializeField] private Transform yellowTarget;
    [SerializeField] private Transform blackTarget;

    [Header("�X�i�b�v����")]
    [SerializeField] private float snapDistance = 1f;

    [Header("UnityEvent")]
    public UnityEvent onDragStart;
    public UnityEvent onDragSuccess;
    public UnityEvent onDragFail;
    public UnityEvent onAllDragSuccess;

    private List<GameObject> draggableObjects = new List<GameObject>();
    private HashSet<GameObject> correctlyPlaced = new HashSet<GameObject>();

    private GameObject currentDragging;
    private bool dragging = false;

    void Start()
    {
        // RandomClothesSpawn���琶�������擾
        RandomClothesSpawn spawner = FindObjectOfType<RandomClothesSpawn>();
        if (spawner != null)
        {
            draggableObjects = spawner.spawnedObjects;
        }
        else
        {
            Debug.LogError("RandomClothesSpawn ���V�[���Ɍ�����܂���B");
        }
    }

    void Update()
    {
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);

        // �h���b�O�J�n
        if (PlayerInputReader.Instance.ClickStarted && !dragging)
        {
            GameObject topObject = GetTopmostObjectAtPoint(pointerWorldPos);
            if (topObject != null)
            {
                currentDragging = topObject;
                dragging = true;
                onDragStart?.Invoke();
            }
        }

        // �h���b�O��
        if (PlayerInputReader.Instance.ClickHeld && dragging && currentDragging != null)
        {
            currentDragging.transform.position = pointerWorldPos;
        }

        // �h���b�O�I��
        if (PlayerInputReader.Instance.ClickReleased && dragging && currentDragging != null)
        {
            dragging = false;

            ColoredObject colored = currentDragging.GetComponent<ColoredObject>();
            if (colored != null)
            {
                Transform targetArea = GetTargetArea(colored.colorType);
                float distance = Vector2.Distance(currentDragging.transform.position, targetArea.position);

                if (distance <= snapDistance)
                {
                    // ���� �^�[�Q�b�g�G���A�̎q�ɐݒ�
                    currentDragging.transform.SetParent(targetArea);
                    currentDragging.transform.localPosition = Vector3.zero;

                    correctlyPlaced.Add(currentDragging);
                    onDragSuccess?.Invoke();
                    Debug.Log($"�h���b�O�����I({colored.colorType})");

                    // �S�����`�F�b�N
                    if (correctlyPlaced.Count >= draggableObjects.Count)
                    {
                        onAllDragSuccess?.Invoke();
                    }
                }
                else
                {
                    // ���s
                    onDragFail?.Invoke();
                    Debug.Log($"�h���b�O���s�i�������� or �F�Ⴂ�j: {colored.colorType}");
                }
            }

            currentDragging = null;
        }
    }

    // �w����W�ɂ���I�u�W�F�N�g�̒��ŁAsortingOrder���ł��������̂�Ԃ�
    GameObject GetTopmostObjectAtPoint(Vector2 worldPoint)
    {
        GameObject topObject = null;
        int highestOrder = int.MinValue;

        foreach (GameObject obj in draggableObjects)
        {
            if (obj == null) continue;

            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(worldPoint))
            {
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                int order = sr != null ? sr.sortingOrder : 0;

                if (order > highestOrder)
                {
                    highestOrder = order;
                    topObject = obj;
                }
            }
        }

        return topObject;
    }

    // �F���Ƃ̃^�[�Q�b�g�G���A�擾
    Transform GetTargetArea(ColoredObject.ObjectColor color)
    {
        switch (color)
        {
            case ColoredObject.ObjectColor.White: return whiteTarget;
            case ColoredObject.ObjectColor.Blue: return blueTarget;
            case ColoredObject.ObjectColor.Yellow: return yellowTarget;
            case ColoredObject.ObjectColor.Black: return blackTarget;
            default: return null;
        }
    }
}
