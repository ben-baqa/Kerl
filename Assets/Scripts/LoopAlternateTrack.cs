using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopAlternateTrack : MonoBehaviour
{
    public AudioClip firstLoop;
    public AudioClip subsequentLoops;
    public float volume = 0.6f;

    AudioSource firstSource;
    AudioSource secondSource;

    void Start()
    {
        firstSource = gameObject.AddComponent<AudioSource>();
        firstSource.playOnAwake = false;
        firstSource.clip = firstLoop;
        firstSource.volume = volume;
        firstSource.loop = false;
        firstSource.Play();

        secondSource = gameObject.AddComponent<AudioSource>();
        secondSource.playOnAwake = false;
        secondSource.clip = subsequentLoops;
        secondSource.volume = volume;
        secondSource.loop = true;
        secondSource.PlayScheduled(AudioSettings.dspTime + firstLoop.length);
    }

    public void Stop()
    {
        firstSource.Stop();
        secondSource.Stop();
    }
}
