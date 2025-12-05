using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    public ScoreManager _scoreManager;
    [SerializeField] GameObject _clearObject;
    [SerializeField] private TMP_Text _finalTimeText;

    private float _elapsedTime = 0f;
    private bool _isRunning = false;
    private bool _isPlaying = false;

    [SerializeField] private int _targetScore = 10000;

    private void Update()
    {
        if (!_isRunning) return;

        _elapsedTime += Time.deltaTime;
        int min = Mathf.FloorToInt(_elapsedTime / 60f);
        float sec = _elapsedTime % 60f;

        _timerText.text = $"{min:00}:{sec:00.00}";

        if (_scoreManager.Score >= _targetScore)
        {
            StopTimer();
            _clearObject.SetActive(true);
            _finalTimeText.text = $"TIME  {min:00}:{sec:00.00}";
            Clear();
        }
    }
    public void StartTimer()
    {
        if (_isPlaying) return;
        _isPlaying = true;
        _isRunning = true;
    }

    public void StopTimer()
    {
        _isRunning = false;
    }

    public void ResetTimer()
    {
        _elapsedTime = 0f;
        _isRunning = true;
    }

    private void Clear()
    {
        Invoke(nameof(RestartScene), 5f);
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
