using JSAM;
using PickleMan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<FishData> allFishData;
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
        gameData.InitDefaults();
        playerData.InitDefaults();
        playerStates = player.GetComponent<PlayerStates>();
        playerMovement = player.GetComponent<PlayerMovement>();

        foreach (FishData fishData in allFishData)
        {
            fishData.InitDefaults();
        }

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
        SpawnRandomFish();
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
            print(timeSinceBobHitWater);
            fishHooked = true;
            CaughtFish();
        }
    }

    public void BobHitWater(GameObject spawnedBob)
    {
        print("bob hit water");

        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.Fishing;
        bobInWater = true;
    }

    public void CaughtFish()
    {
        GameEventsManager.Instance.miscEvents.FishCollected();
        FishData caughtFishData = SpawnRandomFish();
        print(caughtFishData);
        gameData.LastFishCaught = caughtFishData;
        bobInWater = false;
        fishHooked = false;
        timeSinceBobHitWater = 0f;
        DestroyCurrentBob();
        playerStates.CurrentFishingState = PlayerStates.PlayerFishingState.None;
    }

    public FishData SpawnRandomFish()
    {
        float totalFishWeight = 0f;
        float[] weightListThresholds = new float[allFishData.Count];

        int i = 0;
        foreach (FishData fishData in allFishData)
        {
            totalFishWeight += fishData.catchWeightChance;
            float fishWeight = totalFishWeight;
            weightListThresholds[i] = fishWeight;
            i += 1;
        }

        float randomFishWieght = Random.Range(0f, totalFishWeight);
        int index = FindClosestIndex(randomFishWieght, weightListThresholds);
        FishData randomFishData = allFishData[index];

        return randomFishData;
    }

    private int FindClosestIndex(float targetValue, float[] array)
    {
        float closestDifference = Mathf.Abs(targetValue - array[0]);
        int closestIndex = 0;

        for (int i = 1; i < array.Length; i++)
        {
            float difference = Mathf.Abs(targetValue - array[i]);
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
