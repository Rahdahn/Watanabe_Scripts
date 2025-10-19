using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SliderGameController : MonoBehaviour
{
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;

    private Slider[] sliders; // 複数のスライダーを管理するための配列

    private int currentSliderIndex = 0;
    private float[] successRangeMin; // 成功範囲の最小値
    private float[] successRangeMax; // 成功範囲の最大値

    public void Start()
    {
        // スライダーを配列に格納
        sliders = new Slider[] { slider1, slider2, slider3, slider4 };

        // 成功範囲の初期化
        successRangeMin = new float[sliders.Length];
        successRangeMax = new float[sliders.Length];

        // 各スライダーごとに成功範囲を設定
        successRangeMin[0] = 1f;
        successRangeMax[0] = 2f;

        successRangeMin[1] = 2f;
        successRangeMax[1] = 3f;

        successRangeMin[2] = 3f;
        successRangeMax[2] = 4f;

        successRangeMin[3] = 4f;
        successRangeMax[3] = 5f;

        // ゲーム開始時に初期化
        Initialize();
    }

    public void Initialize()
    {
        // 最初のスライダーをアクティブにする
        sliders[currentSliderIndex].gameObject.SetActive(true);

        // スライダー移動のコルーチンを開始
        StartCoroutine(MoveSlider());
    }

    IEnumerator MoveSlider()
    {
        Slider currentSlider = sliders[currentSliderIndex];
        float sliderValue = currentSlider.minValue;
        float direction = 1f;

        while (true)
        {
            // スライダーの値を更新
            sliderValue += Time.deltaTime * 2f * direction; // 速度を調整
            currentSlider.value = sliderValue;

            // スライダーが端に達したら方向を変更
            if (sliderValue >= currentSlider.maxValue || sliderValue <= currentSlider.minValue)
            {
                direction *= -1f; // 方向を変更
            }

            if (Input.GetMouseButtonDown(0))
            {
                float clickPosition = Input.mousePosition.x;

                if (clickPosition >= currentSlider.fillRect.position.x && clickPosition <= currentSlider.fillRect.position.x + currentSlider.fillRect.rect.width)
                {
                    float normalizedClick = (clickPosition - currentSlider.fillRect.position.x) / currentSlider.fillRect.rect.width;
                    float actualValue = Mathf.Lerp(currentSlider.minValue, currentSlider.maxValue, normalizedClick);

                    // 成功範囲内のクリックを判定
                    if (actualValue >= successRangeMin[currentSliderIndex] && actualValue <= successRangeMax[currentSliderIndex])
                    {
                        // 次のスライダーまたは結果シーンへ移動
                        if (currentSliderIndex < sliders.Length - 1)
                        {
                            currentSlider.gameObject.SetActive(false); // 現在のスライダーを非アクティブにする
                            currentSliderIndex++;
                            sliders[currentSliderIndex].gameObject.SetActive(true); // 次のスライダーをアクティブにする
                        }
                        else
                        {
                            SceneManager.LoadScene("ResultScene"); // 結果シーンをロード
                        }
                    }
                    else
                    {
                        SceneManager.LoadScene("ResultButScene"); // 失敗シーンをロード
                    }
                }
            }

            yield return null;
        }
    }
}
