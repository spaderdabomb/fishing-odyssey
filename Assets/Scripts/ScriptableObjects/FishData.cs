using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FishData", menuName = "Fishing Odyssey/Fish", order = 1)]
public class FishData : ScriptableObject
{
    public string fishID;
    public string fishName;
    public string displayName;
    public string description;
    public FishType fishType;
    public ObjectRarity fishRarity;
    public BiomeType biomeType;
    public Texture2D fishIcon;
    public Texture2D uncaughtFishIcon;

    public float catchWeightChance = 100f;
    public float health = 0f;
    public float swimSpeed = 5f;
    public float playerFollowSpeed = 10f;
    public float fleeSpeed = 10f;
    public float weightRangeLow = 0.1f;
    public float weightRangeHigh = 10f;
    public float weight = 1f;

    public float stateChangeTimerMin = 10f;
    public float stateChangeTimerMax = 15f;
    public FishMoveState[] allowableMoveStates;

    [HideInInspector] public string fishTypeStr;
    [HideInInspector] public string fishRarityStr;

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

    private void OnValidate()
    {
#if UNITY_EDITOR
        fishName = this.name;
        fishID = this.name + this.fishRarity.ToString();
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}

public static class FishDataExtensions
{
    public static readonly Dictionary<string, FishData> dataDict = new Dictionary<string, FishData>();
    public static string path = "ScriptableObjects/Fish";

    public static FishData GetFishData(string uniqueID)
    {
        if (dataDict.Count <= 0)
        {
            CreateDataDictionary();
        }

        if (dataDict.TryGetValue(uniqueID, out FishData data))
        {
            return data;
        }

        Debug.LogWarning($"Data not found: {uniqueID}");
        return null;
    }

    public static Dictionary<string, FishData> GetAllData()
    {
        if (dataDict.Count <= 0)
        {
            CreateDataDictionary();
        }

        return dataDict;
    }

    public static int GetTotalUniqueFish()
    {
        if (dataDict.Count <= 0)
        {
            CreateDataDictionary();
        }

        return dataDict.Count * Enum.GetValues(typeof(ObjectRarity)).Length;
    }

    private static void CreateDataDictionary()
    {
        var scriptableObjects = Resources.LoadAll<FishData>(path);
        foreach (var scriptableObject in scriptableObjects)
        {
            dataDict.Add(scriptableObject.fishID, scriptableObject);
        }
    }

    public static FishData Clone(this FishData data)
    {
        FishData spawnedData = ScriptableObject.Instantiate(data);
        return spawnedData;
    }

    public static FishData CreateNew(this FishData data, ObjectRarity objectRarity)
    {
        FishData spawnedData = ScriptableObject.Instantiate(data);
        spawnedData.fishRarity = objectRarity;
        spawnedData.fishID = data.name + objectRarity.ToString();
        return spawnedData;
    }
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
    Any = -1,
    None = 0,
    Salmon = 1,
    Tuna = 2,
    Trout = 3,
    Bass = 4,
    Catfish = 5,
    Snapper = 6,
    Cod = 7,
    Flounder = 8, 
    Tilapia = 9,
    Mahi_Mahi = 10,
    Swordfish = 11,
    Perch = 12,
    Carp = 13,
    Pike = 14,
    Haddock = 15, 
    Grouper = 16, 
    Redfish = 17,
    Halibut = 18,
    Snook = 19,
    Mackerel = 20
}