using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private GameObject currentBob = null;

    private void OnEnable()
    {
        GameEventsManager.Instance.fishingEvents.onCastRod += CastRod;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.fishingEvents.onCastRod -= CastRod;
    }

    private void Update()
    {
        UpdateFishingLineRenderer();
    }

    private void UpdateFishingLineRenderer()
    {
        if (currentBob == null)
            return;

        lineRenderer.SetPosition(0, GameManager.Instance.bobStartLocation.transform.position);
        lineRenderer.SetPosition(1, currentBob.transform.position);
    }

    private void CastRod(GameObject fishingBob)
    {
        currentBob = fishingBob;
    }
}
