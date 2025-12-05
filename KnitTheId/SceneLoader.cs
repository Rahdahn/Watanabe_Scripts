using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneLoader : MonoBehaviour
{
    [SceneName]
    public string sceneToLoad;

    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                float currentTime = Time.time;
                if (currentTime - lastTapTime < doubleTapThreshold)
                {
                    LoadScene();
                }

                lastTapTime = currentTime;
            }
        }

#if UNITY_EDITOR
        // デバッグ用：エディターでもマウスで動作確認
        if (Application.isEditor && Input.GetMouseButtonDown(0))
        {
            float currentTime = Time.time;
            if (currentTime - lastTapTime < doubleTapThreshold)
            {
                LoadScene();
            }

            lastTapTime = currentTime;
        }
#endif
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            FindObjectOfType<SceneFader>().FadeToScene(sceneToLoad);

        }
    }
}
