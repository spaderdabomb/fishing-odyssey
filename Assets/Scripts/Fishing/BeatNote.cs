using ES3Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatNote : MonoBehaviour
{
    FishingLine fishingLine = null;

    [SerializeField] float scaleStartRatio = 1f;
    [SerializeField] float scaleEndRatio = 0.1f;
    [SerializeField] float emissionStrength = 4f;
    [SerializeField] Material material;
    [SerializeField] ParticleSystem glowParticles;

    private BeatSequence beatSequence;
    private float currentPositionPercentage = 0f;

    private float startScale;

    private void Start()
    {
        startScale = transform.localScale.x;
    }

    public void OnInstantiate(FishingLine newFishingLine, BeatSequence newBeatSequence)
    {
        fishingLine = newFishingLine;
        transform.position = newFishingLine.lineRenderer.GetPosition(1);
        beatSequence = newBeatSequence;

        ParticleSystemRenderer particleRenderer = glowParticles.gameObject.GetComponent<ParticleSystemRenderer>();
        Material particleMaterial = particleRenderer.material;
        particleMaterial.SetColor("_EmissionColor", newBeatSequence.beatColor);

        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_BeatRingColor", newBeatSequence.beatColor);
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }

    void Update()
    {
        if (fishingLine == null)
            return;

        Vector3 lineRendererVector = fishingLine.lineRenderer.GetPosition(0) - fishingLine.lineRenderer.GetPosition(1);
        currentPositionPercentage += Time.deltaTime * 20f * beatSequence.beatSpeed;
        currentPositionPercentage = Mathf.Clamp(currentPositionPercentage, 0f, 100f);

        transform.position = fishingLine.lineRenderer.GetPosition(1) + lineRendererVector * currentPositionPercentage / 100f;
        float newScale = startScale * (scaleStartRatio - (scaleStartRatio - scaleEndRatio) * currentPositionPercentage / 100f);
        transform.localScale = new Vector3(newScale, newScale, newScale);
        transform.LookAt(fishingLine.lineRenderer.GetPosition(0));
        
        glowParticles.transform.localScale = new Vector3(newScale, newScale, newScale) / startScale;
    }

    private void OnDestroy()
    {
        GameObject beatNoteExplosion = Instantiate(BeatSequencer.Instance.beatNoteExplosionPrefab, BeatSequencer.Instance.beatNoteContainer.transform);
        beatNoteExplosion.transform.position = transform.position;

        ParticleSystemRenderer particleRenderer = beatNoteExplosion.GetComponent<ParticleSystemRenderer>();
        Material particleMaterial = particleRenderer.material;
        particleMaterial.SetColor("_EmissionColor", beatSequence.beatColor * emissionStrength);

        ParticleSystem ps = beatNoteExplosion.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new Color(beatSequence.beatColor.r, beatSequence.beatColor.g, beatSequence.beatColor.b, 1f);
    }
}
