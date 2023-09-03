using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishingRodData", menuName = "Fishing Odyssey/FishingRod", order = 1)]
public class FishingRodData : ScriptableObject
{
    public FishingRodType type;
    public ObjectRarities rarity;
    public GameObject fishingRodPrefab;

    public FishingRodStats fishingRodStats;

    [SerializeField] private float hookChance;
    [SerializeField] private float maxCastDistance;
    [SerializeField] private float waitTimeMultiplier;
    [SerializeField] private float escapeChanceMultiplier;

    public void InitDefaults()
    {
        fishingRodStats = new FishingRodStats(type, rarity, hookChance, maxCastDistance, waitTimeMultiplier, escapeChanceMultiplier);
    }
}

public struct FishingRodStats
{
    public FishingRodType type;
    public ObjectRarities rarity;

    public float hookChance;
    public float maxCastDistance;
    public float waitTimeMultiplier;
    public float escapeChanceMultiplier;

    public FishingRodStats(FishingRodType type, ObjectRarities rarity, 
                           float hookChance, float maxCastDistance, float waitTimeMultiplier, float escapeChanceMultiplier)
    {
        float scaleFactor = 1f;
        if (rarity == ObjectRarities.Common)
            scaleFactor = 1f;
        else if (rarity == ObjectRarities.Uncommon)
            scaleFactor = 1.5f;
        
        this.type = type;
        this.rarity = rarity;

        this.hookChance = hookChance * scaleFactor;
        this.maxCastDistance = maxCastDistance * scaleFactor;
        this.waitTimeMultiplier = waitTimeMultiplier / scaleFactor;
        this.escapeChanceMultiplier = escapeChanceMultiplier / scaleFactor;
    }
}

public enum FishingRodType
{
    BasicRod,
    SpeedRod
}

public enum FishingMethod
{ 
    Standard,
    NettingBig,
    NettingSmall,
    Harpooning
}
