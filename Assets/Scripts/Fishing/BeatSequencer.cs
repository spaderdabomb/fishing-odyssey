using JSAM;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSequencer : MonoBehaviour
{
    public static BeatSequencer Instance;

    [Header("Game Scene")]
    public GameObject beatNotePrefab;
    public FishingLine fishingLine;

    [Header("Beat Properties")]
    public float baseSpeed = 1f;
    public float missTolerance = 3f;
    public float pitchShift = 0.05f;

    public BeatSequence BeatSequence { get; private set; }
    public int BeatIndex { get; private set; } = 0;
    public int BeatSpawnedIndex { get; private set; } = 0;

    private FishData currentFishData = null;
    private float timeBeforeNextBeat = 9999f;
    private bool beatSequenceStarted = false;
    private List<BeatNote> beatNotesActive = new List<BeatNote>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found more than one {this} in the scene.");
        }
        Instance = this;
    }

    private void Update()
    {
        if (!beatSequenceStarted)
            return;

        timeBeforeNextBeat -= Time.deltaTime;
        if (timeBeforeNextBeat < 0 && BeatSpawnedIndex < BeatSequence.numberBeats)
        {
            InstantiateBeatNote(BeatSequence);
        }
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

    public void InitBeatSequence(FishData fishData) 
    {
        // log distribution based on weight
        // weight = 0 --> 4 beats
        // weight = 100 --> 13 beats
        int numberBeats = Mathf.RoundToInt(Mathf.Log10(10f*(fishData.weight + 1f))) + 3;

        float beatSpeed = (float)fishData.fishRarity;
        float timeBetweenBeats = (float)fishData.fishRarity;
        Color beatColor = GameManager.Instance.gameData.rarityToColorDict[fishData.fishRarity];

        BeatSequence = new BeatSequence(numberBeats, timeBetweenBeats, beatSpeed, beatColor);
        InstantiateBeatNote(BeatSequence);

        currentFishData = fishData;
        beatSequenceStarted = true;
    }

    private void EndBeatSequence()
    {
        beatSequenceStarted = false;
        timeBeforeNextBeat = 9999f;
        BeatIndex = 0;
        BeatSpawnedIndex = 0;
        RemoveAllBeatNotes();
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
        GameObject beatNoteGO = Instantiate(beatNotePrefab, fishingLine.transform);
        BeatNote beatNote = beatNoteGO.GetComponent<BeatNote>();
        beatNote.OnInstantiate(fishingLine, beatSequence);

        beatNotesActive.Add(beatNote);

        BeatSpawnedIndex++;
        ResetTimeBeforeNextBeat();
    }

    private void ResetTimeBeforeNextBeat()
    {
        timeBeforeNextBeat = 1f / (BeatSequence.timeBetweenBeats * baseSpeed);
    }

    public void BeatNotePressed()
    {
        if (beatNotesActive.Count <= 0)
        {
            return;
        }

        float currentDistance = (beatNotesActive[0].transform.position - fishingLine.lineRenderer.GetPosition(0)).magnitude;
        GameEventsManager.Instance.fishingEvents.BeatNoteSubmitted(currentDistance < missTolerance);
    }

    public void BeatNoteSubmitted(bool result)
    {
        if (result)
        {
            GameObject firstBeatNoteGO = beatNotesActive[0].gameObject;
            beatNotesActive.RemoveAt(0);
            Destroy(firstBeatNoteGO);
            BeatIndex++;
            SoundFileObject sfo = AudioManager.GetSoundSafe(MainAudioLibrarySounds.BeatNoteGood);
            sfo.startingPitch += pitchShift;
            AudioManager.PlaySound(sfo);
        }

        if (BeatIndex >= BeatSequence.numberBeats)
        {
            GameEventsManager.Instance.fishingEvents.FishCaught(currentFishData);
            AudioManager.PlaySound(MainAudioLibrarySounds.BeatSequenceComplete);
        }
    }
}

public struct BeatSequence
{
    public int numberBeats;
    public float beatSpeed;
    public float timeBetweenBeats;
    public Color beatColor;

    public BeatSequence(int numberBeats, float timeBetweenBeats, float beatSpeed, Color beatColor)
    {
        this.numberBeats = numberBeats;
        this.timeBetweenBeats = timeBetweenBeats;
        this.beatSpeed = beatSpeed;
        this.beatColor = beatColor;
    }
}
