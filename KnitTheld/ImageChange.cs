using System.Diagnostics;
using UnityEngine;

public class ImageChange : MonoBehaviour
{
    [Header("変更対象")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("切り替えるスプライト")]
    [SerializeField] private Sprite newSprite;

    public void OnImageChange()
    {
        if (targetRenderer != null && newSprite != null)
        {
            targetRenderer.sprite = newSprite;
        }
    }
}
