#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[InitializeOnLoad]
public class BeatCounterWindow : EditorWindow
{
    private static BeatCounter _beatCounter;

    [MenuItem("Tools/Beat Counter Viewer")]
    public static void ShowWindow()
    {
        GetWindow<BeatCounterWindow>("Beat Counter Viewer");
    }

    private void OnEnable()
    {
        // Playモード変更時に呼び出されるイベント登録
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        FindBeatCounter();
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    // Playモード開始・終了時に自動でBeatCounterを探す
    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode ||
            state == PlayModeStateChange.EnteredEditMode)
        {
            FindBeatCounter();
        }
    }

    private void FindBeatCounter()
    {
        _beatCounter = FindObjectOfType<BeatCounter>();
    }
    private void OnGUI()
    {
        GUILayout.Label("Beat Counter Viewer", EditorStyles.boldLabel);

        _beatCounter = (BeatCounter)EditorGUILayout.ObjectField("BeatCounter", _beatCounter, typeof(BeatCounter), true) as BeatCounter;
        if (_beatCounter == null)
        {
            if (GUILayout.Button("シーン内のBeatCounterを検索"))
            {
                FindBeatCounter();
            }
            return;
        }

        if (_beatCounter != null)
        {
            GUILayout.Label($"BPM: {_beatCounter._targetBPM}");
            GUILayout.Label($"ビート数: {_beatCounter._beatCount}");
        }
    }

    private void Update()
    {
        Repaint();
    }
}
#endif