using UnityEngine;

public class ClothesSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject[] clothingPrefabs;

    public float laneDistance = 2.5f;
    public float spacing = 8f;
    public float spawnAheadDistance = 100f;

    private float nextSpawnZ;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // ✅ start spawning ahead of player (not fixed 20)
        if (player != null)
            nextSpawnZ = player.position.z + 20f;
        else
            nextSpawnZ = 20f;
    }

    void Update()
    {
        if (player == null) return;

        while (nextSpawnZ < player.position.z + spawnAheadDistance)
        {
            SpawnRow();
            nextSpawnZ += spacing;
        }
    }

    void SpawnRow()
    {
        // ✅ safety check
        if (clothingPrefabs == null || clothingPrefabs.Length == 0)
        {
            Debug.LogWarning("No clothing prefabs assigned!");
            return;
        }

        // 🎯 random lane (like before)
        int lane = Random.Range(-1, 2);
        float x = lane * laneDistance;

        Vector3 pos = new Vector3(x, 1.1f, nextSpawnZ);

        GameObject randomClothing =
            clothingPrefabs[Random.Range(0, clothingPrefabs.Length)];

        Instantiate(randomClothing, pos, Quaternion.identity);
    }
}