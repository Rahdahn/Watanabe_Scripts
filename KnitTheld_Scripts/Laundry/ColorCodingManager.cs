using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColorCodingManager : MonoBehaviour
{
    [Header("ドロップターゲットエリア（色別）")]
    [SerializeField] private Transform whiteTarget;
    [SerializeField] private Transform blueTarget;
    [SerializeField] private Transform yellowTarget;
    [SerializeField] private Transform blackTarget;

    [Header("スナップ距離")]
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
        // RandomClothesSpawnから生成物を取得
        RandomClothesSpawn spawner = FindObjectOfType<RandomClothesSpawn>();
        if (spawner != null)
        {
            draggableObjects = spawner.spawnedObjects;
        }
        else
        {
            Debug.LogError("RandomClothesSpawn がシーンに見つかりません。");
        }
    }

    void Update()
    {
        Vector2 pointerWorldPos = Camera.main.ScreenToWorldPoint(PlayerInputReader.Instance.PointerPosition);

        // ドラッグ開始
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

        // ドラッグ中
        if (PlayerInputReader.Instance.ClickHeld && dragging && currentDragging != null)
        {
            currentDragging.transform.position = pointerWorldPos;
        }

        // ドラッグ終了
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
                    // 成功 ターゲットエリアの子に設定
                    currentDragging.transform.SetParent(targetArea);
                    currentDragging.transform.localPosition = Vector3.zero;

                    correctlyPlaced.Add(currentDragging);
                    onDragSuccess?.Invoke();
                    Debug.Log($"ドラッグ成功！({colored.colorType})");

                    // 全成功チェック
                    if (correctlyPlaced.Count >= draggableObjects.Count)
                    {
                        onAllDragSuccess?.Invoke();
                    }
                }
                else
                {
                    // 失敗
                    onDragFail?.Invoke();
                    Debug.Log($"ドラッグ失敗（距離超過 or 色違い）: {colored.colorType}");
                }
            }

            currentDragging = null;
        }
    }

    // 指定座標にあるオブジェクトの中で、sortingOrderが最も高いものを返す
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

    // 色ごとのターゲットエリア取得
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
