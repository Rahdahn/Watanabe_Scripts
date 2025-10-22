using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExplanationText : MonoBehaviour
{
    [Header("マウスで重なる対象のUIボタン")]
    public List<RectTransform> targetButtons;

    [Header("各ボタンに対応する説明文")]
    public List<string> displayTexts;

    [Header("表示先のTMP")]
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
