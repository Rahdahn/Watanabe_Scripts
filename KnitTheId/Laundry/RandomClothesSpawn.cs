using System.Collections.Generic;
using UnityEngine;

public class RandomClothesSpawn : MonoBehaviour
{
    [Header("生成するPrefabたち")]
    [SerializeField] private GameObject[] prefabs;

    [Header("生成ポイント")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("生成数")]
    [SerializeField] private int spawnCount = 10;

    [Header("Order in Layer の範囲")]
    [SerializeField] private int orderMin = 0;
    [SerializeField] private int orderMax = 5;

    [SerializeField] private Transform parentContainer;

    [HideInInspector] public List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        SpawnRandomClothes();
    }

    public void SpawnRandomClothes()
    {
        spawnedObjects.Clear();

        if (prefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogError("Prefab または SpawnPoint が設定されていません。");
            return;
        }

        // 出現済みカラーを追跡
        HashSet<ColoredObject.ObjectColor> spawnedColors = new HashSet<ColoredObject.ObjectColor>();
        int orderRange = Mathf.Max(1, orderMax - orderMin + 1);
        int currentOrder = orderMin;

        // ランダム生成
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject obj = Instantiate(prefab, point.position, Quaternion.identity, parentContainer);
            spawnedObjects.Add(obj);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = currentOrder;
                currentOrder = (currentOrder + 1 - orderMin) % orderRange + orderMin;
            }

            var colored = obj.GetComponent<ColoredObject>();
            if (colored != null)
                spawnedColors.Add(colored.colorType);
        }

        // 全色保証
        ColoredObject.ObjectColor[] allColors =
        {
            ColoredObject.ObjectColor.White,
            ColoredObject.ObjectColor.Blue,
            ColoredObject.ObjectColor.Yellow,
            ColoredObject.ObjectColor.Black
        };

        foreach (var color in allColors)
        {
            if (!spawnedColors.Contains(color))
            {
                GameObject prefab = GetPrefabByColor(color);
                if (prefab == null) continue;

                Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject obj = Instantiate(prefab, point.position, Quaternion.identity, parentContainer);
                spawnedObjects.Add(obj);

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = currentOrder;
                    currentOrder = (currentOrder + 1 - orderMin) % orderRange + orderMin;
                }

                Debug.Log($"補完生成: {color}");
            }
        }
    }

    GameObject GetPrefabByColor(ColoredObject.ObjectColor color)
    {
        foreach (var prefab in prefabs)
        {
            var c = prefab.GetComponent<ColoredObject>();
            if (c != null && c.colorType == color)
                return prefab;
        }
        return null;
    }
}
