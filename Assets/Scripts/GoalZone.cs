using System.Collections;
using UnityEngine;
using TMPro;

public class GoalZone : MonoBehaviour
{
    [Header("Goal Setup")]
    public bool isRedGoalHole = true;    // TRUE = Red's goal hole (Blue scores here), FALSE = Blue's goal hole (Red scores here)

    [Header("UI & Light References")]
    public TMP_Text scoreText;           // Top scoreboard text ("BLUE 0 - 0 RED")
    public TMP_Text goalMessageText;     // Center popup text ("Blue scored" / "Red scored")
    public Light goalLight;              // Point Light inside THIS goal pocket

    [Header("Settings")]
    public float celebrationTime = 2.0f; // Duration (in seconds) before resetting light, hiding message, and respawning ball

    private static int blueScore = 0;
    private static int redScore = 0;
    private bool isCelebrating = false;

    void Start()
    {
        blueScore = 0;
        redScore = 0;
        UpdateUI();

        // Keep goal light white by default
        if (goalLight != null)
            goalLight.color = Color.white;

        // Hide the goal message at the start of the match
        if (goalMessageText != null)
            goalMessageText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCelebrating) return; // Prevent double-scoring while celebration is active

        BallBooster ball = other.GetComponent<BallBooster>();
        if (ball != null)
        {
            StartCoroutine(GoalSequence(ball));
        }
    }

    IEnumerator GoalSequence(BallBooster ball)
    {
        isCelebrating = true;

        // 1. Freeze and hide the ball inside the goal
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        MeshRenderer mr = ball.GetComponent<MeshRenderer>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        if (mr != null) mr.enabled = false;

        // 2. Update score, glow the goal light, and display the message
        if (isRedGoalHole)
        {
            // Ball entered Red's goal -> Blue scored!
            blueScore++;
            if (goalLight != null)
                goalLight.color = Color.blue; // Red bar's light glows Blue

            if (goalMessageText != null)
            {
                goalMessageText.text = "Blue scored";
                goalMessageText.color = new Color(0.2f, 0.6f, 1f); // Bright Blue text
                goalMessageText.gameObject.SetActive(true);
            }
        }
        else
        {
            // Ball entered Blue's goal -> Red scored!
            redScore++;
            if (goalLight != null)
                goalLight.color = Color.red; // Blue bar's light glows Red

            if (goalMessageText != null)
            {
                goalMessageText.text = "Red scored";
                goalMessageText.color = new Color(1f, 0.25f, 0.25f); // Bright Red text
                goalMessageText.gameObject.SetActive(true);
            }
        }

        UpdateUI();

        // 3. Hold the message and colored light for 1-2 seconds
        yield return new WaitForSeconds(celebrationTime);

        // 4. Return light to white and hide the goal message
        if (goalLight != null)
            goalLight.color = Color.white;

        if (goalMessageText != null)
            goalMessageText.gameObject.SetActive(false);

        // 5. Unhide and respawn the ball at the center of the table
        if (mr != null) mr.enabled = true;
        if (rb != null) rb.isKinematic = false;
        ball.ResetBall();

        isCelebrating = false;
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"BLUE {blueScore}  -  {redScore} RED";
        }
    }
}