using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [Header("Throwing")]
    public float throwForce = 9;
    public float spinForce = 1, spinPush = .1f,
        spinMomentum = .1f;
    [Space(10)]
    [Header("Movement")]
    public float friction;
    public float radialFriction, stopThreshold;

    private Rigidbody rb;
    private float spin = 0;
    private bool thrown;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(rb.angularVelocity.y * Vector3.right * spinPush);

        rb.velocity *= (1 - friction);
        rb.angularVelocity *= (1 - radialFriction);

        if (rb.velocity.magnitude < stopThreshold)
            rb.velocity = Vector3.zero;
    }

    public void Throw(float spin)
    {
        if (thrown)
            return;
        thrown = true;
        this.spin = spin;
        transform.Rotate(Vector3.up, -spin);
        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        rb.AddTorque(Vector3.up * spin * spinForce, ForceMode.Impulse);
    }
}
