using System;
using UnityEngine;

public class FishingEvents 
{ 
    public event Action<GameObject, GameObject> onBobHitWater;
    public void BobHitWater(GameObject currentBob, GameObject hitWater) => onBobHitWater?.Invoke(currentBob, hitWater);

    public event Action<FishData, GameObject> onFishSpawned;
    public void FishSpawned(FishData fishData, GameObject currentBob) => onFishSpawned?.Invoke(fishData, currentBob);

    public event Action<FishData> onFishHooked;
    public void FishHooked(FishData fishData) => onFishHooked?.Invoke(fishData);

    public event Action<FishData> onNewFishCaught;
    public void NewFishCaught(FishData fishData) => onNewFishCaught?.Invoke(fishData);

    public event Action<GameObject> onCastRod;
    public void CastRod(GameObject fishingBob) => onCastRod?.Invoke(fishingBob);

    public event Action onStoppedFishing;
    public void StoppedFishing()
    {
        onStoppedFishing?.Invoke();
        GameEventsManager.Instance.destroyOnStoppedFishing.Raise();
    }

    public event Action<bool> onBeatNoteSubmitted;
    public void BeatNoteSubmitted(bool result) => onBeatNoteSubmitted?.Invoke(result);

    public event Action<FishData> onFishCaught;
    public void FishCaught(FishData fishData)
    {
        onFishCaught?.Invoke(fishData);
        StoppedFishing();
    }
}
