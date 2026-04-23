using System.Collections.Generic;
using UnityEngine;

public class StreetGenerator : MonoBehaviour
{
    public Transform player;
    public int initialSegments = 18;
    public float spacing = 6f;
    public float spawnAheadDistance = 120f;
    public float despawnBehindDistance = 30f;
    public float roadWidth = 7.5f;
    public float sidewalkWidth = 2.2f;

    private readonly Queue<GameObject> spawnedSegments = new Queue<GameObject>();
    private float nextSegmentZ;
    private GameObject fallbackGround;
    private Shader runtimeShader;

    void Start()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        runtimeShader = FindRuntimeShader();

        if (player == null)
        {
            GameObject taggedPlayer = GameObject.FindGameObjectWithTag("Player");
            if (taggedPlayer != null)
                player = taggedPlayer.transform;
        }

        ClearExistingChildren();
        CreateSkyBackdrop();
        CreateFallbackGround();

        nextSegmentZ = 0f;

        for (int i = 0; i < initialSegments; i++)
            SpawnSegment();

        if (player != null)
        {
            Vector3 pos = player.position;
            pos.y = 1.2f;
            pos.x = 0f;
            pos.z = 0f;
            player.position = pos;
        }
    }

    void Update()
    {
        if (player == null) return;

        while (nextSegmentZ < player.position.z + spawnAheadDistance)
            SpawnSegment();

        while (spawnedSegments.Count > 0)
        {
            GameObject oldest = spawnedSegments.Peek();

            if (oldest == null)
            {
                spawnedSegments.Dequeue();
                continue;
            }

            if (oldest.transform.position.z >= player.position.z - despawnBehindDistance)
                break;

            spawnedSegments.Dequeue();
            Destroy(oldest);
        }
    }

    void SpawnSegment()
    {
        GameObject segmentRoot = new GameObject("StreetSegment_" + nextSegmentZ.ToString("F0"));
        segmentRoot.transform.SetParent(transform, false);
        segmentRoot.transform.position = new Vector3(0f, 0f, nextSegmentZ);

        CreateRoad(segmentRoot.transform);
        CreateLaneStripes(segmentRoot.transform);
        CreateStreetProps(segmentRoot.transform);

        // ❌ Banner removed here

        spawnedSegments.Enqueue(segmentRoot);
        nextSegmentZ += spacing;
    }

    void CreateRoad(Transform parent)
    {
        GameObject safetyGround = CreateBlock("SafetyGround", new Vector3(0f, -1.35f, 0f),
            new Vector3(roadWidth + sidewalkWidth * 2.4f, 2.2f, spacing + 0.6f),
            new Color(0.24f, 0.19f, 0.11f), parent);

        var safetyRenderer = safetyGround.GetComponent<Renderer>();
        if (safetyRenderer != null)
            safetyRenderer.enabled = false;

        CreateBlock("Road", new Vector3(0f, -0.2f, 0f), new Vector3(roadWidth, 0.4f, spacing), GetRoadColor(), parent);
        CreateBlock("LeftSidewalk", new Vector3(-(roadWidth + sidewalkWidth) * 0.5f, -0.05f, 0f), new Vector3(sidewalkWidth, 0.3f, spacing), GetSidewalkColor(), parent);
        CreateBlock("RightSidewalk", new Vector3((roadWidth + sidewalkWidth) * 0.5f, -0.05f, 0f), new Vector3(sidewalkWidth, 0.3f, spacing), GetSidewalkColor(), parent);
        CreateBlock("LeftGrass", new Vector3(-7.6f, -0.18f, 0f), new Vector3(4.2f, 0.08f, spacing + 0.4f), new Color(0.67f, 0.86f, 0.58f), parent);
        CreateBlock("RightGrass", new Vector3(7.6f, -0.18f, 0f), new Vector3(4.2f, 0.08f, spacing + 0.4f), new Color(0.67f, 0.86f, 0.58f), parent);
    }

    void CreateLaneStripes(Transform parent)
    {
        for (int lane = -1; lane <= 0; lane++)
        {
            CreateBlock("Stripe_" + lane,
                new Vector3((lane + 0.5f) * 2.5f, 0.02f, 0f),
                new Vector3(0.18f, 0.05f, spacing * 0.55f),
                new Color(1f, 0.95f, 0.76f),
                parent);
        }
    }

    void CreateStreetProps(Transform parent)
    {
        CreateFence(false, parent);
        CreateFence(true, parent);

        if (Random.value < 0.75f) CreateLamp(false, parent);
        if (Random.value < 0.75f) CreateLamp(true, parent);

        if (Random.value < 0.55f) CreateTree(false, parent);
        if (Random.value < 0.55f) CreateTree(true, parent);

        if (Random.value < 0.22f) CreateBenchCluster(parent);
    }

    void CreateLamp(bool rightSide, Transform parent)
    {
        float side = rightSide ? 1f : -1f;
        float x = side * 5.25f;
        float z = Random.Range(-1.4f, 1.4f);

        Transform pole = CreateCylinder("LampPole",
            new Vector3(x, 1.9f, z),
            new Vector3(0.09f, 1.9f, 0.09f),
            new Color(0.28f, 0.34f, 0.4f), parent).transform;

        CreateSphere("LampGlow", new Vector3(0f, 2f, 0f), Vector3.one * 0.34f,
            new Color(1f, 0.95f, 0.72f), pole);
    }

    void CreateBenchCluster(Transform parent)
    {
        float side = Random.value < 0.5f ? -1f : 1f;
        float x = side * 4.6f;
        float z = Random.Range(-1.4f, 1.4f);

        Transform bench = CreateBlock("BenchSeat",
            new Vector3(x, 0.45f, z),
            new Vector3(1.1f, 0.14f, 0.42f),
            new Color(0.62f, 0.4f, 0.24f), parent).transform;

        CreateBlock("BenchBack", new Vector3(0f, 0.34f, -0.16f), new Vector3(1.1f, 0.55f, 0.08f), new Color(0.58f, 0.37f, 0.22f), bench);
        CreateBlock("BenchLegL", new Vector3(-0.42f, -0.28f, 0f), new Vector3(0.08f, 0.55f, 0.08f), new Color(0.24f, 0.24f, 0.26f), bench);
        CreateBlock("BenchLegR", new Vector3(0.42f, -0.28f, 0f), new Vector3(0.08f, 0.55f, 0.08f), new Color(0.24f, 0.24f, 0.26f), bench);
    }

    void CreateFence(bool rightSide, Transform parent)
    {
        float side = rightSide ? 1f : -1f;
        float x = side * 4.45f;

        CreateCylinder("FencePostA", new Vector3(x, 0.52f, -1.6f), new Vector3(0.06f, 0.52f, 0.06f), new Color(0.82f, 0.78f, 0.72f), parent);
        CreateCylinder("FencePostB", new Vector3(x, 0.52f, 1.6f), new Vector3(0.06f, 0.52f, 0.06f), new Color(0.82f, 0.78f, 0.72f), parent);

        CreateBlock("FenceRailTop", new Vector3(x, 0.78f, 0f), new Vector3(0.08f, 0.08f, 3.3f), new Color(0.9f, 0.88f, 0.84f), parent);
        CreateBlock("FenceRailMid", new Vector3(x, 0.48f, 0f), new Vector3(0.08f, 0.08f, 3.3f), new Color(0.9f, 0.88f, 0.84f), parent);
    }

    void CreateTree(bool rightSide, Transform parent)
    {
        float side = rightSide ? 1f : -1f;
        float x = side * Random.Range(6.3f, 8.3f);
        float z = Random.Range(-2f, 2f);
        float height = Random.Range(1.1f, 1.5f);

        Transform trunk = CreateCylinder("TreeTrunk",
            new Vector3(x, height * 0.5f, z),
            new Vector3(0.16f, height * 0.5f, 0.16f),
            new Color(0.47f, 0.3f, 0.18f), parent).transform;

        CreateSphere("TreeTopA", new Vector3(0f, height, 0f), new Vector3(1.1f, 0.9f, 1.1f), new Color(0.48f, 0.72f, 0.42f), trunk);
        CreateSphere("TreeTopB", new Vector3(0.34f, height - 0.12f, 0.08f), new Vector3(0.86f, 0.7f, 0.86f), new Color(0.58f, 0.81f, 0.48f), trunk);
    }

    void ClearExistingChildren()
    {
        spawnedSegments.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    void CreateFallbackGround()
    {
        if (fallbackGround != null)
            Destroy(fallbackGround);

        fallbackGround = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fallbackGround.name = "FallbackGround";
        fallbackGround.transform.SetParent(transform, false);
        fallbackGround.transform.localPosition = new Vector3(0f, -2.2f, 260f);
        fallbackGround.transform.localScale = new Vector3(roadWidth + sidewalkWidth * 4f, 2.5f, 1200f);

        var r = fallbackGround.GetComponent<Renderer>();
        if (r != null) r.enabled = false;
    }

    void CreateSkyBackdrop()
    {
        if (Camera.main != null)
            Camera.main.backgroundColor = new Color(0.89f, 0.96f, 1f);
    }

    GameObject CreateBlock(string name, Vector3 pos, Vector3 scale, Color color, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = pos;
        obj.transform.localScale = scale;
        ApplyRuntimeColor(obj.GetComponent<Renderer>(), color);
        return obj;
    }

    GameObject CreateCylinder(string name, Vector3 pos, Vector3 scale, Color color, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = pos;
        obj.transform.localScale = scale;
        ApplyRuntimeColor(obj.GetComponent<Renderer>(), color);
        return obj;
    }

    GameObject CreateSphere(string name, Vector3 pos, Vector3 scale, Color color, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = pos;
        obj.transform.localScale = scale;
        ApplyRuntimeColor(obj.GetComponent<Renderer>(), color);
        return obj;
    }

    void ApplyRuntimeColor(Renderer renderer, Color color)
    {
        if (renderer == null)
            return;

        Material material = new Material(runtimeShader != null ? runtimeShader : Shader.Find("Standard"));

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);

        renderer.material = material;
    }

    Shader FindRuntimeShader()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader != null)
            return shader;

        shader = Shader.Find("Universal Render Pipeline/Simple Lit");
        if (shader != null)
            return shader;

        shader = Shader.Find("Standard");
        return shader;
    }

    Color GetRoadColor() => new Color(0.15f, 0.16f, 0.2f);
    Color GetSidewalkColor() => new Color(0.74f, 0.73f, 0.76f);
}
