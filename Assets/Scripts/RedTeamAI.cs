using UnityEngine;
using System.Collections.Generic;

public class RedTeamAI : MonoBehaviour
{
    [Header("Assign References")]
    public FoosballRod[] redRods;
    public Transform ball;
    private Rigidbody ballRb;

    [Header("Movement Settings")]
    public float slideSpeed = 2.0f;       // Increased from 0.6 for snappier tracking
    public float predictionTime = 0.08f;  // How far ahead the AI predicts ball movement
    public float deadzone = 0.015f;       // Stops rods from jittering when aligned

    [Header("Kick & Rotation Settings")]
    public float maxAngle = 80f;
    public float snapSpeed = 30f;
    public float kickRange = 0.16f;       // Z-distance to trigger a snap kick
    public float windUpRange = 0.35f;     // Z-distance to pull foot back in preparation
    public float kickDirection = -1f;     // Set to -1 or 1 depending on which way kicks forward

    // Stores the child player transforms for each rod so the AI knows where every foot is
    private List<Transform>[] rodPlayers;
    private float[] kickTimers;

    void Start()
    {
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody>();
        }

        if (redRods != null)
        {
            rodPlayers = new List<Transform>[redRods.Length];
            kickTimers = new float[redRods.Length];

            for (int i = 0; i < redRods.Length; i++)
            {
                rodPlayers[i] = new List<Transform>();
                if (redRods[i] == null) continue;

                // Find all player figures attached to this rod via their Colliders
                Collider[] colliders = redRods[i].GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    if (col.transform != redRods[i].transform)
                    {
                        rodPlayers[i].Add(col.transform);
                    }
                }

                // Fallback: If no colliders are found on children yet, grab direct children
                if (rodPlayers[i].Count == 0)
                {
                    foreach (Transform child in redRods[i].transform)
                    {
                        rodPlayers[i].Add(child);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (ball == null || redRods == null || redRods.Length == 0) return;

        // Predict future ball position based on current velocity
        Vector3 predictedBallPos = ball.position;
        if (ballRb != null)
        {
            predictedBallPos += ballRb.linearVelocity * predictionTime;
        }

        for (int i = 0; i < redRods.Length; i++)
        {
            if (redRods[i] == null) continue;

            // --- 1. SLIDE LOGIC (Find the closest player figure on this rod to the ball) ---
            float targetX = predictedBallPos.x;
            float closestPlayerX = redRods[i].transform.position.x;

            if (rodPlayers != null && rodPlayers[i].Count > 0)
            {
                float minDist = float.MaxValue;
                foreach (Transform player in rodPlayers[i])
                {
                    if (player == null) continue;
                    float dist = Mathf.Abs(targetX - player.position.x);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestPlayerX = player.position.x;
                    }
                }
            }

            float xDiff = targetX - closestPlayerX;
            float lsY = 0f;

            // Only slide if outside the deadzone to prevent micro-jittering
            if (Mathf.Abs(xDiff) > deadzone)
            {
                lsY = Mathf.Clamp(xDiff * 6f, -1f, 1f);
            }

            // --- 2. ROTATION / KICK LOGIC ---
            float zDiff = ball.position.z - redRods[i].transform.position.z;
            float zDist = Mathf.Abs(zDiff);
            float xDistToPlayer = Mathf.Abs(ball.position.x - closestPlayerX);

            float rsX = 0f;

            // If the ball is close on the Z-axis AND lined up with a player foot on the X-axis
            if (zDist < kickRange && xDistToPlayer < 0.22f)
            {
                // Snap kick repeat loop (swings forward hard, resets briefly if ball gets stuck)
                kickTimers[i] += Time.deltaTime * 16f;
                rsX = Mathf.Sin(kickTimers[i]) > -0.2f ? kickDirection : -kickDirection * 0.6f;
            }
            else if (zDist < windUpRange)
            {
                // Ball is approaching: pull foot back slightly for a stronger hit
                kickTimers[i] = 0f;
                rsX = -kickDirection * 0.45f;
            }
            else
            {
                kickTimers[i] = 0f;
                rsX = 0f; // Rest in neutral vertical stance
            }

            // Send final calculated inputs to your FoosballRod script
            redRods[i].MoveAndRotate(lsY, rsX, slideSpeed, maxAngle, snapSpeed);
        }
    }
}