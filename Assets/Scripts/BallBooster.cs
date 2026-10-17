using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallBooster : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float nudgePower = 0.8f;

    private Rigidbody rb;
    private Vector3 startPos;
    private float stuckTime = 0f;
    private float lastSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    void Update()
    {
        // Only allow manual ball reset during active play (not during goal celebrations or after match end)
        if (rb.isKinematic) return;

        /* --- MANUAL RESET DISABLED FOR NOW ---
        bool resetPressed = false;

        // Check Keyboard 'R'
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            resetPressed = true;
        }

        // Check Gamepad 'Y' (buttonNorth) across all connected controllers
        if (!resetPressed)
        {
            foreach (var pad in Gamepad.all)
            {
                if (pad.buttonNorth.wasPressedThisFrame)
                {
                    resetPressed = true;
                    break;
                }
            }
        }

        if (resetPressed)
        {
            ResetBall();
        }
        ---------------------------------------- */
    }

    void FixedUpdate()
    {
        // Pause physics logic while the ball is frozen during a goal celebration
        if (rb.isKinematic) return;

        // Auto-reset if the ball flies off the table
        if (Vector3.Distance(transform.position, startPos) > 3.5f || transform.position.y < startPos.y - 0.5f)
        {
            ResetBall();
            return;
        }

        Vector3 v = rb.linearVelocity;

        // Keep the ball flat on the table surface
        v.y = Mathf.Clamp(v.y, -1f, 0.5f);

        // HARD CAP: Absolutely prevent speed from exceeding maxSpeed
        if (v.magnitude > maxSpeed)
        {
            v = v.normalized * maxSpeed;
        }

        rb.linearVelocity = v;
        lastSpeed = v.magnitude;

        // Auto-nudge ONLY if completely stopped in open space for 2.5 seconds
        if (rb.linearVelocity.magnitude < 0.03f)
        {
            stuckTime += Time.fixedDeltaTime;
            if (stuckTime > 2.5f)
            {
                Vector3 gentlePush = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                rb.AddForce(gentlePush * nudgePower, ForceMode.VelocityChange);
                stuckTime = 0f;
            }
        }
        else
        {
            stuckTime = 0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb.isKinematic) return;

        stuckTime = 0f;

        float currentSpeed = rb.linearVelocity.magnitude;

        // Anti-Shoot Guard: If it hit a static wall (not a player) and suddenly gained speed out of nowhere, clamp it back
        if (collision.gameObject.GetComponentInParent<FoosballRod>() == null && currentSpeed > lastSpeed * 1.15f && lastSpeed > 0.05f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * lastSpeed;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        stuckTime = 0f;
    }

    public void ResetBall()
    {
        stuckTime = 0f;
        lastSpeed = 0f;
        transform.position = startPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}