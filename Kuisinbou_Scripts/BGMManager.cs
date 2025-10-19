using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip mainGameBGM;
    [SerializeField] private AudioClip mainGameBGM2;
    [SerializeField] private AudioClip resultBGM;

    [SerializeField, Range(0f, 1f)] private float volume = 1.0f;
    [SerializeField] private float fadeDuration = 1.0f;

    private AudioSource _singleSource;
    private AudioSource _slowSource;
    private AudioSource _fastSource;
    private Tween _fadeTween;

    private bool _isMainGame = false;
    private bool _isFastActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _singleSource = gameObject.AddComponent<AudioSource>();
        _singleSource.loop = true;
        _singleSource.volume = volume;

        _slowSource = gameObject.AddComponent<AudioSource>();
        _fastSource = gameObject.AddComponent<AudioSource>();
        _slowSource.loop = true;
        _fastSource.loop = true;
        _slowSource.volume = 0f;
        _fastSource.volume = 0f;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateBGM(scene.name);
    }

    private void UpdateBGM(string sceneName)
    {
        if (sceneName == "TitleScene" || sceneName == "EntryScene" || sceneName == "ExplanationScene")
        {
            _isMainGame = false;
            PlaySingle(titleBGM);
        }
        else if (sceneName == "MainGameScene")
        {
            _isMainGame = true;
            PlayMainGame();
        }
        else if (sceneName == "ResultScene")
        {
            _isMainGame = false;
            PlaySingle(resultBGM);
        }
    }

    private void PlaySingle(AudioClip clip)
    {
        StopMainGameSources();

        if (_singleSource.clip == clip) return;

        _fadeTween?.Kill();
        _fadeTween = _singleSource.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            _singleSource.clip = clip;
            _singleSource.Play();
            _singleSource.DOFade(volume, fadeDuration).SetUpdate(true);
        });
    }

    private void PlayMainGame()
    {
        _singleSource.Stop();

        _slowSource.clip = mainGameBGM;
        _fastSource.clip = mainGameBGM2;

        double startTime = AudioSettings.dspTime + 0.1f;
        _slowSource.PlayScheduled(startTime);
        _fastSource.PlayScheduled(startTime);

        _slowSource.volume = volume;
        _fastSource.volume = 0f;

        _isFastActive = false;
    }

    private void StopMainGameSources()
    {
        _slowSource.Stop();
        _fastSource.Stop();
    }

    public void SwitchToMainGameBGM2()
    {
        if (!_isMainGame || _isFastActive) return;

        _slowSource.DOFade(0f, fadeDuration);
        _fastSource.DOFade(volume, fadeDuration);
        _isFastActive = true;
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (_isMainGame)
        {
            _slowSource.volume = _isFastActive ? 0f : volume;
            _fastSource.volume = _isFastActive ? volume : 0f;
        }
        else
        {
            _singleSource.volume = volume;
        }
    }
}
