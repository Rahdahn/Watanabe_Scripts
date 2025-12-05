using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public BoardManager _boardM;
    private SpriteRenderer _sr;
    private Color _originalColor;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;
    }

    private void OnMouseDown()
    {
        _boardM.SelectTile(this);
    }

    public void HighlightTile(bool enable)
    {
        if (enable)
        {
            _sr.DOColor(_originalColor * 1.3f, 0.15f);
        }
        else
        {
            _sr.DOColor(_originalColor, 0.15f);
        }
    }
}