using System.Collections.Generic;
using UnityEngine;

public class DraggableManager : MonoBehaviour
{
    // PrefabとHierarchy上のオブジェクトを定義
    public GameObject d1Prefab;
    public GameObject d2Prefab;
    public GameObject d3Prefab;
    public GameObject d4Prefab;
    public GameObject d5Prefab;

    public GameObject d1Object;
    public GameObject d2Object;
    public GameObject d3Object;
    public GameObject d4Object;
    public GameObject d5Object;

    // ドラッグ可能なオブジェクトのスナップ位置を管理するための辞書
    private Dictionary<Draggable, Dictionary<MonoBehaviour, Vector2>> draggableSnapPositions;

    // 初期化メソッド
    private void Start()
    {
        InitializeDraggables();
    }

    // ドラッグ可能なオブジェクトを初期化するメソッド
    private void InitializeDraggables()
    {
        draggableSnapPositions = new Dictionary<Draggable, Dictionary<MonoBehaviour, Vector2>>();

        // 必要な引数を正しく指定する
        CreateAndAttachAnimals(d1Prefab, d1Object, GameData.D1Count);
        CreateAndAttachAnimals(d2Prefab, d2Object, GameData.D2Count);
        CreateAndAttachAnimals(d3Prefab, d3Object, GameData.D3Count);
        CreateAndAttachAnimals(d4Prefab, d4Object, GameData.D4Count);
        CreateAndAttachAnimals(d5Prefab, d5Object, GameData.D5Count);
    }

    // Prefabから動物を生成し、指定のオブジェクトにアタッチするメソッド
    public void CreateAndAttachAnimals(GameObject prefab, GameObject referenceObject, int count)
    {
        if (prefab == null || referenceObject == null || count <= 0)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject newAnimal = Instantiate(prefab);
            newAnimal.transform.SetParent(referenceObject.transform.parent);
            newAnimal.transform.position = referenceObject.transform.position;
            newAnimal.transform.localScale = referenceObject.transform.localScale;
            newAnimal.transform.rotation = referenceObject.transform.rotation;

            Draggable draggableComponent = newAnimal.GetComponent<Draggable>();
            if (draggableComponent != null)
            {
                draggableSnapPositions[draggableComponent] = new Dictionary<MonoBehaviour, Vector2>();
            }
        }
    }
}
