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

}
