using Ink.Parsed;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // Save data values
    [HideInInspector] public float MasterVolume = 1f;
    [HideInInspector] public float MusicVolume = 1f;
    [HideInInspector] public float SfxVolume = 1f;

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

    public T Load<T>(string saveKeyName, T saveKey)
    {
        return ES3.Load<T>(saveKeyName, defaultValue: saveKey);
    }

    private void InitData()
    {
        inventoryItemData = new BaseItemData[GameManager.Instance.gameData.inventoryRows*GameManager.Instance.gameData.inventoryCols];
        handsContainerItemData = new BaseItemData[GameManager.Instance.gameData.gearCols];
        tackleContainerItemData = new BaseItemData[GameManager.Instance.gameData.gearCols];
        outfitContainerItemData = new BaseItemData[GameManager.Instance.gameData.gearCols];
        accessoriesContainerItemData = new BaseItemData[GameManager.Instance.gameData.gearCols];
    }

    private void LoadAllData()
    {
        IPersistable[] persistables = FindObjectsOfType<MonoBehaviour>().OfType<IPersistable>().ToArray();
        foreach (var persistable in persistables)
        {
            persistable.LoadData();
            print($"persistable load: {persistable}");
        }
    }

    public void Save<T>(string saveKeyName, T saveKeyValue)
    {
        ES3.Save(saveKeyName, saveKeyValue);
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
        Save(nameof(inventoryItemData), inventoryItemData);
    }

    private void OnApplicationQuit()
    {
        // SaveAllData();
    }

    public void SaveGearContainerData(GearContainerType gearContainerType, BaseItemData[] newItemData)
    {
        switch (gearContainerType)
        {
            case GearContainerType.Hands:
                handsContainerItemData = newItemData;
                Save(nameof(handsContainerItemData), handsContainerItemData); 
                break;
            case GearContainerType.Tackle: 
                tackleContainerItemData = newItemData;
                Save(nameof(tackleContainerItemData), tackleContainerItemData); 
                break;
            case GearContainerType.Outfit: 
                outfitContainerItemData = newItemData;
                Save(nameof(outfitContainerItemData), outfitContainerItemData);
                break;
            case GearContainerType.Accessories: 
                accessoriesContainerItemData = newItemData;
                Save(nameof(accessoriesContainerItemData), accessoriesContainerItemData); 
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
}

public interface IPersistable
{
    public void SaveData();
    public void LoadData();
}