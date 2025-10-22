using UnityEngine;

public class Note : MonoBehaviour
{
    public enum NoteType { Type1, Type2, Type3, Type4, Type5, Type6 }
    public NoteType noteType;

    public bool IsMoving { get; private set; } = true;

    private NovelButtonManager manager;

    [Header("ノーツが消えるX位置")]
    [SerializeField] private float destroyX = -10f;

    void Awake()
    {
        manager = FindObjectOfType<NovelButtonManager>();
        manager?.RegisterNote(this);
    }

    void Update()
    {
        // 左端まで到達したら自動削除
        if (transform.position.x < destroyX)
        {
            manager?.UnregisterNote(this);
            Destroy(gameObject);
        }
    }

    public void Move(float delta)
    {
        transform.Translate(Vector2.left * delta);
    }

    public void StopMove() => IsMoving = false;
    public void ResumeMove() => IsMoving = true;

    public int GetNoteTypeIndex() => (int)noteType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("JudgeLine"))
        {
            manager?.StopAllNotes();
        }
    }

    private void OnDestroy()
    {
        manager?.UnregisterNote(this);
    }
}
