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

    // Player
    public GameObject player;
    public GameObject playerCamera;
    [HideInInspector] public PlayerStates playerStates;
    [HideInInspector] public PlayerMovement playerMovement;

    // Data
    public GameData gameData;
    public PlayerData playerData;
    public List<FishingRodData> allFishingRodData;
    public List<GameObject> biomeLocations;

    // Containers
    public GameObject bobContainer;
    public GameObject bobStartLocation;
    public GameObject itemContainer;
    public FishingLine fishingLine;
    public GameObject fishingEffectsContainer;

    // Prefabs
    public GameObject fishingRodBob;
    public GameObject rippleParticleSystemPrefab;
    public GameObject fishSplashNormal;

    // Public properties
    public bool BobInWater { get; private set; } = false;
    public bool FishIsHooked { get; private set; } = false;
    public bool FishIsSpawned { get; private set; } = false;
    public FishData CurrentFishData { get; private set; } = null;

    // Member fields
    private GameObject currentBob;
    private float timeSinceBobHitWater = 0f;
    private FishData lastFishCaught = null;

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

        GameEventsManager.Instance.fishingEvents.onCastRod += CastRod;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing += StoppedFishing;
        GameEventsManager.Instance.fishingEvents.onFishSpawned += FishSpawned;
        GameEventsManager.Instance.fishingEvents.onFishHooked += FishHooked;
        GameEventsManager.Instance.fishingEvents.onFishCaught += FishCaught;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.fishingEvents.onCastRod -= CastRod;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing -= StoppedFishing;
        GameEventsManager.Instance.fishingEvents.onFishHooked -= FishHooked;
        GameEventsManager.Instance.fishingEvents.onFishCaught -= FishCaught;
        GameEventsManager.Instance.fishingEvents.onFishSpawned -= FishSpawned;
    }

    void Start()
    {
        UIGameManager.Instance.SetCursorStateVisible(false);
        AudioManager.PlayMusic(MainAudioLibraryMusic.Ghostrifter);
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        FixedUpdateFishing();
    }

    void FixedUpdateFishing()
    {
        // Need to use fixed time because it rolls a chance of catching every 1/60 seconds
        if (BobInWater && !FishIsSpawned)
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
            FishData spawnedFishData = SpawnRandomFish();
            GameEventsManager.Instance.fishingEvents.FishSpawned(spawnedFishData, currentBob);
        }
    }

    private void FishSpawned(FishData fishData, GameObject spawnedBob)
    {
        FishIsSpawned = true;
        CurrentFishData = fishData;

        GameObject splashParticles = Instantiate(fishSplashNormal, fishingEffectsContainer.transform);
        FishSplash fishSplash = splashParticles.GetComponent<FishSplash>();
        fishSplash.OnInstantiate(fishData, spawnedBob);
    }

    public void BobHitWater(GameObject spawnedBob, GameObject water)
    {
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.Fishing;
        BobInWater = true;

        GameObject rippleParticles = Instantiate(rippleParticleSystemPrefab, fishingEffectsContainer.transform);
        rippleParticles.transform.position = spawnedBob.transform.position;

        AudioManager.PlaySound(MainAudioLibrarySounds.BobSplash);
    }

    public void FishHooked(FishData fishData)
    {
        FishIsHooked = true;
        AudioManager.PlaySound(MainAudioLibrarySounds.FishHooked);
    }

    public void FishCaught(FishData fishData)
    {
        FishData caughtFishData = SpawnRandomFish();
        lastFishCaught = caughtFishData;
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

    private void ClearFishingParameters()
    {
        FishIsHooked = false;
        BobInWater = false;
        FishIsSpawned = false;
        CurrentFishData = null;
        timeSinceBobHitWater = 0f;
        fishingLine.lineRenderer.enabled = false;

        Destroy(currentBob);
    }

    public void CastRod(GameObject newBob)
    {
        ClearFishingParameters();

        fishingLine.lineRenderer.enabled = true;
        currentBob = newBob;
    }

    public void StoppedFishing()
    {
        ClearFishingParameters();
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.None;
    }

    public void UsePortal(int biomeNum)
    {
        player.transform.position = biomeLocations[biomeNum].transform.position;
    }
}
