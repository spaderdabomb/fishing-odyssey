using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "GameData", menuName = "Fishing Odyssey/GameData")]
public class GameData : SerializedScriptableObject
{
    [Header("General")]
    public float baseSpawnChance = 1f / 30f;
    public Dictionary<ObjectRarity, Color> rarityToColorDict;

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
        lastFishCaught = GameManager.Instance.allFishData[0];
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

public enum ObjectRarity
{
    None = 0, 
    Common = 1,
    Uncommon = 2,
    Rare = 3,
    Epic = 4,
    Legendary = 5,
    Mythic = 6
}
