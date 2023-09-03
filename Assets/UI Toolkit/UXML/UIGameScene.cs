using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using UnityEditor.Rendering.LookDev;
using static UnityEngine.Mesh;
using QuickEye.UIToolkit;

public partial class UIGameScene
{
    public string[] optionButtonNames;
    public List<CollectionSlot> collectionSlots = new();
    public List<UIMenuButton> optionsButtons = new();
    public List<Tab> playerInfoTabs = new();
    public VisualElement ghosticonRef;
    public VisualElement root;
    public UIGameScene(VisualElement root)
    {
        optionButtonNames = new string[] { "Stats", "Settings", "Quit" };
        this.root = root;

        AssignQueryResults(root);
        InitCollectionMenu();
        InitOptionsMenu();
        InitPlayerInfoMenu();
    }

    private void InitPlayerInfoMenu()
    {
        foreach (Tab tab in tabGroup.contentContainer.Children())
        {
            playerInfoTabs.Add(tab);
            tab.RegisterValueChangedCallback(evt => OnPlayerInfoTabChanged(evt, tab));
        }
    }

    private void InitCollectionMenu()
    {
        for (int i = 0; i < GameManager.Instance.allFishData.Count; i++)
        {
            VisualElement newSlotTemplate = UIGameManager.Instance.collectionSlot.CloneTree();
            CollectionSlot newSlot = new CollectionSlot(newSlotTemplate, GameManager.Instance.allFishData[i]);
            collectionSlots.Add(newSlot);
            scrollViewCollections.Add(newSlotTemplate);
        }
    }

    private void InitOptionsMenu()
    {
        foreach (string buttonName in optionButtonNames)
        {
            VisualElement optionsButtonTemplate = UIGameManager.Instance.optionsButton.CloneTree();
            UIMenuButton spawnedOptionsButton = new UIMenuButton(optionsButtonTemplate, buttonName);
            optionsButtons.Add(spawnedOptionsButton);
            optionsButtonContainer.Add(optionsButtonTemplate);
        }
    }

    public void ClearCollectionMenu()
    {
        scrollViewCollections.Clear();
        collectionSlots.Clear();
    }

    public void TogglePlayerDataMenu()
    {
        // If different menu already open, return
        if (allMenus.style.display == DisplayStyle.Flex && menuOptions.style.display == DisplayStyle.Flex)
        {
            return;
        }

        // Menu closing
        if (allMenus.style.display == DisplayStyle.Flex)
        {
            MenuClosed();
        }
        // Menu opening
        else
        {
            MenuOpened();
            menuOptions.style.display = DisplayStyle.None;
            menuPlayerInfo.style.display = DisplayStyle.Flex;
        }
    }

    public void ToggleOptionsMenu()
    {
        // Menu closing
        if (allMenus.style.display == DisplayStyle.Flex)
        {
            MenuClosed();
        }
        // Menu opening
        else
        {
            MenuOpened();
            menuOptions.style.display = DisplayStyle.Flex;
            menuPlayerInfo.style.display = DisplayStyle.None;
        }
    }

    public void MenuOpened() 
    {
        allMenus.style.display = DisplayStyle.Flex;
        gameSceneContainer.style.display = DisplayStyle.None;

        InputManager.Instance.playerInputActions.Disable();
        InputManager.Instance.gameInput.InMenuInput.Enable();
        UIGameManager.Instance.SetCursorStateVisible(true);
    }

    public void MenuClosed()
    {
        gameSceneContainer.style.display = DisplayStyle.Flex;
        allMenus.style.display = DisplayStyle.None;

        InputManager.Instance.playerInputActions.Enable();
        InputManager.Instance.gameInput.InMenuInput.Disable();
        UIGameManager.Instance.SetCursorStateVisible(false);
    }

    private void OnPlayerInfoTabChanged(ChangeEvent<bool> evt, VisualElement tab)
    {
        Debug.Log(tab);
    }

    public void OnFishPowerChanged(float newValue)
    {
        fishPowerMeter.style.height = Length.Percent(newValue);
    }

    public void OnFishCaught(FishData caughtFishData)
    {
        Debug.Log(caughtFishData.fishID);
        bool hasBeenCaught = DataManager.Instance.GetFishBool(caughtFishData);
        if (hasBeenCaught)
        {

        }
        else
        {
            DataManager.Instance.SetFishBool(caughtFishData, true);
            DataManager.Instance.SaveData();

            Debug.Log(DataManager.Instance.GetFishBool(caughtFishData));
            ClearCollectionMenu();
            InitCollectionMenu();
        }
    }

    public void AddInventoryToPlayerInfo(VisualElement inventory)
    {
        inventoryLeftContainer.Add(inventory);
    }

    public void AddElementToGearContainer(VisualElement newElement)
    {
        gearContainerLayout.Add(newElement);
    }
}
