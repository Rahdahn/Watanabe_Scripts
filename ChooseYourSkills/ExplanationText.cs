using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExplanationText : MonoBehaviour
{
    [Header("�}�E�X�ŏd�Ȃ�Ώۂ�UI�{�^��")]
    public List<RectTransform> targetButtons;

    [Header("�e�{�^���ɑΉ����������")]
    public List<string> displayTexts;

    [Header("�\�����TMP")]
    public TextMeshProUGUI displayTextUI;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        for (int i = 0; i < targetButtons.Count; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(targetButtons[i], mousePos, mainCamera))
            {
                if (i < displayTexts.Count)
                {
                    displayTextUI.text = displayTexts[i];
                }
                return;
            }
        }

        displayTextUI.text = "";
    }
}
