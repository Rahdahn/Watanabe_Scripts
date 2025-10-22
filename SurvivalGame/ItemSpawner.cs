using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("�X�|�[��������A�C�e��Prefab")]
    [SerializeField] private GameObject itemPrefab;

    [Header("�X�|�[���|�C���g")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("�X�|�[���Ԋu(�b)")]
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

        // �����_���ȃX�|�[���|�C���g��I��
        int index = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[index];

        // �A�C�e������
        Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
    }
}
