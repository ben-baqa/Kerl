using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runs an instance of a join button on the title menu
/// </summary>
public class JoinButton : MonoBehaviour
{
    public Sprite inactiveSprite, joinedSprite, readySprite;
    public AudioSource joinSound, readySound;

    public float effectSize = 2, bumpSize = 1.2f, sizeLerp = .1f;
    
    public bool ready => _ready || index < 0;
    bool _ready = false;

    Image image;
    float size = 1;
    int index = -1;

    private void Update()
    {
        if (!_ready && index >= 0 && InputProxy.GetToggledInput(index))
            Ready();
    }

    private void FixedUpdate()
    {
        image.transform.localScale = Vector3.one * size;
        size = Mathf.Lerp(size, .99f, sizeLerp);

        if(size < 1)
        {
            size = bumpSize;
        }
    }

    public void Join(int playerIndex)
    {
        index = playerIndex;
        image.sprite = joinedSprite;
        size = effectSize;
        joinSound.Play();
    }

    public void Ready()
    {
        image.sprite = readySprite;
        size = effectSize;
        readySound.Play();
        _ready = true;
    }

    public void Reset()
    {
        image = GetComponent<Image>();
        image.sprite = inactiveSprite;
        size = 1;
        index = -1;
        _ready = false;
    }
}
