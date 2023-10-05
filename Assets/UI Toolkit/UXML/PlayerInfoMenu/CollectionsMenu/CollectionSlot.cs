using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

public partial class CollectionSlot : ITabInterface
{
    public VisualElement root;
    public FishData fishData;
    public CollectionSlot(VisualElement root, FishData fishData, int tabIndex)
    {
        AssignQueryResults(root);
        InitCollectionSlot();
        RegisterCallbacks();

        this.root = root;
        this.fishData = fishData;
        tabRoot.tabIndex = tabIndex;
    }

    private void InitCollectionSlot()
    {

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
