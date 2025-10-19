using UnityEngine;
using TMPro;

public class AnimalCounter : MonoBehaviour
{
    public TextMeshProUGUI d1CountText;
    public TextMeshProUGUI d2CountText;
    public TextMeshProUGUI d3CountText;
    public TextMeshProUGUI d4CountText;
    public TextMeshProUGUI d5CountText;

    void Start()
    {
        UpdateAnimalCountDisplay();
    }

    public void UpdateAnimalCountDisplay()
    {
        // 各種類のカウントをTextMeshProに反映
        d1CountText.text = $"X {GameData.D1Count}";
        d2CountText.text = $"X {GameData.D2Count}";
        d3CountText.text = $"X {GameData.D3Count}";
        d4CountText.text = $"X {GameData.D4Count}";
        d5CountText.text = $"X {GameData.D5Count}";
    }
}
