using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] notePrefabs;       // 生成するノーツPrefab
    [SerializeField] private Transform spawnPoint;           // 出現位置
    [SerializeField] private float spawnInterval = 1.5f;     // ノーツ生成間隔

    private float timer = 0f;

    void Update()
    {
        var manager = FindObjectOfType<NovelButtonManager>();
        if (manager != null && manager.IsStopped)
        {
            // 停止中はタイマーリセットして生成ストップ
            timer = 0f;
            return;
        }

        // タイマー加算
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnNote();
            timer = 0f;
        }
    }

    public void SpawnNote()
    {
        if (notePrefabs.Length == 0) return;

        int index = Random.Range(0, notePrefabs.Length);
        Vector3 pos = spawnPoint.position;
        Instantiate(notePrefabs[index], pos, Quaternion.identity);
    }
}
