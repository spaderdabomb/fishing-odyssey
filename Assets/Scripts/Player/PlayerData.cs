using PickleMan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Fishing Odyssey/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] public float healthBase = 100f;
    [SerializeField] public float fishPowerBase = 100f;
    [SerializeField] public float fishPowerIncreaseRate = 50f;
    [SerializeField] public int fishingLevel = 1;

    [ReadOnly] public FishingRodData currentFishingRod;

    // Health
    [SerializeField, ReadOnly] private float healthCurrent = 100f;
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

    // Throw power
    [SerializeField, ReadOnly] private float fishPowerCurrent;
    public float FishPowerCurrent
    {
        get { return fishPowerCurrent; }
        set
        {
            if (fishPowerCurrent != value)
            {
                fishPowerCurrent = Mathf.Max(value, 0);
                fishPowerCurrent = Mathf.Min(fishPowerCurrent, fishPowerBase);
                FishPowerChanged?.Invoke(fishPowerCurrent);
            }
        }
    }

    public UnityAction<float> HealthChanged;
    public UnityAction<float> FishPowerChanged;

    public void InitDefaults()
    {
        healthCurrent = healthBase;
        fishPowerCurrent = 0f;
    }
}
