using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using QuickEye.UIToolkit;
using UnityEngine.Rendering.Universal;
using System;
using Unity.AppUI.UI;

public partial class MenuCollections
{
    public VisualElement root;
    private List<CollectionsBiomeTab> biomeTabs;
    private List<BiomeData> biomeDataList;
    private List<CollectionSlot> currentCollectionSlots;
    private List<RaritySlotContainer> currentFishRaritySlots;
    private int currentBiomeTabIndex = 0;

    private const string biomeTabStyle = "biome-tab";
    public MenuCollections(VisualElement root)
    {
        this.root = root;
        AssignQueryResults(root);
        RegisterCallbacks();
        InitMenuCollections();
    }

    private void InitMenuCollections()
    {
        InitBiomeTabs();
    }

    private void RegisterCallbacks()
    {
        GameEventsManager.Instance.miscEvents.onNewFishCaught += NewFishCaught;
    }

    private void UnregisterCallbacks()
    {
        GameEventsManager.Instance.miscEvents.onNewFishCaught -= NewFishCaught;
    }

    private void InitBiomeTabs()
    {
        biomeTabs = new List<CollectionsBiomeTab>();
        biomeDataList = new List<BiomeData>();
        currentCollectionSlots = new List<CollectionSlot>();
        currentFishRaritySlots = new List<RaritySlotContainer>();
        Dictionary<string, BiomeData> biomeDict = BiomeExtensions.GetAllData();

        int i = 0;
        foreach (var kvp in biomeDict)
        {
            VisualElement collectionBiomeTabClone = UIGameManager.Instance.collectionsBiomeTab.CloneTree();
            CollectionsBiomeTab collectionBiomeTab = new CollectionsBiomeTab(collectionBiomeTabClone, kvp.Value, i);
            biomeTabGroup.Add(collectionBiomeTabClone);
            biomeTabs.Add(collectionBiomeTab);
            biomeDataList.Add(kvp.Value);
            i++;
        }

        UpdateCollectionProgressUI();
        SetBiomeTabIndex(0);
    }

    public void SetBiomeTabIndex(int index)
    {
        for (int i = 0; i < biomeTabs.Count; i++)
        {
            CollectionsBiomeTab biomeTab = biomeTabs[i];
            biomeTab.SetTabSelectedValue(i == index);
        }

        currentBiomeTabIndex = index;
        ClearBiomeTabs();
        SetBiomeTabUI(index);
    }

    private void ClearBiomeTabs()
    {
        foreach (CollectionSlot collectionSlot in currentCollectionSlots)
        {
            collectionSlot.UnregisterCallbacks();
        }

        currentCollectionSlots.Clear();
        collectionSlotContainer.Clear();
    }

    private void SetBiomeTabUI(int index)
    {
        biomeSubheaderLabel.text = biomeDataList[index].displayName;
        Dictionary<BiomeType, List<FishData>> biomeToFishDict = BiomeExtensions.GetAllFishInBiomes();

        int i = 0;
        foreach (KeyValuePair<BiomeType, List<FishData>> kvp in biomeToFishDict)
        {
            if (kvp.Key == biomeDataList[index].biomeType)
            {
                for (int j = 0; j < kvp.Value.Count; j++)
                {
                    FishData fishData = kvp.Value[j];
                    VisualElement collectionSlotClone = UIGameManager.Instance.collectionSlot.CloneTree();
                    CollectionSlot newCollectionSlot = new CollectionSlot(collectionSlotClone, fishData, j);
                    collectionSlotContainer.Add(collectionSlotClone);
                    currentCollectionSlots.Add(newCollectionSlot);
                    newCollectionSlot.SetTabSelectedValue(j == 0);
                }
            }
            i++;
        }

        UpdateFishObtainedLabel();
        SetCollectionSlotIndex(0);
    }

    public void SetCollectionSlotIndex(int index)
    {
        for (int i = 0; i < currentCollectionSlots.Count; i++)
        {
            CollectionSlot collectionSlot = currentCollectionSlots[i];
            collectionSlot.SetTabSelectedValue(i == index);
            if (i == index)
            {
                ClearCurrentFishUI();
                SetCurrentFishUI(collectionSlot.fishData);
            }
        }

        SetFishRaritySlotIndex(0);
    }

