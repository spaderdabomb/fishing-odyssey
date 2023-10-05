using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using UnityEditor.Rendering.LookDev;
using static UnityEngine.Mesh;
using QuickEye.UIToolkit;
using Dialogue;

public partial class UIGameScene
{
    public string[] optionButtonNames;
    public List<UIMenuButton> optionsButtons = new();
    public List<Tab> playerInfoTabs = new();
    public VisualElement root;

    public MenuMissions menuMissions;
    public MenuCollections menuCollections;
    public UIGameScene(VisualElement root)
    {
        optionButtonNames = new string[] { "Stats", "Settings", "Quit" };
        this.root = root;

        AssignQueryResults(root);
        InitCollectionMenu();
        InitOptionsMenu();
        InitPlayerInfoMenu();
        InitMissionsMenu();
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
        menuCollections = new MenuCollections(menuCollectionsRoot);
    }

    private void InitMissionsMenu()
    {
        menuMissions = new MenuMissions(menuMissionsRoot);
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
/*        scrollViewCollections.Clear();
        collectionSlots.Clear();*/
    }

    public void TogglePlayerDataMenu()
    {
        // If different menu already open, return (first && statement because menuOptions inside of allMenus)
        if ((allMenus.style.display == DisplayStyle.Flex && menuOptions.style.display == DisplayStyle.Flex) ||
            (DialogueManager.Instance.dialogueNPC.root.style.display == DisplayStyle.Flex))
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
        if (allMenus.style.display == DisplayStyle.Flex || DialogueManager.Instance.dialogueNPC.root.style.display == DisplayStyle.Flex)
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
        UIGameManager.Instance.SetPlayerInMenuOptions(MenuType.PlayerInfo);
    }

    public void MenuClosed()
    {
        gameSceneContainer.style.display = DisplayStyle.Flex;
        allMenus.style.display = DisplayStyle.None;
        if (DialogueManager.Instance.dialogueNPC.root.style.display == DisplayStyle.Flex)
        {
            DialogueManager.Instance.ExitDialogueMode();
        }
        DialogueManager.Instance.dialogueNPC.root.style.display = DisplayStyle.None;
        UIGameManager.Instance.SetPlayerInMenuOptions(MenuType.GameScene);
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
/*        bool hasBeenCaught = DataManager.Instance.GetFishBool(caughtFishData);
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
        }*/
    }

    public void AddInventoryToPlayerInfo(VisualElement inventory)
    {
        inventoryLeftContainer.Add(inventory);
    }

    public void AddElementToGearContainer(VisualElement newElement)
    {
        gearContainerLayout.Add(newElement);
    }

    public VisualElement GetGhostIconRef()
    {
        return ghostIcon;
    }

    public Label GetGhostIconLabelRef()
    {
        return ghostIconLabel;
    }
}
