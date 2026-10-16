using UnityEngine;
using UnityEngine.InputSystem;

public class BallBooster : MonoBehaviour
{
    public float kickPower = 1.5f;
    public float maxSpeed = 5f;
    public float nudgePower = 0.8f;

    // Works with Keyboard R, Gamepad Y/Triangle, and Generic USB Controller Trigger/Buttons!
    public InputAction resetAction = new InputAction("Reset", binding: "<Keyboard>/r");

    private Rigidbody rb;
    private Vector3 startPos;
    private float stuckTime;

    void OnEnable()
    {
        // Add Gamepad and Generic USB Joystick bindings automatically if not set in Inspector
        if (resetAction.bindings.Count <= 1)
        {
            resetAction.AddBinding("<Gamepad>/buttonNorth"); // Y / Triangle
            resetAction.AddBinding("<Joystick>/trigger");    // Primary button on generic USB controllers
            resetAction.AddBinding("<HID>/button2");         // Secondary button on generic USB controllers
        }
        resetAction.Enable();
    }

    void OnDisable()
    {
        resetAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    void Update()
    {
        // Single check for Keyboard + Gamepad + USB Joystick!
        if (resetAction.WasPressedThisFrame())
        {
            ResetBall();
        }
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, startPos) > 3.5f || transform.position.y < startPos.y - 0.5f)
        {
            ResetBall();
            return;
        }

        Vector3 v = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
        v.y = Mathf.Min(v.y, 2f);
        rb.linearVelocity = v;

        stuckTime = rb.linearVelocity.magnitude < 0.05f ? stuckTime + Time.fixedDeltaTime : 0f;
        if (stuckTime > 1.5f)
        {
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * nudgePower, ForceMode.Impulse);
            stuckTime = 0f;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        FoosballRod rod = col.gameObject.GetComponentInParent<FoosballRod>();
        if (rod != null)
        {
            // Only boost when the rod is actively tilted/kicking
            float currentAngle = Mathf.Abs(Mathf.DeltaAngle(0f, rod.transform.localEulerAngles.x)) +
                                 Mathf.Abs(Mathf.DeltaAngle(0f, rod.transform.localEulerAngles.z));

            if (currentAngle > 8f)
            {
                Vector3 dir = col.contacts[0].normal;
                dir.y = Mathf.Clamp(dir.y, 0f, 0.15f);
                rb.AddForce(dir.normalized * kickPower, ForceMode.Impulse);
            }
        }
        else
        {
            // Hit a wall or table edge: prevent sudden physics speed spikes
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed * 0.7f);
        }
    }

    public void ResetBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPos;

        Vector3 serveDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        rb.AddForce(serveDir * nudgePower, ForceMode.Impulse);
    }
}