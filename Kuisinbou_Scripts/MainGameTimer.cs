using NaughtyAttributes;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class MainGameTimer : MonoBehaviour
{
    [SerializeField] private float _mainTime = 99f;
    [SerializeField] private UnityEngine.UI.Image _timerImage;
    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField, Scene] private string _sceneName;

    [Header("TimeUp Settings")]
    [SerializeField] private TextMeshProUGUI _timeUpText;
    [SerializeField] private float _scaleUp = 3f;
    [SerializeField] private float _animDuration = 0.6f;
    [SerializeField] private float _timeUpDisplayDuration = 1.5f;

    private float _timeRemaining;
    private bool _canStartTimer = false;
    private bool _isTimeUpShown = false;
    private bool _hasSwitchedBGM = false;

    private void Start()
    {
        _timeRemaining = _mainTime;
        UpdateUI();
        StartCoroutine(DelayedStart(1f));
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        _canStartTimer = true;
    }

    private void Update()
    {
        if (!_canStartTimer || CountdownManager.IsCountingDown || _isTimeUpShown)
            return;

        if (_timeRemaining <= 30f && !_hasSwitchedBGM)
        {
            _hasSwitchedBGM = true;
            BGMManager.Instance?.SwitchToMainGameBGM2();
        }

        if (_timeRemaining > 0)
        {
            _timeRemaining -= Time.unscaledDeltaTime;
            if (_timeRemaining < 0) _timeRemaining = 0;
            UpdateUI();

            if (_timeRemaining <= 0)
            {
                StartCoroutine(ShowTimeUpAndLoadScene());
            }
        }
    }

    private IEnumerator ShowTimeUpAndLoadScene()
    {
        _isTimeUpShown = true;

        Time.timeScale = 0f;

        if (_timeUpText != null)
        {
            _timeUpText.gameObject.SetActive(true);
            _timeUpText.text = "TIME UP";
            _timeUpText.transform.localScale = Vector3.zero;
            _timeUpText.alpha = 0;

            _timeUpText.DOFade(1f, 0f).SetUpdate(true);
            _timeUpText.transform
                .DOScale(_scaleUp, _animDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(_timeUpDisplayDuration);

        LoadNextScene();
    }

    private void UpdateUI()
    {
        if (_timerImage != null)
        {
            _timerImage.fillAmount = _timeRemaining / _mainTime;
        }

        if (_timerText != null)
        {
            _timerText.text = Mathf.CeilToInt(_timeRemaining).ToString();
        }
    }

    private void LoadNextScene()
    {
        SceneFader.Instance.FadeToScene(_sceneName);
        Time.timeScale = 1f;
    }
}
