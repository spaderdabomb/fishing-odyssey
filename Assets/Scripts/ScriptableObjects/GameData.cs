using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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

    [Header("Colors")]
    public Color standardTextColor;
    public Color orangeTextColor;
    public Color standardTintColor;

    [Header("Settings")]
    public int inventoryRows = 8;
    public int inventoryCols = 4;
    public int gearCols = 6;

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
    [Description("Tutorial Island")]
    TutorialIsland = 0,
    [Description("Grassy Biome")]
    Grassy = 1,
    [Description("Desert Island")]
    Desert = 2
}

public enum ObjectRarities
{
    None = 0, 
    Common = 1,
    Uncommon = 2,
    Rare = 3,
    Special = 4, 
    Epic = 5,
    Legendary = 6,
    Mythic = 7
}
