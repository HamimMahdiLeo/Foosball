using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GoalZone : MonoBehaviour
{
    [Header("Goal Setup")]
    public bool isRedGoalHole = true;    // TRUE = Red's goal hole (Blue scores), FALSE = Blue's goal hole (Red scores)

    [Header("UI & Light References")]
    public TMP_Text scoreText;           // Top scoreboard text ("BLUE 0 - 0 RED")
    public TMP_Text timerText;           // Countdown timer text ("02:00")
    public TMP_Text goalMessageText;     // Center popup text ("Blue scored" / "Red scored" / Winner)
    public Light goalLight;              // Point Light inside THIS goal pocket

    [Header("Settings")]
    public float celebrationTime = 2.0f; // Duration before respawning ball
    public float matchDuration = 120f;   // Match time in seconds (120 = 2 minutes)

    private static int blueScore = 0;
    private static int redScore = 0;
    private static float timeRemaining = 120f;
    private static bool isMatchOver = false;
    private static bool isCelebrating = false;

    void Start()
    {
        // Only initialize the match state once from the Red Goal trigger
        if (isRedGoalHole)
        {
            blueScore = 0;
            redScore = 0;
            timeRemaining = matchDuration;
            isMatchOver = false;
            isCelebrating = false;
            UpdateUI();
            UpdateTimerUI();
        }

        if (goalLight != null)
            goalLight.color = Color.white;

        if (goalMessageText != null)
            goalMessageText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Let only ONE trigger drive the timer and match-end input
        if (!isRedGoalHole) return;

        // When the match is over, press Keyboard R or Gamepad Y to restart the entire match
        if (isMatchOver)
        {
            bool restartPressed = (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame);

            if (!restartPressed)
            {
                foreach (var pad in Gamepad.all)
                {
                    if (pad.buttonNorth.wasPressedThisFrame)
                    {
                        restartPressed = true;
                        break;
                    }
                }
            }

            if (restartPressed)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }

        // Count down when not celebrating a goal
        if (!isCelebrating && timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                EndMatch();
            }

            UpdateTimerUI();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCelebrating || isMatchOver) return;

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
            blueScore++;
            if (goalLight != null)
                goalLight.color = Color.blue;

            if (goalMessageText != null)
            {
                goalMessageText.text = "Blue scored";
                goalMessageText.color = new Color(0.2f, 0.6f, 1f);
                goalMessageText.gameObject.SetActive(true);
            }
        }
        else
        {
            redScore++;
            if (goalLight != null)
                goalLight.color = Color.red;

            if (goalMessageText != null)
            {
                goalMessageText.text = "Red scored";
                goalMessageText.color = new Color(1f, 0.25f, 0.25f);
                goalMessageText.gameObject.SetActive(true);
            }
        }

        UpdateUI();

        // 3. Hold the message and colored light
        yield return new WaitForSeconds(celebrationTime);

        // 4. Return light to white and hide the goal message (if match hasn't ended)
        if (goalLight != null)
            goalLight.color = Color.white;

        if (!isMatchOver)
        {
            if (goalMessageText != null)
                goalMessageText.gameObject.SetActive(false);

            // 5. Unhide and respawn the ball at the center
            if (mr != null) mr.enabled = true;
            if (rb != null) rb.isKinematic = false;
            ball.ResetBall();
        }

        isCelebrating = false;
    }

    void EndMatch()
    {
        isMatchOver = true;

        // Freeze the ball in place when the final whistle blows
        BallBooster ball = FindAnyObjectByType<BallBooster>();
        if (ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // Show winner message
        if (goalMessageText != null)
        {
            goalMessageText.gameObject.SetActive(true);

            if (blueScore > redScore)
            {
                goalMessageText.text = "TIME'S UP!\nBLUE WINS!\n<size=40%>Press R or Y to Restart</size>";
                goalMessageText.color = new Color(0.2f, 0.6f, 1f);
            }
            else if (redScore > blueScore)
            {
                goalMessageText.text = "TIME'S UP!\nRED WINS!\n<size=40%>Press R or Y to Restart</size>";
                goalMessageText.color = new Color(1f, 0.25f, 0.25f);
            }
            else
            {
                goalMessageText.text = "TIME'S UP!\nDRAW MATCH!\n<size=40%>Press R or Y to Restart</size>";
                goalMessageText.color = Color.yellow;
            }
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"BLUE {blueScore}  -  {redScore} RED";
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Turn timer red in the final 10 seconds!
            timerText.color = (timeRemaining <= 10f) ? Color.red : Color.white;
        }
    }
}