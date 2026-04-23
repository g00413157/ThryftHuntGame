using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public Transform player;

    public float spawnDistance = 40f;
    public float spacing = 15f;
    public float laneDistance = 2.5f;

    public static int CurrentSafeLane;

    private float nextSpawnZ;

    void Start()
    {
        nextSpawnZ = spawnDistance;
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.z + spawnDistance >= nextSpawnZ)
        {
            SpawnRow();
            nextSpawnZ += spacing;
        }
    }

    void SpawnRow()
    {
        // 🎯 choose safe lane
        int safeLane = Random.Range(-1, 2);
        CurrentSafeLane = safeLane;

        // 🚧 choose ONE obstacle lane (not safe)
        int obstacleLane;
        do
        {
            obstacleLane = Random.Range(-1, 2);
        }
        while (obstacleLane == safeLane);

        float x = obstacleLane * laneDistance;

        Vector3 pos = new Vector3(x, 0.6f, nextSpawnZ);

        Instantiate(
            obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)],
            pos,
            Quaternion.identity
        );
    }
}