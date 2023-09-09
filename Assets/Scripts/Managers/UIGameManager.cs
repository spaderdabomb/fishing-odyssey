using ATL.AudioData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(1)]
public class UIGameManager : MonoBehaviour
{
    public static UIGameManager Instance;

    // UI Toolkit
    public UIDocument mainUIDocument;
    public VisualTreeAsset collectionSlot;
    public VisualTreeAsset optionsButton;
    public VisualTreeAsset popupMenuInventory;
    public VisualTreeAsset popupMenuInventoryStatsContainer;

    [HideInInspector] public VisualElement root;
    [HideInInspector] public UIGameScene uiGameScene;

    // Unity UI
    public InteractPopup interactPopoup;

    // Private fields
    private PlayerData playerData;
    private GameData gameData;
    private bool onFirstGuiUpdate = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        root = mainUIDocument.rootVisualElement;
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        uiGameScene = new UIGameScene(root);

        playerData = GameManager.Instance.playerData;
        gameData = GameManager.Instance.gameData;

        playerData.FishPowerChanged += uiGameScene.OnFishPowerChanged;
        gameData.OnFishCaught += uiGameScene.OnFishCaught;
    }

    private void OnDisable()
    {
        playerData.FishPowerChanged -= uiGameScene.OnFishPowerChanged;
        gameData.OnFishCaught -= uiGameScene.OnFishCaught;

    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (!onFirstGuiUpdate)
        {
            onFirstGuiUpdate = true;
        }
    }

    public void SetCursorStateVisible(bool newState)
    {
        if (newState)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    public void SetPlayerInMenuOptions(MenuType menuType)
    {

        if (menuType == MenuType.Options)
        {
            InputManager.Instance.playerInputActions.Disable();
            InputManager.Instance.gameInput.InMenuInput.Enable();
            SetCursorStateVisible(true);
        }
        else if (menuType == MenuType.PlayerInfo)
        {
            InputManager.Instance.playerInputActions.Disable();
            InputManager.Instance.gameInput.InMenuInput.Enable();
            SetCursorStateVisible(true);
        }
        else if (menuType == MenuType.Settings)
        {

        }
        else if (menuType == MenuType.Dialogue)
        {
            InputManager.Instance.playerInputActions.Disable();
            InputManager.Instance.gameInput.InMenuInput.Disable();
            InputManager.Instance.gameInput.InDialogueInput.Enable();
            SetCursorStateVisible(true);
        }
        else if (menuType == MenuType.GameScene)
        {
            InputManager.Instance.playerInputActions.Enable();
            InputManager.Instance.gameInput.InMenuInput.Disable();
            InputManager.Instance.gameInput.InDialogueInput.Disable();
            SetCursorStateVisible(false);
        }
    }
}

public enum MenuType
{
    Options = 0,
    PlayerInfo = 1,
    Settings = 2,
    Dialogue = 3,
    GameScene = 4
}
