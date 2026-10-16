using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public string scoringTeam = "Blue"; // Type "Blue" or "Red" in the Inspector
    public Vector3 ballRespawnPos;      // Leaves at (0,0,0) to use ball's start position

    private Vector3 initialBallPos;
    private bool hasSavedStartPos = false;
    private int score = 0;

    void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the goal has the BallBooster script
        if (other.GetComponent<BallBooster>() != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (!hasSavedStartPos)
            {
                initialBallPos = ballRespawnPos != Vector3.zero ? ballRespawnPos : new Vector3(0f, other.transform.position.y + 0.2f, 0f);
                hasSavedStartPos = true;
            }

            score++;
            Debug.Log(scoringTeam + " SCORED! Total " + scoringTeam + " Goals: " + score);

            // Reset ball to center
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            other.transform.position = initialBallPos;
        }
    }
}