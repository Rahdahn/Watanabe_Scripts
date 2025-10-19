using UnityEngine;
using UnityEngine.InputSystem;

public class ExplanationReady : MonoBehaviour
{
    [SerializeField] private GameObject _readyObject;
    [SerializeField] private GameObject _buttonGuide;

    private ChangeScene _changeScene;
    private bool _already = false;

    void Start()
    {
        _changeScene = GameObject.Find("SceneManager").GetComponent<ChangeScene>();
    }

    public void OnReady(InputAction.CallbackContext context)
    {
        if (context.performed && !_already)
        {
            _changeScene.readyIndex++;

            if (_readyObject != null)
            {
                _readyObject.SetActive(false);
                _buttonGuide.SetActive(false);
            }

            _already = true;
        }
    }
}
