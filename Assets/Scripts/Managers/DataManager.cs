using Ink.Parsed;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using static UnityEngine.Mesh;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // Save data values
    [HideInInspector] public float MasterVolume { get; set; } = 1f;
    [HideInInspector] public float MusicVolume { get; set; } = 1f;
    [HideInInspector] public float SfxVolume { get; set; } = 1f;

    [HideInInspector] public int TotalFishCaught { get; set; } = 0;
    [HideInInspector] public Dictionary<string, bool> FishCaughtDict { get; set; } = new Dictionary<string, bool>();
    [HideInInspector] public Dictionary<string, DateTime> FishDateCaughtDict { get; set; } = new Dictionary<string, DateTime>();
    [HideInInspector] public Dictionary<string, int> FishTotalCaughtDict { get; set; } = new Dictionary<string, int>();
    [HideInInspector] public Dictionary<string, float> FishCaughtBestWeightDict { get; set; } = new Dictionary<string, float>();

    [HideInInspector] public BaseItemData[] inventoryItemData;
    [HideInInspector] public BaseItemData[] handsContainerItemData;
    [HideInInspector] public BaseItemData[] tackleContainerItemData;
    [HideInInspector] public BaseItemData[] outfitContainerItemData;
    [HideInInspector] public BaseItemData[] accessoriesContainerItemData;

    public UnityAction<float> OnSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitData();
            LoadAllData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Events

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        GameEventsManager.Instance.miscEvents.onFishCaught += FishCaught;
        GameEventsManager.Instance.miscEvents.onNewFishCaught += NewFishCaught;
    }

    private void UnregisterEvents()
    {
        GameEventsManager.Instance.miscEvents.onFishCaught -= FishCaught;
        GameEventsManager.Instance.miscEvents.onNewFishCaught -= NewFishCaught;
    }

    private void FishCaught(FishData fishData)
    {
        TotalFishCaught += 1;
        IncrementOrCreateKey(FishTotalCaughtDict, fishData.fishID);
    }

    private void NewFishCaught(FishData fishData)
    {
        FishCaughtDict.TryAdd(fishData.fishID, true);
        FishDateCaughtDict.TryAdd(fishData.fishID, DateTime.Now);
        UpdateIfBiggerOrCreateKeyWithFloatValue(FishCaughtBestWeightDict, fishData.fishID, fishData.weight);
        print($"new fish caught: {fishData.fishID}");
    }

    #endregion

    private void InitData()
    {
        inventoryItemData = new BaseItemData[GameManager.Instance.gameData.inventoryRows * GameManager.Instance.gameData.inventoryCols];
        handsContainerItemData = new BaseItemData[GameManager.Instance.gameData.gearCols];
        tackleContainerItemData = new BaseItemData[GameManager.Instance.gameData.gearCols];
        outfitContainerItemData = new BaseItemData[GameManager.Instance.gameData.gearCols];
        accessoriesContainerItemData = new BaseItemData[GameManager.Instance.gameData.gearCols];
    }

    public T Load<T>(string saveKeyName, T saveKey)
    {
        return ES3.Load<T>(saveKeyName, defaultValue: saveKey);
    }

    private void LoadAllData()
    {
        IPersistable[] persistables = FindObjectsOfType<MonoBehaviour>().OfType<IPersistable>().ToArray();
        foreach (var persistable in persistables)
        {
            persistable.LoadData();
            print($"persistable load: {persistable}");
        }

        TotalFishCaught = Load(nameof(TotalFishCaught), TotalFishCaught);
        FishCaughtDict = Load(nameof(FishCaughtDict), FishCaughtDict);
        FishDateCaughtDict = Load(nameof(FishDateCaughtDict), FishDateCaughtDict);
        FishTotalCaughtDict = Load(nameof(FishTotalCaughtDict), FishTotalCaughtDict);
        FishCaughtBestWeightDict = Load(nameof(FishCaughtBestWeightDict), FishCaughtBestWeightDict);
    }

    public void SaveAllData()
    {
        IPersistable[] persistables = FindObjectsOfType<MonoBehaviour>().OfType<IPersistable>().ToArray();
        foreach (var persistable in persistables)
        {
            persistable.SaveData();
            print($"persistable save: {persistable}");
        }

        // All gear containers
        GearContainerType[] gearContainerTypes = (GearContainerType[])Enum.GetValues(typeof(GearContainerType));
        foreach (GearContainerType gearContainerType in gearContainerTypes)
        {
            BaseItemData[] tempItemData = InventoryManager.Instance.GetGearSlotData(gearContainerType);
            SaveGearContainerData(gearContainerType, tempItemData);
        }

        // Inventory
        inventoryItemData = InventoryManager.Instance.GetAllInventorySlotData();
        ES3.Save(nameof(inventoryItemData), inventoryItemData);

        // DataManager values
        ES3.Save(nameof(TotalFishCaught), TotalFishCaught);
        ES3.Save(nameof(FishCaughtDict), FishCaughtDict);
        ES3.Save(nameof(FishDateCaughtDict), FishDateCaughtDict);
        ES3.Save(nameof(FishTotalCaughtDict), FishTotalCaughtDict);
        ES3.Save(nameof(FishCaughtBestWeightDict), FishCaughtBestWeightDict);

    }

    private void OnApplicationQuit()
    {
        SaveAllData();
    }

    public void SaveGearContainerData(GearContainerType gearContainerType, BaseItemData[] newItemData)
    {
        switch (gearContainerType)
        {
            case GearContainerType.Hands:
                handsContainerItemData = newItemData;
                ES3.Save(nameof(handsContainerItemData), handsContainerItemData); 
                break;
            case GearContainerType.Tackle: 
                tackleContainerItemData = newItemData;
                ES3.Save(nameof(tackleContainerItemData), tackleContainerItemData); 
                break;
            case GearContainerType.Outfit: 
                outfitContainerItemData = newItemData;
                ES3.Save(nameof(outfitContainerItemData), outfitContainerItemData);
                break;
            case GearContainerType.Accessories: 
                accessoriesContainerItemData = newItemData;
                ES3.Save(nameof(accessoriesContainerItemData), accessoriesContainerItemData); 
                break;
            default:
                Debug.Log($"failed to save gearContainerType {gearContainerType}");
                break;
        }
    }

    public BaseItemData[] LoadGearContainerData(GearContainerType gearContainerType)
    {
        switch (gearContainerType)
        {
            case GearContainerType.Hands:
                handsContainerItemData = Load(nameof(handsContainerItemData), handsContainerItemData);
                return handsContainerItemData;
            case GearContainerType.Tackle:
                tackleContainerItemData = Load(nameof(tackleContainerItemData), tackleContainerItemData);
                return tackleContainerItemData;
            case GearContainerType.Outfit:
                outfitContainerItemData = Load(nameof(outfitContainerItemData), outfitContainerItemData);
                return outfitContainerItemData;
            case GearContainerType.Accessories:
                accessoriesContainerItemData = Load(nameof(accessoriesContainerItemData), accessoriesContainerItemData);
                return accessoriesContainerItemData;
            default:
                Debug.Log($"failed to load gearContainerType {gearContainerType}");
                return null;
        }
    }

    #region Utility Methods for saved values

    public int GetTotalFishCollected()
    {
        return FishCaughtDict.Count;
    }

    public bool IsFishCaught(string fishID)
    {
        if (FishCaughtDict.TryGetValue(fishID, out bool fishCaught))
        {
            return fishCaught;
        }
        else
        {
            return false;
        }
    }

    public bool IsFishTypeCaught(FishData fishData)
    {
        string fishName = fishData.fishName;
        foreach (ObjectRarity objectRarity in Enum.GetValues(typeof(ObjectRarity)))
        {
            string currentFishID = fishName + objectRarity.ToString();
            if (FishCaughtDict.TryGetValue(currentFishID, out bool fishCaught))
            {
                return fishCaught;
            }
        }

        return false;
    }

    public int GetFishCollectedInBiome(BiomeType biomeType)
    {
        int fishCollectedInBiome = 0;
        List<FishData> biomeFishData = BiomeExtensions.GetAllFishInBiomes()[biomeType];
        foreach (FishData biomeFish in  biomeFishData)
        {
            if (FishCaughtDict.TryGetValue(biomeFish.fishID, out bool fishCaught))
            {
                if (fishCaught)
                {
                    fishCollectedInBiome++;
                }
            }
        }

        return fishCollectedInBiome;
    }

    public static void IncrementOrCreateKey(Dictionary<string, int> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key]++;
        }
        else
        {
            dictionary[key] = 1;
        }
    }

    static void UpdateIfBiggerOrCreateKeyWithFloatValue(Dictionary<string, float> dictionary, string key, float newValue)
    {
        if (dictionary.ContainsKey(key))
        {
            if (newValue > dictionary[key])
            {
                dictionary[key] = newValue;
            }
        }
        else
        {
            dictionary[key] = newValue;
        }
    }

    #endregion
}

public interface IPersistable
{
    public void SaveData();
    public void LoadData();
}
