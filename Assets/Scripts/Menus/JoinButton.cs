using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Runs an instance of a join button on the title menu
/// </summary>
public class JoinButton : MonoBehaviour
{
    [Header("Sounds")]
    public AudioSource joinSound;
    public AudioSource readySound;

    [Header("Colours")]
    public Color joinedColour;
    public Color readyColour;

    [Space(30)]
    public float joinedSize = 0.75f;
    public float effectSize = 2;
    public float bumpFactor = 1.2f;
    public float sizeLerp = .1f;
    
    public bool ready => _ready || index < 0;
    bool _ready = false;

    Image image;
    TextMeshProUGUI text;
    float size = 1, targetSize = 1;
    int index = -1;

    private void Update()
    {
        if (!_ready && index >= 0 && InputProxy.GetToggledInput(index))
            Ready();
    }

    private void FixedUpdate()
    {
        image.transform.localScale = Vector3.one * size;
        size = Mathf.Lerp(size, targetSize - .01f, sizeLerp);

        if(size < targetSize)
            size = bumpFactor * targetSize;
    }

    public void Join(int playerIndex, Sprite sprite)
    {
        index = playerIndex;
        image.sprite = sprite;
        image.color = joinedColour;
        size = effectSize;
        targetSize = joinedSize;
        joinSound.Play();
    }
    public void Join(int playerIndex, Sprite sprite, string keyCode)
    {
        Join(playerIndex, sprite);
        text.text = keyCode;
    }

    public void Ready()
    {
        image.color = readyColour;
        size = effectSize;
        readySound.Play();
        _ready = true;
    }

    public void Reset(Sprite sprite)
    {
        image = GetComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;

        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = "";

        size = 1;
        targetSize = 1;
        index = -1;
        _ready = false;
    }
}
