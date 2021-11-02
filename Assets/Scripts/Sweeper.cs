using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweeper : MonoBehaviour
{
    public Vector3 offsetFromRock, resultPosition;

    [Header("Sweeping Physics")]
    public float frictionMultipler = 1;
    public float decay = .01f, sweepValue = .1f;

    [Header("body movment")]
    public float followLerp;
    public float lerpLerp, normalLerp, rotLerp,
        broomSoundDelay, brushRot, startRot, resultRot;


    public AudioSource broomSfx;
    public AudioClip[] sweepSounds;

    private Animator anim;
    private AudioSource sfx;
    private SkinnedMeshRenderer rend;
    private TurnManager input;
    private CurlingBar barDisplay;
    private Rock rock;

    private Vector3 startPosition;

    private enum Follow {rock, result, start }
    private Follow followState = Follow.start;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        input = FindObjectOfType<TurnManager>();

        barDisplay = FindObjectOfType<CurlingBar>();

        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frictionMultipler += decay / 50;
        bool sliding = followState == Follow.rock;
        barDisplay.gameObject.SetActive(sliding);
        anim.SetBool("sliding", sliding);

        switch (followState)
        {
            case Follow.rock:
                transform.position = Vector3.Lerp(transform.position,
                    rock.transform.position + offsetFromRock, followLerp);
                followLerp = Mathf.Lerp(followLerp, 1, lerpLerp);
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
                    Vector3.up * brushRot, followLerp);
                PredictLand();
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
            frictionMultipler -= sweepValue;
            StartCoroutine(Brush());
        }
    }

    private IEnumerator Brush()
    {
        yield return new WaitForSeconds(broomSoundDelay);
        broomSfx.clip = sweepSounds[Random.Range(0, sweepSounds.Length)];
        broomSfx.Play();
    }

    public void OnThrow(Rock r)
    {
        rock = r;
        followState = Follow.rock;
        followLerp = 0;
        frictionMultipler = 1;
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

        Material[] ar = rend.materials;
        ar[1] = mat;
        ar[3] = mat;
        rend.materials = ar;
    }

    // predict landing zone, 1 = 82, .75 = 75, 0 = 54
    public void PredictLand()
    {
        Rigidbody rb = rock.GetComponent<Rigidbody>();

        rock.frictionMultiplier = frictionMultipler;

        Vector3 pos = rb.position;
        Vector3 vel = rb.velocity;
        float radVel = rb.angularVelocity.y,
            friction = rock.friction * frictionMultipler;

        int safetyCount = 0;

        while(vel.magnitude > rock.stopThreshold)
        {
            pos += vel / 50;
            vel += Vector3.right * radVel * rock.spinForce / 50;

            vel *= (1 - friction);
            radVel -= radVel * rb.angularDrag;

            if(vel.magnitude < rock.slowDownThreshold)
            {
                vel = Vector3.Lerp(vel, Vector3.zero, rock.slowDownLerp);
            }

            if (safetyCount++ > 1000)
            {
                print("Yipes! " + safetyCount + " was not enough!");
                break;
            }
        }

        //string s = "Long: " + pos.z + ", Calc: ";
        //pos.z = (rb.velocity.z * (1 - friction) / -50) / Mathf.Log(1 - friction);
        //print(s + pos.z);

        float v = Mathf.Clamp01((pos.z - 54) / 28);
        barDisplay.progress = v;
    }
}
