using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LibraryToMix : MonoBehaviour
{
    public Button titleButton;

    void Start()
    {
        if (titleButton != null)
        {
            titleButton.onClick.AddListener(OnTitleButtonClick);
        }
    }

    void OnTitleButtonClick()
    {
        FadeManager.Instance.LoadScene("MixScene(Test)", 1.0f);
    }
}
