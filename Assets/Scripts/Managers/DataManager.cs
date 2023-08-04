using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEngine;
using static UnityEngine.Mesh;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // Persistent data keys
    public static string masterVolume = "masterVolume";
    public static string musicVolume = "musicVolume";
    public static string sfxVolume = "sfxVolume";

    // Persistent data values
    public Dictionary<string, bool> fishCollectionDict = new();

    // Default values
    public static float masterVolumeDefault = 1f;
    public static float musicVolumeDefault = 1f;
    public static float sfxVolumeDefault = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitAllData();
            SetConfig();
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitAllData()
    {
        for (int i = 0; i < GameManager.Instance.allFishData.Count; i++)
        {
            fishCollectionDict.Add(GameManager.Instance.allFishData[i].fishID, false);
        }
    }

    private void SetConfig()
    {
        // build your config
        var config = new FBPPConfig()
        {
            SaveFileName = "URP-template-name.txt",
            AutoSaveData = false,
            ScrambleSaveData = false,
            // EncryptionSecret = "spadersecretcode",
            // SaveFilePath = "C:/Repositories/unity-2d-builtin-template"
        };
        // pass it to FBPP
        FBPP.Start(config);
    }

    private void LoadData()
    {
        for (int i = 0; i < DataManager.Instance.fishCollectionDict.Count; i++)
        {
            FishData fishData = GameManager.Instance.allFishData[i];
            GetSavedFishBool(fishData);
        }
    }

    public void SaveData()
    {
        FBPP.Save();

        for (int i = 0; i < DataManager.Instance.fishCollectionDict.Count; i++)
        {
            FishData fishData = GameManager.Instance.allFishData[i];
            SetFishBool(fishData, DataManager.Instance.fishCollectionDict[fishData.fishID]);
        }
    }
    public void SetFishBool(FishData newFishData, bool value)
    {
        FBPP.SetBool(newFishData.fishID, value);
        DataManager.Instance.fishCollectionDict[newFishData.fishID] = value;
    }

    public bool GetFishBool(FishData newFishData)
    {
        return DataManager.Instance.fishCollectionDict[newFishData.fishID];
    }

    private void GetSavedFishBool(FishData newFishData)
    {
        DataManager.Instance.fishCollectionDict[newFishData.fishID] = FBPP.GetBool(newFishData.fishID, false);
    }
}
