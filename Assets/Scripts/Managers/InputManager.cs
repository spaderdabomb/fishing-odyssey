using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, GameInput.IMenuInputActions, GameInput.IInMenuInputActions
{
    public static InputManager Instance;

    public GameInput gameInput;
    public PlayerInput playerInput;
    public PlayerInputActions playerInputActions;

    // Doesn't work yet
    public LastInputSystem lastInputSystem { get; private set; } = LastInputSystem.KeyboardMouse;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        gameInput = new GameInput();
        gameInput.MenuInput.Enable();
        gameInput.MenuInput.SetCallbacks(this);
        gameInput.InMenuInput.SetCallbacks(this);

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

    public void SetInputActionRef<T>(T inputActionRef)
    {
        if (inputActionRef is PlayerInputActions)
        {
            playerInputActions = inputActionRef as PlayerInputActions;
        }
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

    public void OnDropItem(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        InventoryManager.Instance.inventory.DropItem();
    }

    public void OnSplitItemHalf(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        InventoryManager.Instance.inventory.TrySplitItem(true);
    }

    public void OnSplitItemOne(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        // InventoryManager.Instance.inventory.TrySplitItem(false);
    }

    public void OnEquipItem(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        print("Equipping Item");
    }

    public void OnAction1(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
    }

    public enum LastInputSystem
    {
        KeyboardMouse,
        Gamepad,
        Touch,
        None
    }
}
