using System.Diagnostics;
using UnityEngine;

public class ImageChange : MonoBehaviour
{
    [Header("�ύX�Ώ�")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("�؂�ւ���X�v���C�g")]
    [SerializeField] private Sprite newSprite;

    public void OnImageChange()
    {
        if (targetRenderer != null && newSprite != null)
        {
            targetRenderer.sprite = newSprite;
        }
    }
}
