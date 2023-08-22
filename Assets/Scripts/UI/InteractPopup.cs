using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static UnityEditor.Progress;

public class InteractPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keyBindLabel;
    [SerializeField] TextMeshProUGUI interactObjectText;
    private PlayerInputActions playerInput;
    private string currentInputKey = "";
    private PlayerInteract playerInteract;

    private void Start()
    {
        playerInput = new();

        if (InputManager.Instance.playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            currentInputKey = playerInput.PlayerInteract.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("Keyboard&Mouse"));
        }
        else if (InputManager.Instance.playerInput.currentControlScheme == "Gamepad")
        {
            currentInputKey = playerInput.PlayerInteract.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("Gamepad"));
        }
    }

    public void ShowPopup(string objectTextStr, Color keyBindColor)
    {
        gameObject.SetActive(true);
        string keybindHexColor = "#" + ColorUtility.ToHtmlStringRGBA(keyBindColor);
        keyBindLabel.text = "[" + "<color=" + keybindHexColor + ">" + currentInputKey + "</color>" + "] Pick up";
        interactObjectText.text = objectTextStr;
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
