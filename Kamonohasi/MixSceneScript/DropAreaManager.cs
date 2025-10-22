using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class DropAreaManager : MonoBehaviour
{
    public static DropAreaManager Instance { get; private set; }

    public GameObject dropArea1;
    public GameObject dropArea2;
    public GameObject dropArea3;
    public GameObject dropArea4;
    public SliderMove1 sliderMove1;

    private Dictionary<GameObject, bool> dropAreaOccupied;
    private Dictionary<GameObject, string> dropAreaTags;

    public AnimalCounter animalCounter;  // AnimalCounterの参照を持つ

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (dropArea1 == null || dropArea2 == null || dropArea3 == null || dropArea4 == null)
        {
            return;
        }

        dropAreaOccupied = new Dictionary<GameObject, bool>
        {
            { dropArea1, false },
            { dropArea2, false },
            { dropArea3, false },
            { dropArea4, false }
        };

        dropAreaTags = new Dictionary<GameObject, string>
        {
            { dropArea1, null },
            { dropArea2, null },
            { dropArea3, null },
            { dropArea4, null }
        };

        InitializeDraggables();
    }

    private void InitializeDraggables()
    {
        var draggables = FindObjectsOfType<Draggable>();
        var dropAreaList = new List<RectTransform>
    {
        dropArea1.GetComponent<RectTransform>(),
        dropArea2.GetComponent<RectTransform>(),
        dropArea3.GetComponent<RectTransform>(),
        dropArea4.GetComponent<RectTransform>()
    };

        foreach (var draggable in draggables)
        {
            draggable.SetDropAreas(dropAreaList);

            draggable.snapPositions = new Dictionary<RectTransform, Vector2>
        {
            { dropArea1.GetComponent<RectTransform>(), GetSnapPosition(dropArea1) },
            { dropArea2.GetComponent<RectTransform>(), GetSnapPosition(dropArea2) },
            { dropArea3.GetComponent<RectTransform>(), GetSnapPosition(dropArea3) },
            { dropArea4.GetComponent<RectTransform>(), GetSnapPosition(dropArea4) }
        };

            draggable.onDropSuccess = (area, resetAction) =>
            {
                Debug.Log("ドラッグ成功時");

                if (dropAreaOccupied[area.gameObject])
                {
                    Debug.Log("このエリアはすでに使用されています。");
                    resetAction.Invoke();
                }
                else
                {
                    // ドロップエリアが使用済みであることを設定
                    dropAreaOccupied[area.gameObject] = true;

                    // ドラッグされたオブジェクトをドロップエリアにスナップ
                    draggable.transform.position = GetSnapPosition(area.gameObject);

                    // ドロップされたエリアのタグを保存
                    dropAreaTags[area.gameObject] = draggable.tag;

                    // ドロップされたエリアを非表示にする
                    area.gameObject.SetActive(false);

                    // 動物のカウントを減らす
                    ReduceAnimalCount(draggable.tag);

                    // すべてのドロップエリアが埋まったかチェック
                    CheckAllDropAreasFilled();
                }
            };

            draggable.onDropFail = (Action resetAction) =>
            {
                Debug.Log("ドラッグ失敗時");
                resetAction.Invoke();
            };
        }
    }


    Vector2 GetSnapPosition(GameObject dropArea)
    {
        return dropArea.GetComponent<RectTransform>().position;
    }

    void CheckAllDropAreasFilled()
    {
        foreach (var occupied in dropAreaOccupied.Values)
        {
            if (!occupied)
            {
                return;
            }
        }
        SaveDropOrder();
        StartSliderMove1();
    }

    void SaveDropOrder()
    {
        List<string> dropOrder = new List<string>
        {
            dropAreaTags[dropArea1],
            dropAreaTags[dropArea2],
            dropAreaTags[dropArea3],
            dropAreaTags[dropArea4]
        };

        string dropOrderString = string.Join(",", dropOrder);
        PlayerPrefs.SetString("DropOrder", dropOrderString);
        PlayerPrefs.SetFloat("DropOrderPercentage", CalculatePercentage(dropOrderString));
    }

    float CalculatePercentage(string dropOrder)
    {
        Dictionary<string, float> dropOrderPercentages = new Dictionary<string, float>
        {
            { "D1,D5,D4,D3", 41.1f },
            { "D1,D5,D3,D4", 92.2f },
            { "D1,D3,D4,D5", 90.0f },
            { "D1,D3,D5,D4", 75.4f },
            { "D1,D4,D5,D3", 33.3f },
            { "D1,D4,D3,D5", 8.2f },
            { "D5,D1,D4,D3", 92.1f },
            { "D5,D1,D3,D4", 47.1f },
            { "D5,D4,D1,D3", 8.6f },
            { "D5,D4,D3,D1", 70.6f },
            { "D5,D3,D1,D4", 70.0f },
            { "D5,D3,D4,D1", 34.2f },
            { "D4,D5,D3,D1", 81.6f },
            { "D4,D5,D1,D3", 58.8f },
            { "D4,D1,D3,D5", 5.1f },
            { "D4,D1,D5,D3", 33.8f },
            { "D4,D3,D1,D5", 79.7f },
            { "D4,D3,D5,D1", 53.4f },
            { "D3,D1,D4,D5", 99.9f },
            { "D3,D1,D5,D4", 95.2f },
            { "D3,D4,D1,D5", 55.7f },
            { "D3,D4,D5,D1", 2.3f },
            { "D3,D5,D4,D1", 91.6f },
            { "D3,D5,D1,D4", 80.3f },
            { "D4,D1,D2,D5", 18.1f},
            { "D2,D1,D5,D4", 27.1f},
            { "D1,D2,D5,D4", 8.0f},
            { "D1,D5,D2,D4", 67.4f},
            { "D5,D4,D1,D2", 64.0f},
            { "D5,D2,D4,D1", 91.2f}
        };

        if (dropOrderPercentages.TryGetValue(dropOrder, out float percentage))
        {
            return percentage;
        }

        return 0.0f;
    }

    void StartSliderMove1()
    {
        sliderMove1.gameObject.SetActive(true);
    }

    // 動物のカウントを減らす処理
    void ReduceAnimalCount(string tag)
    {
        switch (tag)
        {
            case "D1":
                if (GameData.D1Count > 0) GameData.D1Count--;
                break;
            case "D2":
                if (GameData.D2Count > 0) GameData.D2Count--;
                break;
            case "D3":
                if (GameData.D3Count > 0) GameData.D3Count--;
                break;
            case "D4":
                if (GameData.D4Count > 0) GameData.D4Count--;
                break;
            case "D5":
                if (GameData.D5Count > 0) GameData.D5Count--;
                break;
            default:
                Debug.LogWarning("Unknown animal tag: " + tag);
                break;
        }

        // カウントを減らした後にUIを更新
        animalCounter.UpdateAnimalCountDisplay();
    }
}
