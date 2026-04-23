using UnityEngine;
using UnityEditor;

public class CreateClothingPrefab
{
    [MenuItem("Tools/Create Clothing Prefab")]
    static void CreatePrefab()
    {
        // Create clothing object
        GameObject clothing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        clothing.name = "Clothing";

        // Shape it like clothing
        clothing.transform.localScale = new Vector3(1f, 0.3f, 1f);

        // Set random color
        Renderer renderer = clothing.GetComponent<Renderer>();
        renderer.sharedMaterial = new Material(Shader.Find("Standard"));
        renderer.sharedMaterial.color = Random.ColorHSV();

        // Add trigger collider
        BoxCollider col = clothing.GetComponent<BoxCollider>();
        col.isTrigger = true;

        // ✅ Add NEW script (fixed)
        Collectible collectible = clothing.AddComponent<Collectible>();

        // Set default type (you can change this later in Inspector)
        collectible.type = Collectible.ClothingType.Jeans;
        collectible.value = 1;

        // Ensure folder exists
        string folderPath = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Save as prefab
        string prefabPath = folderPath + "/Clothing.prefab";
        PrefabUtility.SaveAsPrefabAsset(clothing, prefabPath);

        // Clean up scene object
        GameObject.DestroyImmediate(clothing);

        Debug.Log("👕 Clothing prefab created at: " + prefabPath);
    }
}