using UnityEngine;

public class ImageChange : MonoBehaviour
{
    [Header("変更対象")]
    [SerializeField] private SpriteRenderer _targetRenderer;

    [Header("切り替えるスプライト")]
    [SerializeField] private Sprite _newSprite;

    public void OnImageChange()
    {
        if (_targetRenderer != null && _newSprite != null)
        {
            _targetRenderer.sprite = _newSprite;
        }
    }
}
