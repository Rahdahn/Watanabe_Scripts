using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public static PlayerInputReader Instance { get; private set; }

    private PlayerInputActions inputActions;

    public Vector2 PointerPosition => inputActions.UI.Point.ReadValue<Vector2>();
    public bool ClickStarted => clickStarted;
    public bool ClickHeld => clickHeld;
    public bool ClickReleased => clickReleased;

    private bool clickStarted, clickHeld, clickReleased;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inputActions = new PlayerInputActions();
        inputActions.UI.Enable();

        inputActions.UI.Click.started += _ => clickStarted = true;
        inputActions.UI.Click.performed += _ => clickHeld = true;
        inputActions.UI.Click.canceled += _ => {
            clickReleased = true;
            clickHeld = false;
        };
    }

    void LateUpdate()
    {
        clickStarted = false;
        clickReleased = false;
    }
}
