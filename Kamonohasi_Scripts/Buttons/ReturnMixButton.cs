using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnMixButton : MonoBehaviour
{
    public Button titleButton;

    void Start()
    {
        if (titleButton != null)
        {
            titleButton.onClick.AddListener(OnMixButtonClick);
        }
    }

    void OnMixButtonClick()
    {
        FadeManager.Instance.LoadScene("MixScene(Test)", 1.0f);
    }
}
