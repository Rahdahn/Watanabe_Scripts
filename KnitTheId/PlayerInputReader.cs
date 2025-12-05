using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch; // optional (not required here)

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

    public List<Vector2> GetActivePointerPositions()
    {
        var list = new List<Vector2>();

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton != null && mouse.leftButton.isPressed)
        {
            list.Add(mouse.position.ReadValue());
        }

        var ts = Touchscreen.current;
        if (ts != null)
        {
            foreach (var touch in ts.touches)
            {
                if (touch.press != null && touch.press.isPressed)
                {
                    list.Add(touch.position.ReadValue());
                }
            }
        }

        return list;
    }

    public List<Vector2> GetPointerStartPositions()
    {
        var list = new List<Vector2>();

        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton != null && mouse.leftButton.wasPressedThisFrame)
        {
            list.Add(mouse.position.ReadValue());
        }

        var ts = Touchscreen.current;
        if (ts != null)
        {
            foreach (var touch in ts.touches)
            {
                if (touch.press != null && touch.press.wasPressedThisFrame)
                {
                    list.Add(touch.position.ReadValue());
                }
            }
        }

        return list;
    }
}
