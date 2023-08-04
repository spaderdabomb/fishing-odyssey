using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "GameData", menuName = "Fishing Odyssey/GameData")]
public class GameData : ScriptableObject
{
    public float baseSpawnChance = 1f / 30f;

    public List<Color> rarityColors;
    public List<FishRarity> fishRarities;
    public Dictionary<FishRarity, Color> rarityToColorDict;
    public Dictionary<BiomeType, FishType[]> fishInBiomeDict;

    // Health
    [SerializeField, ReadOnly] private FishData lastFishCaught;
    public FishData LastFishCaught
    {
        get { return lastFishCaught; }
        set
        {
            lastFishCaught = value;
            OnFishCaught?.Invoke(lastFishCaught);
        }
    }

    public UnityAction<FishData> OnFishCaught;

    public void InitDefaults()
    {
        rarityToColorDict = new();
        fishInBiomeDict = new();
        lastFishCaught = GameManager.Instance.allFishData[0];

        for (int i = 0; i < fishRarities.Count; i++)
        {
            rarityToColorDict.Add(fishRarities[i], rarityColors[i]);
        }

        FishType[] grassyFishTypes = new FishType[] { FishType.Salmon, FishType.Tuna, FishType.Trout, FishType.Bass };
        FishType[] desertFishTypes = new FishType[] { FishType.Salmon, FishType.Tuna, FishType.Trout, FishType.Bass };
        fishInBiomeDict.Add(BiomeType.Grassy, grassyFishTypes);
        fishInBiomeDict.Add(BiomeType.Desert, desertFishTypes);
    }
}

public enum BiomeType
{
    Grassy,
    Desert
}

public enum ObjectRarities
{
    None,
    Common,
    Uncommon,
    Rare,
    Special,
    Epic,
    Legendary,
    Mythic
}
