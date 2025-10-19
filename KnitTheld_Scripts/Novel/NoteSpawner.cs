using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] notePrefabs;       // ��������m�[�cPrefab
    [SerializeField] private Transform spawnPoint;           // �o���ʒu
    [SerializeField] private float spawnInterval = 1.5f;     // �m�[�c�����Ԋu

    private float timer = 0f;

    void Update()
    {
        var manager = FindObjectOfType<NovelButtonManager>();
        if (manager != null && manager.IsStopped)
        {
            // ��~���̓^�C�}�[���Z�b�g���Đ����X�g�b�v
            timer = 0f;
            return;
        }

        // �^�C�}�[���Z
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
