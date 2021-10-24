using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweeper : MonoBehaviour
{
    public Vector3 offsetFromRock, resultPosition;

    public float followLerp, lerpLerp, normalLerp, rotLerp,
        broomSoundDelay, brushRot, startRot, resultRot;

    public AudioSource broomSfx;

    private Animator anim;
    private AudioSource sfx;
    private SkinnedMeshRenderer[] rend;
    private TurnManager input;
    private Transform rock;

    private Vector3 startPosition;

    private enum Follow {rock, result, start }
    private Follow followState = Follow.start;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
        rend = GetComponentsInChildren<SkinnedMeshRenderer>();
        input = FindObjectOfType<TurnManager>();

        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (followState)
        {
            case Follow.rock:
                transform.position = Vector3.Lerp(transform.position,
                    rock.position + offsetFromRock, followLerp);
                followLerp = Mathf.Lerp(followLerp, 1, lerpLerp);
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
                    Vector3.up * brushRot, followLerp);
                break;
            case Follow.result:
                transform.position = Vector3.Lerp(transform.position,
                    resultPosition, followLerp);
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
                    Vector3.up * resultRot, rotLerp);
                break;
        }
        if (input.GetInput() && followLerp > .3f &&
            anim.GetCurrentAnimatorStateInfo(0).IsName("slide") &&
            followState == Follow.rock)
        {
            sfx.Play();
            anim.SetTrigger("sweep");
            StartCoroutine(Brush());
        }
    }

    private IEnumerator Brush()
    {
        yield return new WaitForSeconds(broomSoundDelay);
        broomSfx.Play();
    }

    public void OnThrow(Transform t)
    {
        rock = t;
        followState = Follow.rock;
        followLerp = 0;
    }

    public void OnResult()
    {
        followState = Follow.result;
        followLerp = normalLerp;
    }

    public void OnTurnStart(Material mat)
    {
        followState = Follow.start;
        transform.position = startPosition;
        transform.eulerAngles = Vector3.up * startRot;
        Material[] ar = rend[0].materials;
        ar[1] = mat;
        rend[0].materials = ar;
        rend[1].material = mat;
    }
}
