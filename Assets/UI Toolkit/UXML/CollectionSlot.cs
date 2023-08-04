using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CollectionSlot
{
    private VisualElement root;
    public FishData fishData;
    public CollectionSlot(VisualElement newRoot, FishData newFishData)
    {
        root = newRoot;
        AssignQueryResults(root);

        fishData = newFishData;
        InitCollectionSlot();
    }

    private void InitCollectionSlot()
    {
        fishIcon.style.backgroundImage = fishData.fishIcon;
        fishLabel.text = fishData.fishTypeStr;
        rarityLabel.text = fishData.fishRarityStr;
        rarityLabel.style.color = GameManager.Instance.gameData.rarityToColorDict[fishData.fishRarity];
        collectedLabel.text = "0";

        bool fishCollected = DataManager.Instance.GetFishBool(fishData);
        root.SetEnabled(fishCollected);
    }
}
