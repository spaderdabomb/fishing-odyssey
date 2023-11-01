using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

public partial class RaritySlotContainer : ITabInterface
{
    public VisualElement root;
    public FishData fishData;
    public ObjectRarity objectRarity;
    public RaritySlotContainer(VisualElement root, FishData fishdata, ObjectRarity objectRarity, int index)
    {
        this.root = root;
        this.fishData = fishdata;
        this.objectRarity = objectRarity;

        AssignQueryResults(root);
        tabRoot.tabIndex = index;
        RegisterCallbacks();
        InitRaritySlot();
    }

    private void InitRaritySlot()
    {
        raritySlotLabel.text = objectRarity.ToString();
        rarityColorBg.style.unityBackgroundImageTintColor = GameManager.Instance.gameData.rarityToColorDict[objectRarity];
        rarityFishIcon.style.backgroundImage = DataManager.Instance.IsFishCaught(fishData.fishID) ? fishData.fishIcon : fishData.uncaughtFishIcon;
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
        UIGameManager.Instance.uiGameScene.menuCollections.SetFishRaritySlotIndex(tabRoot.tabIndex);
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
