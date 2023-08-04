using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FishData", menuName = "Fishing Odyssey/Fish", order = 1)]
public class FishData : ScriptableObject
{
    public FishType fishType = FishType.None;
    public FishRarity fishRarity = FishRarity.None;
    public Texture2D fishIcon;

    public float catchWeightChance = 100f;
    public float health = 0f;
    public float swimSpeed = 5f;
    public float playerFollowSpeed = 10f;
    public float fleeSpeed = 10f;

    public float stateChangeTimerMin = 10f;
    public float stateChangeTimerMax = 15f;
    public FishMoveState[] allowableMoveStates;

    [HideInInspector] public string fishTypeStr;
    [HideInInspector] public string fishRarityStr;
    [HideInInspector] public string fishID;

    // Health
    [SerializeField, ReadOnly] private float healthCurrent;
    public float HealthCurrent
    {
        get { return healthCurrent; }
        set
        {
            if (healthCurrent != value)
            {
                healthCurrent = Mathf.Max(value, 0);
                HealthChanged?.Invoke(healthCurrent);
            }
        }
    }

    public UnityAction<float> HealthChanged;


    public void InitDefaults()
    {
        healthCurrent = health;
        fishTypeStr = fishType.ToString();
        fishRarityStr = fishRarity.ToString();
        fishID = GetUniqueFishID(fishType, fishRarity);
    }

    public string GetUniqueFishID(FishType newFishType, FishRarity newFishRarity)
    {
        int fishTypeValue = (int)newFishType;
        int rarityValue = (int)newFishRarity;

        return $"{fishTypeValue:D2}{rarityValue:D2}";
    }
}

public enum FishRarity
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

public enum FishMoveState
{
    None,
    Idling,
    Swimming,
    Fleeing,
    PlayerFollowing,
    Attacking
}

public enum FishType
{
    None,
    Salmon,
    Tuna,
    Trout,
    Bass,
    Catfish,
    Snapper,
    Cod,
    Flounder,
    Tilapia,
    Mahi_Mahi,
    Swordfish,
    Perch,
    Carp,
    Pike,
    Haddock,
    Grouper,
    Redfish,
    Halibut,
    Snook,
    Mackerel
}