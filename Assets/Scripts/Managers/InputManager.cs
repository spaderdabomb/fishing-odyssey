using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, GameInput.IMenuInputActions
{
    public static InputManager Instance;

    private GameInput gameInput;
    public PlayerInput playerInput;

    // Doesn't work yet
    public LastInputSystem lastInputSystem { get; private set; } = LastInputSystem.KeyboardMouse;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        gameInput.MenuInput.SetCallbacks(this);

        // Subscribe to Input changed event
        InputSystem.onDeviceChange += OnDeviceChange;
        playerInput = GetComponent<PlayerInput>();

    }

    private void OnDisable()
    {
        gameInput.MenuInput.RemoveCallbacks(this);
    }

    private void Update()
    {
        string currentControlScheme = playerInput.currentControlScheme;
        // Debug.Log("Current Control Scheme: " + currentControlScheme);
    }


    public void OnToggleGameMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        UIGameManager.Instance.uiGameScene.TogglePlayerDataMenu();
    }

    public void OnToggleOptionsMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        UIGameManager.Instance.uiGameScene.ToggleOptionsMenu();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added)
        {
            Debug.Log($"New device added: {device}");
        }
        else if (change == InputDeviceChange.Removed)
        {
            Debug.Log($"Device removed: {device}");
        }
    }

    public void OnTestGamepad(InputAction.CallbackContext context)
    {
        print("testing gamepad");

    }

    public enum LastInputSystem
    {
        KeyboardMouse,
        Gamepad,
        Touch,
        None
    }
}
