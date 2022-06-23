using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the playback of primary audio effects during a round
/// </summary>
public class AudioEffects : MonoBehaviour
{
    [Header("Turn Sounds")]
    public AudioClip blueTurnSound;
    public AudioClip redTurnSound;

    [Header("Action Sounds")]
    public AudioClip rockSelectionSound;
    public AudioClip targetSelectionSound;
    public AudioClip throwSound;
    public AudioClip brushSound;

    [Header("Character Noises")]
    public AudioClip[] throwCharacterSounds;
    public AudioClip[] brushCharacterSounds;
    public float brushSoundSelay;

    public AudioSourceSettings settings;

    List<AudioSource> sources = new List<AudioSource>();

    private void Update()
    {
        List<AudioSource> toDestroy = new List<AudioSource>();

        foreach (AudioSource s in sources)
            if (!s.isPlaying)
                toDestroy.Add(s);

        foreach (AudioSource s in toDestroy)
        {
            Destroy(s);
            sources.Remove(s);
        }
    }

    public void OnTurnStart(bool blueTurn) => PlayClip(blueTurn ? blueTurnSound : redTurnSound);
    public void OnRockSelection() => PlayClip(rockSelectionSound);
    public void OnTargetSelection() => PlayClip(targetSelectionSound);
    public void OnThrow()
    {
        PlayClip(throwSound);
        PlayRandom(throwCharacterSounds);
    }
    public void OnBrush()
    {
        PlayClip(brushSound);
        PlayRandomWithDelay(brushCharacterSounds, brushSoundSelay);
    }

    public void PlayClip(AudioClip clip, float volumeMultiplier = 1)
    {
        if (!clip)
            return;

        AudioSource s = gameObject.AddComponent<AudioSource>();
        ApplyParameters(s);
        s.volume *= volumeMultiplier;

        s.clip = clip;
        s.Play();
        sources.Add(s);
    }

    public void PlayClipWithDelay(AudioClip clip, float delay, float volumeMultiplier = 1)
    {
        StartCoroutine(PlayWithDelay(clip, delay, volumeMultiplier));
    }

    IEnumerator PlayWithDelay(AudioClip clip, float delay, float volumeMultiplier)
    {
        yield return new WaitForSecondsRealtime(delay);
        PlayClip(clip, volumeMultiplier);
    }

    public void PlayRandom(IReadOnlyList<AudioClip> collection, float volumeMultiplier = 1)
    {
        int n = collection.Count;
        if (n == 0)
            return;
        PlayClip(collection[Random.Range(0, n)]);
    }

    public void PlayRandomWithDelay(IReadOnlyList<AudioClip> collection, float delay, float volumeMultiplier = 1)
    {
        int n = collection.Count;
        if (n == 0)
            return;
        PlayClipWithDelay(collection[Random.Range(0, n)], delay, volumeMultiplier);
    }

    void ApplyParameters(AudioSource s)
    {
        s.volume = settings.volume;
        s.pitch = settings.pitch;
        s.dopplerLevel = settings.doppler;
        s.minDistance = settings.minDist;
        s.maxDistance = settings.maxDist;
        s.spatialBlend = settings.spatialBlend;
        s.rolloffMode = settings.rollMode;
    }
}

[System.Serializable]
public struct AudioSourceSettings
{
    [Range(0, 1)]
    public float spatialBlend, volume;
    public AudioRolloffMode rollMode;
    public float pitch, doppler, spread, minDist, maxDist;

    public void Apply(AudioSourceSettings s)
    {
        float vol = volume;
        this = s;
        volume = vol;
    }
}
