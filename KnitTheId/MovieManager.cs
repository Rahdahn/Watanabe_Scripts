using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class OpeningVideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private float _touchCount;

    public UnityEvent onMovieEnd;
    public UnityEvent onMovieSkip;

    private float lastTapTime = 0f;
    private int tapCount = 0;
    private float tapResetTime = 1.0f;

    void Start()
    {
        if (_videoPlayer == null)
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        _videoPlayer.loopPointReached += OnVideoEnd;
        _videoPlayer.Play();
    }

    void Update()
    {
        bool tapped = false;

        // タッチ入力
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                tapped = true;
            }
        }

        // デバッグ用クリック入力
        if (Input.GetMouseButtonDown(0))
        {
            tapped = true;
        }

        if (tapped)
        {
            float currentTime = Time.time;

            // 前回タップから一定時間以内なら連続タップとしてカウント
            if (currentTime - lastTapTime <= tapResetTime)
            {
                tapCount++;
            }
            else
            {
                tapCount = 1;
            }

            lastTapTime = currentTime;


            if (tapCount >= 5)
            {
                SkipVideo();
                onMovieSkip?.Invoke();
            }
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        _videoPlayer.Stop();
        onMovieEnd?.Invoke();
    }

    private void SkipVideo()
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Stop();
            onMovieEnd?.Invoke();
        }
    }
}
