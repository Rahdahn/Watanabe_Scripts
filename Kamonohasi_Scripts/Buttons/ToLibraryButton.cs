using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToLibraryButton : MonoBehaviour
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
        FadeManager.Instance.LoadScene("LibraryScene", 1.0f);
    }
}
