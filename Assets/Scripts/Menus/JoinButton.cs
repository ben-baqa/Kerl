using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runs an instance of a join button on the title menu
/// </summary>
public class JoinButton : MonoBehaviour
{
    public Sprite joined, ready;

    public float effectSize = 2, bumpSize = 1.2f, sizeLerp = .1f;

    private Image rend;
    private AudioSource[] sfx;
    private float size = 1;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Image>();
        sfx = GetComponentsInChildren<AudioSource>();
    }

    private void FixedUpdate()
    {
        rend.transform.localScale = Vector3.one * size;
        size = Mathf.Lerp(size, .99f, sizeLerp);

        if(size < 1)
        {
            size = bumpSize;
        }
    }

    public void Join()
    {
        rend.sprite = joined;
        size = effectSize;
        sfx[0].Play();
    }

    public void Ready()
    {
        rend.sprite = ready;
        size = effectSize;
        sfx[1].Play();
    }
}
