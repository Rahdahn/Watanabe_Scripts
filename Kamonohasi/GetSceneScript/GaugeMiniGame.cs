using UnityEngine;

public class GaugeMiniGame : MonoBehaviour
{
    public GaugeController gaugeController;
    public float successThreshold = 0.1f;

    private System.Action<bool> onCompleteCallback;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckSuccess();
        }
    }

    public void StartMiniGame(System.Action<bool> onComplete)
    {
        onCompleteCallback = onComplete;
        gaugeController.ResetGauge();
        gameObject.SetActive(true);
    }

    private void CheckSuccess()
    {
        float fillAmount = gaugeController.gaugeImage.fillAmount;

        float successRangeMin = 0.75f - successThreshold;
        float successRangeMax = 0.75f + successThreshold;

        bool isSuccess = fillAmount >= successRangeMin && fillAmount <= successRangeMax;
        onCompleteCallback?.Invoke(isSuccess);
        gameObject.SetActive(false);
    }
}
