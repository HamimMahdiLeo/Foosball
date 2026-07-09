/*
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    [Header("Balanced Formation")]
    public Transform balanced;

    [Header("Player")]
    public GameObject bluePlayerPrefab;

    [Header("Player Transform")]
    public Vector3 playerPosition = new Vector3(0.3921f, 1.2261f, -0.6120f);
    public Vector3 playerRotation = new Vector3(0f, -180f, 0f);
    public Vector3 playerScale = new Vector3(0.027f, 0.027f, 0.027f);

    [ContextMenu("Generate Blue Team")]
    public void GenerateBlueTeam()
    {
        foreach (Transform point in balanced)
        {
            GameObject player = Instantiate(bluePlayerPrefab);

            player.transform.position = point.position + playerPosition;
            player.transform.rotation = Quaternion.Euler(playerRotation);
            player.transform.localScale = playerScale;
        }
    }
}
*/