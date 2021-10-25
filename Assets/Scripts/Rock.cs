using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [Header("Throwing")]
    public float throwForce = 9;
    public float spinForce = 1, spinPush = .1f, spinWeightExponent = 3;
    [Space(10)]
    [Header("Movement")]
    public float friction;
    public float radialFriction, stopThreshold, slowDownThreshold,
        slowDownLerp, bounce = 1.5f, particleMultiplier;

    [Space(10)]
    public float resultViewThreshold = 75;

    [Header("Sounds")]
    public AudioClip[] sounds;
    public AudioSource slip;

    [HideInInspector]
    public Skipper skip;

    [HideInInspector]
    public float frictionMultiplier = 1;

    public bool blue;

    private Rigidbody rb;
    private AudioSource sfx, grindNoise;
    private ParticleSystem particles;

    private float spin = 0, particleCount;
    private bool thrown, turnEnded, resultsViewed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sfx = GetComponent<AudioSource>();
        particles = GetComponentInChildren<ParticleSystem>();
        grindNoise = particles.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(rb.angularVelocity.y * Vector3.right * spinPush);

        rb.velocity *= (1 - (friction * frictionMultiplier));
        rb.angularVelocity *= (1 - radialFriction);
        grindNoise.volume = Mathf.Clamp01(Mathf.Abs(rb.velocity.z) * 3);

        if(rb.velocity.magnitude < slowDownThreshold)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, slowDownLerp);
        }
        if (rb.velocity.magnitude < stopThreshold)
        {
            rb.velocity = Vector3.zero;
            if (!turnEnded && rb.position.z > 25)
            {
                turnEnded = true;
                skip.StartTurn();
            }
        }
        if(rb.position.y < -2 && ! turnEnded)
        {
            turnEnded = true;
            skip.StartTurn();
            slip.Play();
        }
        if (rb.position.y < -1000)
            Destroy(gameObject);
        if(!turnEnded && !resultsViewed && rb.position.z > resultViewThreshold)
        {
            resultsViewed = true;
            CameraPositions.OnResult();
            FindObjectOfType<Sweeper>().OnResult();
            FindObjectOfType<ScoreHUD>().OnResult();
        }

        particleCount += (rb.velocity.magnitude * particleMultiplier);
        if (particleCount > 1)
        {
            particles.Emit((int)particleCount);
            particleCount %= 1;
        }
    }

    public void Throw(float spin, float ratio = 1)
    {
        if (thrown)
            return;

        thrown = true;
        this.spin = spin;

        float rad = spin * Mathf.Deg2Rad;
        Vector3 dir = Vector3.forward * Mathf.Cos(rad) +
            Vector3.left * Mathf.Sin(rad);
        
        rb.AddForce(dir.normalized * throwForce, ForceMode.Impulse);

        float r = Mathf.Abs(Mathf.Pow(ratio, 3));
        rb.AddTorque(Vector3.up * spin * spinForce * r, ForceMode.Impulse);
    }

    public void Score(bool scoring)
    {
        transform.GetChild(1).gameObject.SetActive(scoring);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (sfx && !sfx.isPlaying)
        {
            sfx.volume = Mathf.Clamp01(rb.velocity.magnitude * 2);
            sfx.clip = sounds[Random.Range(0, sounds.Length)];
            sfx.Play();
        }
        //TODO make custom collission stuff
        if (collision.collider.CompareTag("Rock"))
            rb.velocity *= bounce;
    }
}
