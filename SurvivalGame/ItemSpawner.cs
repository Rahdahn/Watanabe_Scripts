using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("スポーンさせるアイテムPrefab")]
    [SerializeField] private GameObject itemPrefab;

    [Header("スポーンポイント")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("スポーン間隔(秒)")]
    [SerializeField] private float spawnInterval = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnItem();
            timer = 0f;
        }
    }

    void SpawnItem()
    {
        if (spawnPoints.Count == 0 || itemPrefab == null) return;

        // ランダムなスポーンポイントを選ぶ
        int index = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[index];

        // アイテム生成
        Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
    }
}
