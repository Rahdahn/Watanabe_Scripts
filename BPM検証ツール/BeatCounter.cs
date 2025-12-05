using UnityEngine;
public class BeatCounter : MonoBehaviour
{
    public float _targetBPM = 100f;
    public int _beatCount { get; private set; }
    //private float _lastBeatTime;
    private bool _isRunning = false;
    private float _timer = 0f;

    public void StartCounting()
    {
        _beatCount = 0;
        _timer = 0f;
        _isRunning = true;
    }

    public void StopCounting()
    {
        _isRunning = false;
    }

    private void Update()
    {
        if (!_isRunning) return;
        float secondsPerBeat = 60f / _targetBPM;

        _timer += Time.deltaTime;
        if (_timer >= secondsPerBeat)
        {
            int increment = Mathf.FloorToInt(_timer / secondsPerBeat);
            _beatCount += increment;
            _timer -= increment * secondsPerBeat;
        }
    }
}