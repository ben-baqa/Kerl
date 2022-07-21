using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LoopAlternateTrack : MonoBehaviour
{
    public AudioClip subsequentLoops;

    AudioSource source;
    float clipLength;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.loop = true;
        clipLength = source.clip.length;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= clipLength)
        {
            source.clip = subsequentLoops;
            source.loop = true;
            if (!source.isPlaying)
                source.Play();
            Destroy(this);
        }
    }
}
