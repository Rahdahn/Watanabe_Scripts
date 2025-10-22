using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    public Image gaugeImage;
    public float fillSpeed = 0.5f;  // ゲージの充填速度
    public float maxFillAmount = 0.75f;  // ゲージの最大値（75%）

    private float currentFillAmount = 0f;
    private bool isFilling = true;


    void Update()
    {
        // ゲージを充填または減少させる処理
        if (isFilling)
        {
            currentFillAmount += fillSpeed * Time.deltaTime;
            if (currentFillAmount >= maxFillAmount)
            {
                currentFillAmount = maxFillAmount;
                isFilling = false;  // 充填が完了したら減少フェーズに移行
            }
        }
        else
        {
            currentFillAmount -= fillSpeed * Time.deltaTime;
            if (currentFillAmount <= 0f)
            {
                currentFillAmount = 0f;
                isFilling = true;  // 減少が完了したら充填フェーズに戻る
            }
        }

        gaugeImage.fillAmount = currentFillAmount;
    }

    public void SetFillAmount(float amount)
    {
        currentFillAmount = Mathf.Clamp01(amount) * maxFillAmount;
        gaugeImage.fillAmount = currentFillAmount;
    }

    public void ResetGauge()
    {
        currentFillAmount = 0f;
        gaugeImage.fillAmount = currentFillAmount;
        isFilling = true; // 充填フェーズに戻す
    }
}