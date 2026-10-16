using UnityEngine;

public class BallBooster : MonoBehaviour
{
    public float kickPower = 1.5f;
    public float maxSpeed = 5f;
    public float nudgePower = 0.8f;

    private Rigidbody rb;
    private Vector3 startPos;
    private float stuckTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        // Reset if the ball flies off the table
        if (Vector3.Distance(transform.position, startPos) > 3.5f || transform.position.y < startPos.y - 0.5f)
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = startPos;
            return;
        }

        // Cap max speed and upward chip height
        Vector3 v = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
        v.y = Mathf.Min(v.y, 2f);
        rb.linearVelocity = v;

        // Nudge if stuck for 1.5 seconds
        stuckTime = rb.linearVelocity.magnitude < 0.05f ? stuckTime + Time.fixedDeltaTime : 0f;
        if (stuckTime > 1.5f)
        {
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * nudgePower, ForceMode.Impulse);
            stuckTime = 0f;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponentInParent<FoosballRod>() != null)
        {
            Vector3 dir = col.contacts[0].normal;
            dir.y = Mathf.Clamp(dir.y, 0f, 0.15f); // Allows small chip shots
            rb.AddForce(dir.normalized * kickPower, ForceMode.Impulse);
        }
    }
}