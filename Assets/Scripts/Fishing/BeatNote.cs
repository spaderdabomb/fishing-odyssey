using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatNote : MonoBehaviour
{
    FishingLine fishingLine = null;

    [SerializeField] Material material;

    private BeatSequence beatSequence;
    private float currentPositionPercentage = 0f;

    public void OnInstantiate(FishingLine newFishingLine, BeatSequence newBeatSequence)
    {
        fishingLine = newFishingLine;
        transform.position = newFishingLine.lineRenderer.GetPosition(1);
        material.color = newBeatSequence.beatColor;
        beatSequence = newBeatSequence;
    }

    void Update()
    {
        if (fishingLine == null)
            return;

        Vector3 lineRendererVector = fishingLine.lineRenderer.GetPosition(0) - fishingLine.lineRenderer.GetPosition(1);
        currentPositionPercentage += Time.deltaTime * 20f * beatSequence.beatSpeed;
        currentPositionPercentage = Mathf.Clamp(currentPositionPercentage, 0f, 100f);

        transform.position = fishingLine.lineRenderer.GetPosition(1) + lineRendererVector * currentPositionPercentage / 100f;
    }
}
