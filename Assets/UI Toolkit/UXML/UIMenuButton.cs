using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

public partial class UIMenuButton
{
    public UIMenuButton(VisualElement root, string buttonName)
    {
        AssignQueryResults(root);
        OnInstantiated(buttonName);
    }

    private void OnInstantiated(string buttonName)
    {
        menuButton.clickable.clicked += OnClick;
        menuButton.RegisterCallback<MouseEnterEvent>(OnHover);
        menuButton.text = buttonName;
    }

    private void OnHover(MouseEnterEvent evt)
    {
        AudioManager.PlaySound(MainAudioLibrarySounds.WoodenTick);
    }

    private void OnClick()
    {
        AudioManager.PlaySound(MainAudioLibrarySounds.ConfirmTick);
    }
}
