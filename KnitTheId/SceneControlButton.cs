using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    [Header("タイトルシーンの名前を設定")]
    [SerializeField] private string _titleSceneName = "TitleScene";

    public void GoToTitle()
    {
        if (!string.IsNullOrEmpty(_titleSceneName))
        {
            SceneManager.LoadScene(_titleSceneName);
        }
    }

    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
