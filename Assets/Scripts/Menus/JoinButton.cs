using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runs an instance of a join button on the title menu
/// </summary>
public class JoinButton : MonoBehaviour
{
    public Sprite joinedSprite, readySprite;
    public AudioSource joinSound, readySound;

    public float effectSize = 2, bumpSize = 1.2f, sizeLerp = .1f;
    
    public bool ready => _ready || index < 0;
    bool _ready = false;

    Image sprite;
    float size = 1;
    int index = -1;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<Image>();
    }

    private void Update()
    {
        if (!_ready && index >= 0 && InputProxy.GetToggledInput(index))
            Ready();
    }

    private void FixedUpdate()
    {
        sprite.transform.localScale = Vector3.one * size;
        size = Mathf.Lerp(size, .99f, sizeLerp);

        if(size < 1)
        {
            size = bumpSize;
        }
    }

    public void Join(int playerIndex)
    {
        index = playerIndex;
        sprite.sprite = joinedSprite;
        size = effectSize;
        joinSound.Play();
    }

    public void Ready()
    {
        sprite.sprite = readySprite;
        size = effectSize;
        readySound.Play();
        _ready = true;
    }
}
