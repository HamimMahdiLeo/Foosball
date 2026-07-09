using UnityEngine;

public class RedTeamSpawner : MonoBehaviour
{
    [Header("Red Player Prefab")]
    public GameObject redPlayerPrefab;

    [Header("Spawn Points (2-5-3)")]
    public Transform[] spawnPoints;

    [Header("Player Transform")]
    public Vector3 positionOffset;
    public Vector3 rotation;
    public Vector3 scale = new Vector3(0.027f, 0.027f, 0.027f);

    [ContextMenu("Spawn Red Team")]
    public void SpawnTeam()
    {
        foreach (Transform point in spawnPoints)
        {
            GameObject player = Instantiate(redPlayerPrefab);

            player.transform.position = point.position + positionOffset;
            player.transform.rotation = Quaternion.Euler(rotation);
            player.transform.localScale = scale;
        }
    }
}