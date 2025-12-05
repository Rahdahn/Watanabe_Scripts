using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NovelButtonManager : MonoBehaviour
{
    [Header("各ノーツタイプに対応するボタン")]
    [SerializeField] private Button[] noteButtons = new Button[6];

    [Header("ノーツを生成するSpawner")]
    [SerializeField] private NoteSpawner noteSpawner;

    [Header("ノーツの移動速度")]
    [SerializeField] private float noteSpeed = 5f;

    private List<Note> allNotes = new List<Note>();
    public bool IsStopped { get; private set; } = false;

    void Start()
    {
        // ボタンにイベント登録
        for (int i = 0; i < noteButtons.Length; i++)
        {
            int index = i;
            if (noteButtons[i] != null)
                noteButtons[i].onClick.AddListener(() => PressButton(index));
        }
    }

    void Update()
    {
        foreach (var note in allNotes)
        {
            if (note != null && note.IsMoving)
                note.Move(noteSpeed * Time.deltaTime);
        }
    }

    public void RegisterNote(Note note)
    {
        if (!allNotes.Contains(note))
            allNotes.Add(note);
    }

    public void UnregisterNote(Note note)
    {
        if (allNotes.Contains(note))
            allNotes.Remove(note);
    }

    public void StopAllNotes()
    {
        foreach (var note in allNotes)
            note?.StopMove();

        IsStopped = true;
    }

    public void ResumeAllNotes()
    {
        foreach (var note in allNotes)
            note?.ResumeMove();

        IsStopped = false;
    }

    public void PressButton(int buttonIndex)
    {
        if (!IsStopped) return; // 停止中のときのみ反応

        // JudgeLineで止まっているノーツに対応するボタンかチェック
        bool hasTarget = false;
        foreach (var note in allNotes)
        {
            if (note != null && note.GetNoteTypeIndex() == buttonIndex)
            {
                hasTarget = true;
                break;
            }
        }

        if (!hasTarget) return; // 対応するノーツがなければ無視

        // 停止中のノーツがある場合のみ再開
        ResumeAllNotes();

        // ノーツ生成も再開
        if (noteSpawner != null)
            noteSpawner.SpawnNote();
    }
}
