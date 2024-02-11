using DamageNumbersPro;
using JSAM;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSequencer : SerializedMonoBehaviour
{
    public static BeatSequencer Instance;

    [Header("Game Scene")]
    public GameObject beatNotePrefab;
    public GameObject beatNoteContainer;
    public GameObject beatNoteExplosionPrefab;
    public GameObject beatRingIndicator;
    public GameObject playerCamera;
    public DamageNumber beatRingMiss;
    public DamageNumber beatRingGood;
    public DamageNumber beatRingPerfect;
    public FishingLine fishingLine;

    [Header("Beat Properties")]
    public Dictionary<BeatNoteRegion, int> beatNoteRegionsToSegmentsDict = new Dictionary<BeatNoteRegion, int>();

    [SerializeField] private float firstBeatDelay = 1f;
    [SerializeField] private float minimumTimeBetweenPoints = 0.5f;
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float timeBetweenPointsScaleFactor = 1f;
    [SerializeField] private float rarityDifficultyScaling = 5f;
    [SerializeField] private float missTolerance = 3f;
    [SerializeField] private float pitchShift = 0.05f;
    [SerializeField] private float perfectSizeFraction = 0.25f;

    [SerializeField] private float beatIndicatorIntensityGood = 2f;
    [SerializeField] private float beatIndicatorIntensityPerfect = 3f;

    public BeatSequence BeatSequence { get; private set; }
    public int CurrentSegment { get; private set; } = 0;
    public int BeatSpawnedIndex { get; private set; } = 0;
    public BeatNoteRegion currentBeatNoteRegion { get; private set; } = BeatNoteRegion.None;

    private FishData currentFishData = null;
    private float timeBeforeNextBeat = 9999f;
    private bool beatSequenceStarted = false;
    private List<BeatNote> beatNotesActive = new List<BeatNote>();
    private Vector3 beatIndicatorMeshSize = Vector3.zero;
    private Color beatIndicatorStartColor;
    private float currentTension;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found more than one {this} in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        beatRingIndicator.SetActive(false);
        MeshFilter meshFilter = beatRingIndicator.GetComponent<MeshFilter>();
        Bounds meshBounds = meshFilter.sharedMesh.bounds;
        beatIndicatorMeshSize = meshBounds.size;

        beatIndicatorStartColor = beatRingIndicator.GetComponent<MeshRenderer>().material.color;
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.fishingEvents.onFishHooked += InitBeatSequence;
        GameEventsManager.Instance.fishingEvents.onBeatNoteSubmitted += BeatNoteSubmitted;
        GameEventsManager.Instance.fishingEvents.onCastRod += CastRod;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing += StoppedFishing;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.fishingEvents.onFishHooked -= InitBeatSequence;
        GameEventsManager.Instance.fishingEvents.onBeatNoteSubmitted -= BeatNoteSubmitted;
        GameEventsManager.Instance.fishingEvents.onCastRod -= CastRod;
        GameEventsManager.Instance.fishingEvents.onStoppedFishing -= StoppedFishing;
    }

    private void Update()
    {
        if (!beatSequenceStarted)
            return;


        // Handle beat spawning
        timeBeforeNextBeat -= Time.deltaTime;
        if (timeBeforeNextBeat < 0 && CurrentSegment < BeatSequence.numberSegments)
        {
            InstantiateBeatNote(BeatSequence);
        }

        // Update UI if needed
        Vector3 bobScreenLocation = UIToolkitUtils.WorldSpaceToScreenSpace(GameManager.Instance.CurrentBob.transform.position);
        UIGameManager.Instance.uiGameScene.beatGameBar.UpdatePosition(bobScreenLocation);

        if (beatRingIndicator.activeSelf)
        {
            beatRingIndicator.transform.LookAt(fishingLine.lineRenderer.GetPosition(1));
            BeatNoteRegion beatNoteRegion = GetBeatNoteRegion();

            if (beatNoteRegion == BeatNoteRegion.Good)
            {
                beatRingIndicator.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", beatIndicatorStartColor * beatIndicatorIntensityGood);
            }
            else if (beatNoteRegion == BeatNoteRegion.Perfect)
            {
                beatRingIndicator.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", beatIndicatorStartColor * beatIndicatorIntensityPerfect);
            }
            else
            {
                beatRingIndicator.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", beatIndicatorStartColor);
            }
        }
    }

    public void InitBeatSequence(FishData fishData) 
    {
        // log distribution based on weight
        // weight = 0 --> 4 beats
        // weight = 100 --> 13 beats
        int numberSegments = Mathf.RoundToInt(Mathf.Log10(10f*(fishData.weight + 1f))) + 3;

        float beatSpeed = Mathf.Log10(10f*((float)fishData.fishRarity + rarityDifficultyScaling)) * baseSpeed;
        float timeBetweenBeats = minimumTimeBetweenPoints + 1 / (Mathf.Log10(10f * ((float)fishData.fishRarity) + rarityDifficultyScaling)) * timeBetweenPointsScaleFactor;
        Color beatColor = GameManager.Instance.gameData.rarityToSaturatedColorDict[fishData.fishRarity];

        BeatSequence = new BeatSequence(numberSegments, timeBetweenBeats, beatSpeed, beatColor);
        beatRingIndicator.SetActive(true);

        Vector3 bobScreenLocation = UIToolkitUtils.WorldSpaceToScreenSpace(GameManager.Instance.CurrentBob.transform.position);
        UIGameManager.Instance.uiGameScene.beatGameBar.ShowBeatGameBar(numberSegments, bobScreenLocation);

        timeBeforeNextBeat = firstBeatDelay;
        currentFishData = fishData;
        beatSequenceStarted = true;
    }

    public void BeatNotePressed()
    {
        if (beatNotesActive.Count <= 0)
        {
            return;
        }

        BeatNoteRegion beatNoteRegion = GetBeatNoteRegion();
        GameEventsManager.Instance.fishingEvents.BeatNoteSubmitted(beatNoteRegion);

        if (CurrentSegment >= BeatSequence.numberSegments)
        {
            GameEventsManager.Instance.fishingEvents.FishCaught(currentFishData);
            AudioManager.PlaySound(MainAudioLibrarySounds.BeatSequenceComplete);
        }
    }

    public void BeatNoteSubmitted(BeatNoteRegion beatNoteRegion)
    {
        GameObject firstBeatNoteGO = beatNotesActive[0].gameObject;

        if (beatNoteRegion == BeatNoteRegion.Good)
        {
            beatRingGood.Spawn(firstBeatNoteGO.transform.position + Vector3.up * 0.2f, playerCamera.transform);
        }
        else if (beatNoteRegion == BeatNoteRegion.Perfect)
        {
            beatRingPerfect.Spawn(firstBeatNoteGO.transform.position + Vector3.up * 0.2f, playerCamera.transform);
        }
        else if (beatNoteRegion == BeatNoteRegion.Miss)
        {
            beatRingMiss.Spawn(firstBeatNoteGO.transform.position + Vector3.up * 0.2f, playerCamera.transform);
            SoundFileObject tempsfo = AudioManager.GetSoundSafe(MainAudioLibrarySounds.BeatNoteGood);
            tempsfo.startingPitch = 1f;
            UIGameManager.Instance.uiGameScene.SetTensionBarValue(1f);
        }

        CurrentSegment += beatNoteRegionsToSegmentsDict[beatNoteRegion];

        beatNotesActive.RemoveAt(0);
        Destroy(firstBeatNoteGO);

        SoundFileObject sfo = AudioManager.GetSoundSafe(MainAudioLibrarySounds.BeatNoteGood);
        sfo.startingPitch += pitchShift;
        AudioManager.PlaySound(sfo);
        AudioManager.PlaySound(MainAudioLibrarySounds.ReelFishShort);
    }

    private void EndBeatSequence()
    {
        beatSequenceStarted = false;
        timeBeforeNextBeat = 9999f;
        CurrentSegment = 0;
        BeatSpawnedIndex = 0;

        beatRingIndicator.SetActive(false);
        RemoveAllBeatNotes();

        UIGameManager.Instance.uiGameScene.beatGameBar.HideBeatGameBar();

        SoundFileObject sfo = AudioManager.GetSoundSafe(MainAudioLibrarySounds.BeatNoteGood);
        sfo.startingPitch = 1f;
        currentFishData = null;
    }

    private void RemoveAllBeatNotes()
    {
        foreach (BeatNote beatNote in beatNotesActive)
        {
            Destroy(beatNote.gameObject);
        }

        beatNotesActive.Clear();
    }

    private void CastRod(GameObject fishingBob)
    {
        EndBeatSequence();
    }

    private void StoppedFishing()
    {
        EndBeatSequence();
    }

    private void InstantiateBeatNote(BeatSequence beatSequence)
    {
        GameObject beatNoteGO = Instantiate(beatNotePrefab, beatNoteContainer.transform);
        BeatNote beatNote = beatNoteGO.GetComponent<BeatNote>();
        beatNote.OnInstantiate(fishingLine, beatSequence);

        beatNotesActive.Add(beatNote);

        BeatSpawnedIndex++;
        ResetTimeBeforeNextBeat();
    }

    private void ResetTimeBeforeNextBeat()
    {
        timeBeforeNextBeat = BeatSequence.timeBetweenBeats;
    }

    public BeatNoteRegion GetBeatNoteRegion()
    {
        if (beatNotesActive.Count == 0)
            return BeatNoteRegion.None;

        BeatNoteRegion beatNoteRegion;
        Vector3 beatNotePosition = beatNotesActive[0].transform.position;
        Vector3 beatIndicatorCenter = beatRingIndicator.transform.position + beatRingIndicator.transform.forward * beatIndicatorMeshSize.z * beatRingIndicator.transform.localScale.z;

        if (Vector3.Distance(beatNotePosition, beatIndicatorCenter) < beatIndicatorMeshSize.z * perfectSizeFraction * beatRingIndicator.transform.localScale.z)
        {
            beatNoteRegion = BeatNoteRegion.Perfect;
        }
        else if (Vector3.Distance(beatNotePosition, beatIndicatorCenter) < beatIndicatorMeshSize.z * beatRingIndicator.transform.localScale.z)
        {
            beatNoteRegion = BeatNoteRegion.Good;
        }
        else
        {
            beatNoteRegion = BeatNoteRegion.Miss;
        }

        return beatNoteRegion;
    }
}

public struct BeatSequence
{
    public int numberSegments;
    public float beatSpeed;
    public float timeBetweenBeats;
    public Color beatColor;

    public BeatSequence(int numberSegments, float timeBetweenBeats, float beatSpeed, Color beatColor)
    {
        this.numberSegments = numberSegments;
        this.timeBetweenBeats = timeBetweenBeats;
        this.beatSpeed = beatSpeed;
        this.beatColor = beatColor;
    }
}

public enum BeatNoteRegion
{
    None = 0,
    Miss = 1,
    Good = 2,
    Perfect = 3
}
