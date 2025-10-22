using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NovelButtonManager : MonoBehaviour
{
    [Header("�e�m�[�c�^�C�v�ɑΉ�����{�^��")]
    [SerializeField] private Button[] noteButtons = new Button[6];

    [Header("�m�[�c�𐶐�����Spawner")]
    [SerializeField] private NoteSpawner noteSpawner;

    [Header("�m�[�c�̈ړ����x")]
    [SerializeField] private float noteSpeed = 5f;

    private List<Note> allNotes = new List<Note>();
    public bool IsStopped { get; private set; } = false;

    void Start()
    {
        // �{�^���ɃC�x���g�o�^
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
        if (!IsStopped) return; // ��~���̂Ƃ��̂ݔ���

        // JudgeLine�Ŏ~�܂��Ă���m�[�c�ɑΉ�����{�^�����`�F�b�N
        bool hasTarget = false;
        foreach (var note in allNotes)
        {
            if (note != null && note.GetNoteTypeIndex() == buttonIndex)
            {
                hasTarget = true;
                break;
            }
        }

        if (!hasTarget) return; // �Ή�����m�[�c���Ȃ���Ζ���

        // ��~���̃m�[�c������ꍇ�̂ݍĊJ
        ResumeAllNotes();

        // �m�[�c�������ĊJ
        if (noteSpawner != null)
            noteSpawner.SpawnNote();
    }
}
