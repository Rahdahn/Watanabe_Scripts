using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpawner : MonoBehaviour
{
    [Header("ボタン設定")]
    [SerializeField] private Button[] buttons = new Button[10];
    [SerializeField] private GameObject[] targetObjects = new GameObject[10];

    [Header("UI設定")]
    [SerializeField] private Canvas canvasToDisable;
    [SerializeField] private Canvas canvasSkillText;
    [SerializeField] private Canvas canvasSkillTextbackGrund;
    [SerializeField] private Canvas canvasSkillText2;

    [SerializeField] private Canvas destinationCanvas;

    [SerializeField] private GameManager gameManager;

    private int pressedCount = 0;
    private int[] pressedButtonIndices = new int[4];

    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonPressed(index));
        }
    }

    private void OnButtonPressed(int buttonIndex)
    {
        if (pressedCount >= 4)
            return;

        for (int i = 0; i < pressedCount; i++)
        {
            if (pressedButtonIndices[i] == buttonIndex)
                return;
        }

        pressedButtonIndices[pressedCount] = buttonIndex;
        pressedCount++;

        if (pressedCount == 4)
        {
            ActivateObjects();
            if (canvasToDisable != null)
            {
                canvasToDisable.gameObject.SetActive(false);
                canvasSkillText.gameObject.SetActive(false);
                canvasSkillTextbackGrund.gameObject.SetActive(false);
                canvasSkillText2.gameObject.SetActive(true);
            }
            gameManager.SpawnEnemyAndChar();
        }
    }

    private void ActivateObjects()
    {
        for (int i = 0; i < pressedCount; i++)
        {
            int index = pressedButtonIndices[i];
            if (targetObjects[index] != null)
            {
                targetObjects[index].SetActive(true);
            }

            CopyButtonToCanvas(index, i);
        }
    }

    private void CopyButtonToCanvas(int index, int order)
    {
        if (destinationCanvas == null || buttons[index] == null)
            return;

        GameObject buttonCopy = Instantiate(buttons[index].gameObject, destinationCanvas.transform);
        buttonCopy.name = "CopiedButton_" + index;

        RectTransform rt = buttonCopy.GetComponent<RectTransform>();
        rt.SetParent(destinationCanvas.transform);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.localScale = Vector3.one;
        rt.sizeDelta = new Vector2(300f, 300f);

        int total = pressedCount;
        float spacing = 20f;
        float totalWidth = total * 300f + (total - 1) * spacing;
        float startX = -totalWidth / 2f + 150f;
        float posX = startX + order * (300f + spacing);

        rt.anchoredPosition = new Vector2(posX, 0f);

        // ボタンクリック時にGameManagerの処理を呼ぶ
        Button newButton = buttonCopy.GetComponent<Button>();
        newButton.onClick.RemoveAllListeners();

        if (gameManager != null)
        {
            newButton.onClick.AddListener(() => gameManager.OnButtonClick(buttonCopy));
        }

        TextMeshProUGUI copiedText = buttonCopy.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI originalText = buttons[index].GetComponentInChildren<TextMeshProUGUI>();

        if (copiedText != null && originalText != null)
        {
            copiedText.text = originalText.text;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI が取得できませんでした: " + buttonCopy.name);
        }
    }

}
