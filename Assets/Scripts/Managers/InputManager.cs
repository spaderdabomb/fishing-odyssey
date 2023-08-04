using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, GameInput.IMenuInputActions
{
    public static InputManager Instance;

    private GameInput gameInput;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        gameInput.MenuInput.SetCallbacks(this);
    }
    
    private void OnDisable()
    {
        gameInput.MenuInput.RemoveCallbacks(this);
    }


    public void OnToggleGameMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        UIGameManager.Instance.uiGameScene.ToggleGameMenu();
    }
}
