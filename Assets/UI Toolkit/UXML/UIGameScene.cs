using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using UnityEditor.Rendering.LookDev;
using static UnityEngine.Mesh;

public partial class UIGameScene
{
    public List<CollectionSlot> collectionSlots = new();
    public UIGameScene(VisualElement root)
    {
        AssignQueryResults(root);
        InitCollectionMenu();
    }

    public void InitCollectionMenu()
    {
        for (int i = 0; i < GameManager.Instance.allFishData.Count; i++)
        {
            VisualElement newSlotTemplate = UIGameManager.Instance.collectionSlot.CloneTree();
            CollectionSlot newSlot = new CollectionSlot(newSlotTemplate, GameManager.Instance.allFishData[i]);
            collectionSlots.Add(newSlot);
            scrollViewCollections.Add(newSlotTemplate);
        }
    }

    public void ClearCollectionMenu()
    {
        scrollViewCollections.Clear();
        collectionSlots.Clear();
    }

    public void ToggleGameMenu()
    {
        if (allMenus.style.display == DisplayStyle.Flex)
        {
            allMenus.style.display = DisplayStyle.None;
            gameSceneContainer.style.display = DisplayStyle.Flex;
            UIGameManager.Instance.SetCursorStateVisible(false);
        }
        else
        {
            allMenus.style.display = DisplayStyle.Flex;
            gameSceneContainer.style.display = DisplayStyle.None;
            UIGameManager.Instance.SetCursorStateVisible(true);
        }
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
            // GameObject spawnedParticles = Instantiate(GameManager.Instance.newFishParticlePrefab);
        }
    }
}
