using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using QuickEye.UIToolkit;
using UnityEngine.Rendering.Universal;
using System;

public partial class MenuCollections
{
    public VisualElement root;
    private List<CollectionsBiomeTab> biomeTabs;
    private List<BiomeData> biomeDataList;
    private List<CollectionSlot> currentCollectionSlots;

    private const string biomeTabStyle = "biome-tab";
    public MenuCollections(VisualElement root)
    {
        this.root = root;
        AssignQueryResults(root);
        InitMenuCollections();
    }

    private void InitMenuCollections()
    {
        InitBiomeTabs();
    }

    private void SetCallbacks()
    {

    }

    private void RemoveCallbacks()
    {

    }

    private void InitBiomeTabs()
    {
        biomeTabs = new List<CollectionsBiomeTab>();
        biomeDataList = new List<BiomeData>();
        currentCollectionSlots = new List<CollectionSlot>();
        Dictionary<string, BiomeData> biomeDict = BiomeExtensions.GetAllData();

        int i = 0;
        foreach (var kvp in biomeDict)
        {
            VisualElement collectionBiomeTabClone = UIGameManager.Instance.collectionsBiomeTab.CloneTree();
            CollectionsBiomeTab collectionBiomeTab = new CollectionsBiomeTab(collectionBiomeTabClone, i);
            biomeTabGroup.Add(collectionBiomeTabClone);
            biomeTabs.Add(collectionBiomeTab);
            biomeDataList.Add(kvp.Value);
            i++;
        }

        SetBiomeTabIndex(0);
    }

    public void SetBiomeTabIndex(int index)
    {
        for (int i = 0; i < biomeTabs.Count; i++)
        {
            CollectionsBiomeTab biomeTab = biomeTabs[i];
            biomeTab.SetTabSelectedValue(i == index);
        }

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

        SetCollectionSlotIndex(0);
    }

    public void SetCollectionSlotIndex(int index)
    {
        for (int i = 0; i < currentCollectionSlots.Count; i++)
        {
            CollectionSlot collectionSlot = currentCollectionSlots[i];
            collectionSlot.SetTabSelectedValue(i == index);
        }
    }

    private void SetFishRarityUI(FishData fishData)
    {
        foreach (ObjectRarity objectRarity in Enum.GetValues(typeof(ObjectRarity)))
        {

        }
    }

    public void IncrementCollectionProgress()
    {

    }
}
