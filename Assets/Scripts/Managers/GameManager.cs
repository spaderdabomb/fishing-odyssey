using JSAM;
using PickleMan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;
    public GameObject camera;
    [HideInInspector] public PlayerStates playerStates;
    [HideInInspector] public PlayerMovement playerMovement;

    public GameData gameData;
    public PlayerData playerData;
    public List<FishingRodData> allFishingRodData;
    public List<GameObject> biomeLocations;

    public GameObject bobContainer;
    public GameObject itemContainer;

    public GameObject fishingRodBob;
    public GameObject newFishParticlePrefab;

    private GameObject currentBob;
    private float timeSinceBobHitWater = 0f;
    public bool bobInWater = false;
    public bool fishHooked = false;
    [HideInInspector] public FishData lastFishCaught = null;

    public ItemData testItemData;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found more than one {this} in the scene.");
        }
        Instance = this;
    }

    private void OnEnable()
    {
        playerData.InitDefaults();
        playerStates = player.GetComponent<PlayerStates>();
        playerMovement = player.GetComponent<PlayerMovement>();

        foreach (FishingRodData fishingRodData in allFishingRodData)
        {
            fishingRodData.InitDefaults();
        }
    }

    private void OnDisable()
    {
        
    }

    void Start()
    {
        UIGameManager.Instance.SetCursorStateVisible(false);
        AudioManager.PlayMusic(MainAudioLibraryMusic.Ghostrifter);
    }

    void Update()
    {
        // SpawnRandomFish();
    }

    private void FixedUpdate()
    {
        FixedUpdateFishing();
    }

    void FixedUpdateFishing()
    {
        if (bobInWater && !fishHooked)
        {
            timeSinceBobHitWater += Time.deltaTime;
            DidFishSpawn();
        }
    }

    private void DidFishSpawn()
    {
        float baseSpawnChance = gameData.baseSpawnChance * Time.fixedDeltaTime;
        float rodSpawnChance = playerData.currentFishingRod.fishingRodStats.hookChance * Time.fixedDeltaTime;
        float waitTimeSpawnChance = timeSinceBobHitWater * Time.fixedDeltaTime * 0.1f;
        float spawnChance = baseSpawnChance + rodSpawnChance + waitTimeSpawnChance;
        float randomHookChance = Random.Range(0f, 1f);

        // Spawn fish
        if (randomHookChance < spawnChance)
        {
            // print(timeSinceBobHitWater);
            fishHooked = true;
            CaughtFish();
        }
    }

    public void BobHitWater(GameObject spawnedBob)
    {
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.Fishing;
        bobInWater = true;
    }

    public void CaughtFish()
    {
        FishData caughtFishData = SpawnRandomFish();
        GameEventsManager.Instance.miscEvents.FishCaught(caughtFishData);
        lastFishCaught = caughtFishData;
        bobInWater = false;
        fishHooked = false;
        timeSinceBobHitWater = 0f;
        DestroyCurrentBob();
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.None;
    }

    public FishData SpawnRandomFish()
    {
        float totalFishWeight = 0f;
        List<float> weightListThresholds = new();

        Dictionary<string, FishData> fishDataDict = FishDataExtensions.GetAllData();
        foreach (var fishDataKVP in fishDataDict)
        {
            FishData fishData = fishDataKVP.Value;
            totalFishWeight += fishData.catchWeightChance;
            float fishWeight = totalFishWeight;
            weightListThresholds.Add(fishWeight);
        }

        float randomFishWieght = Random.Range(0f, totalFishWeight);
        int index = FindClosestIndex(randomFishWieght, weightListThresholds);

        ObjectRarity[] enumValues = (ObjectRarity[])Enum.GetValues(typeof(ObjectRarity));
        int randomRarity = Random.Range(0, enumValues.Length);
        ObjectRarity randomRarityEnum = enumValues[randomRarity];

        FishData randomFishData = fishDataDict.Values.ToList()[index];
        FishData newFishData = randomFishData.CreateNew(randomRarityEnum);
        newFishData.weight = Random.Range(newFishData.weightRangeLow, newFishData.weightRangeHigh);

        return newFishData;
    }

    private int FindClosestIndex(float targetValue, List<float> weightList)
    {
        float closestDifference = Mathf.Abs(targetValue - weightList[0]);
        int closestIndex = 0;

        for (int i = 1; i < weightList.Count; i++)
        {
            float difference = Mathf.Abs(targetValue - weightList[i]);
            if (difference < closestDifference)
            {
                closestDifference = difference;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public void DestroyCurrentBob()
    {
        fishHooked = false;
        bobInWater = false;
        Destroy(currentBob);
        ResetBobTime();
    }

    public void ResetBobTime()
    {
        timeSinceBobHitWater = 0f;
    }

    public void SetCurrentBob(GameObject newBob)
    {
        currentBob = newBob;
    }

    public void UsePortal(int biomeNum)
    {
        player.transform.position = biomeLocations[biomeNum].transform.position;
    }
}
