using UnityEngine;
using UnityEngine.UI;

public class DaySpriteManager : MonoBehaviour
{
    [Header("Dayごとのスプライト設定 (1日目, 2日目, 3日目)")]
    [SerializeField] private Sprite _day1Sprite;
    [SerializeField] private Sprite _day2Sprite;
    [SerializeField] private Sprite _day3Sprite;

    [Header("表示するImage")]
    [SerializeField] private Image _targetImage;

    void Start()
    {
        if (_targetImage == null)
        {
            Debug.LogError("Target Image が設定されていません");
            return;
        }

        // 現在のDayに応じてスプライトを切り替え
        switch (GameManager.Day)
        {
            case 1:
                _targetImage.sprite = _day1Sprite;
                break;
            case 2:
                _targetImage.sprite = _day1Sprite;
                break;
            case 3:
                _targetImage.sprite = _day1Sprite;
                break;
            default:
                break;
        }
    }
}
