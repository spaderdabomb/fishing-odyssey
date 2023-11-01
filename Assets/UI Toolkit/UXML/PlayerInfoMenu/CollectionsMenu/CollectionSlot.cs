using UnityEngine.UIElements;
using UnityEngine;
using JSAM;
using System;

public partial class CollectionSlot : ITabInterface
{
    public VisualElement root;
    public FishData fishData;
    public CollectionSlot(VisualElement root, FishData fishData, int tabIndex)
    {
        AssignQueryResults(root);

        this.root = root;
        this.fishData = fishData;
        tabRoot.tabIndex = tabIndex;
        InitCollectionSlot();
        RegisterCallbacks();
    }

    private void InitCollectionSlot()
    {
        fishIcon.style.backgroundImage = DataManager.Instance.IsFishTypeCaught(fishData) ? fishData.fishIcon : fishData.uncaughtFishIcon;
        fishLabel.text = fishData.name;

        ObjectRarity[] objectRarityArr = (ObjectRarity[])Enum.GetValues(typeof(ObjectRarity));
        for (int i = 0; i < objectRarityArr.Length; i++)
        {
            string currentFishID = fishData.fishName + objectRarityArr[i].ToString();
            rarityLightsContainer[i].style.backgroundImage = DataManager.Instance.IsFishCaught(currentFishID) ? UIGameManager.Instance.statusLightLit : 
                                                                                                                UIGameManager.Instance.statusLightUnlit;
        }
    }

    public void RegisterCallbacks()
    {
        tabRoot.RegisterValueChangedCallback(TabIndexChanged);
        tabRoot.RegisterCallback<PointerEnterEvent>(OnHover);
    }

    public void UnregisterCallbacks()
    {
        tabRoot.UnregisterValueChangedCallback(TabIndexChanged);
        tabRoot.UnregisterCallback<PointerEnterEvent>(OnHover);
    }

    public void TabIndexChanged(ChangeEvent<bool> value)
    {
        UIGameManager.Instance.uiGameScene.menuCollections.SetCollectionSlotIndex(tabRoot.tabIndex);
        AudioManager.PlaySound(MainAudioLibrarySounds.ConfirmTick);
    }

    public void SetTabSelectedValue(bool value)
    {
        tabRoot.SetValueWithoutNotify(value);
    }

    public void OnHover(PointerEnterEvent evt)
    {
        AudioManager.PlaySound(MainAudioLibrarySounds.WoodenTick);
    }
}
