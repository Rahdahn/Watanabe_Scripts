using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class BrailleDotManager : MonoBehaviour
{
    [System.Serializable]
    public class DotData
    {
        public int dotIndex;
        public SpriteRenderer spriteRenderer;
        public UnityEvent onPressed; 
    }

    [SerializeField] private BrailleManager _brailleManager;
    [SerializeField] private List<DotData> _dots = new List<DotData>();

    [SerializeField] private bool _hideWhilePressed = true;

    // 内部状態
    private Camera _mainCamera;
    private bool[] _isPressed;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _isPressed = new bool[_dots.Count];
    }

    private void Update()
    {
        var reader = PlayerInputReader.Instance;
        if (reader == null) return;

        // 現在押されている全ポインタ
        List<Vector2> activePositions = reader.GetActivePointerPositions();

        // それぞれのドットについて「現在押されているか」を判定
        for (int i = 0; i < _dots.Count; i++)
        {
            bool currentlyOver = false;
            var sr = _dots[i].spriteRenderer;
            if (sr != null)
            {
                foreach (var pos in activePositions)
                {
                    if (IsPointerOverSprite(sr, pos))
                    {
                        currentlyOver = true;
                        break;
                    }
                }
            }

            if (currentlyOver && !_isPressed[i])
            {
                _isPressed[i] = true;
                // 見た目変更
                if (_hideWhilePressed && sr != null) sr.enabled = false;

                // BrailleManager に通知
                _brailleManager?.OnDotButtonPressed(_dots[i].dotIndex);

                _dots[i].onPressed?.Invoke();
            }

            if (!currentlyOver && _isPressed[i])
            {
                _isPressed[i] = false;
                if (_hideWhilePressed && sr != null) sr.enabled = true;
            }
        }
    }

    private bool IsPointerOverSprite(SpriteRenderer sr, Vector2 screenPos)
    {
        if (sr == null || _mainCamera == null) return false;

        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = sr.transform.position.z;

        return sr.bounds.Contains(worldPos);
    }

    public void ForcePressDot(int dotIndex)
    {
        int idx = _dots.FindIndex(d => d.dotIndex == dotIndex);
        if (idx >= 0) _isPressed[idx] = true;
    }

    public void ForceReleaseDot(int dotIndex)
    {
        int idx = _dots.FindIndex(d => d.dotIndex == dotIndex);
        if (idx >= 0)
        {
            _isPressed[idx] = false;
            if (_dots[idx].spriteRenderer != null && _hideWhilePressed) _dots[idx].spriteRenderer.enabled = true;
        }
    }
}