    private void ClearCurrentFishUI()
    {
        // Clear fish rarity UI
        foreach (RaritySlotContainer raritySlotContainer in currentFishRaritySlots)
        {
            raritySlotContainer.UnregisterCallbacks();
        }

        currentFishRaritySlots.Clear();
        raritiesSlotLayout.Clear();
    }

    private void SetCurrentFishUI(FishData fishData)
    {
        currentFisherLabel.text = fishData.displayName;
        fishDescriptionLabel.text = fishData.description;
        rarityFishIcon.style.backgroundImage = DataManager.Instance.IsFishCaught(fishData.fishID) ? fishData.fishIcon : fishData.uncaughtFishIcon;
        fishInfoColorBg.style.unityBackgroundImageTintColor = GameManager.Instance.gameData.rarityToColorDict[fishData.fishRarity];

        // Init Fish Rarity UI
        int i = 0;
        foreach (ObjectRarity objectRarity in Enum.GetValues(typeof(ObjectRarity)))
        {
            VisualElement fishRaritySlot = UIGameManager.Instance.raritySlotContainer.CloneTree();
            FishData newFishData = fishData.CreateNew(objectRarity);
            RaritySlotContainer raritySlotContainer = new RaritySlotContainer(fishRaritySlot, newFishData, objectRarity, i);
            currentFishRaritySlots.Add(raritySlotContainer);
            raritiesSlotLayout.Add(fishRaritySlot);
            i++;
        }
    }

    private void SetFishInfoUI(FishData fishData)
    {
        if (DataManager.Instance.FishDateCaughtDict.TryGetValue(fishData.fishID, out DateTime fishCaughtDate) &&
            DataManager.Instance.FishTotalCaughtDict.TryGetValue(fishData.fishID, out int fishTotal) &&
            DataManager.Instance.FishCaughtBestWeightDict.TryGetValue(fishData.fishID, out float fishWeight))
        {
            infoLabelLeft1.text = "First Caught";
            infoLabelRight1.text = fishCaughtDate.ToString();
            infoLabelLeft2.text = "Total Caught";
            infoLabelRight2.text = fishTotal.ToString();
            infoLabelLeft3.text = "Best Weight";
            infoLabelRight3.text = fishWeight.ToString("0.00");
            rarityFishIcon.style.backgroundImage = fishData.fishIcon;
        }
        else
        {
            infoLabelLeft1.text = "First Caught";
            infoLabelRight1.text = "-";
            infoLabelLeft2.text = "Total Caught";
            infoLabelRight2.text = "0";
            infoLabelLeft3.text = "Best Weight";
            infoLabelRight3.text = "-";
            rarityFishIcon.style.backgroundImage = fishData.uncaughtFishIcon;
        }
    }


    public void SetFishRaritySlotIndex(int index)
    {
        for (int i = 0; i < currentFishRaritySlots.Count; i++)
        {
            RaritySlotContainer raritySlot = currentFishRaritySlots[i];
            raritySlot.SetTabSelectedValue(i == index);
            if (i == index)
            {
                SetFishInfoUI(raritySlot.fishData);
            }
        }
    }

    private void UpdateCollectionProgressUI()
    {
        int fishObtained = DataManager.Instance.GetTotalFishCollected();
        int totalFish = FishDataExtensions.GetTotalUniqueFish();

        float percentComplete = 100 * ((float)fishObtained / (float)totalFish);
        collectionProgressBar.value = percentComplete;
        collectionProgressBar.title = fishObtained.ToString() + "/" + totalFish.ToString();
    }

    private void UpdateFishObtainedLabel()
    {
        BiomeData currentBiomeData = biomeTabs[currentBiomeTabIndex].biomeData;
        List<FishData> biomeFishData = BiomeExtensions.GetAllFishInBiomes()[currentBiomeData.biomeType];
        int numFishInBiome = biomeFishData.Count;
        int numFishCaughtInBiome = DataManager.Instance.GetFishCollectedInBiome(currentBiomeData.biomeType);
        biomeObtainedLabel.text = "Obtained:  " + numFishCaughtInBiome.ToString() + "/" + numFishInBiome.ToString();
    }

    public void NewFishCaught(FishData fishData)
    {
        UpdateCollectionProgressUI();
    }
}
