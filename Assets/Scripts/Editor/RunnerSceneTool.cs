using UnityEngine;
using UnityEditor;

public class RunnerSceneTool
{
    [MenuItem("Tools/Create Runner Scene (No Player)")]
    static void CreateRunnerScene()
    {
        // CAMERA
        GameObject cam = new GameObject("Main Camera");
        Camera camera = cam.AddComponent<Camera>();
        camera.orthographic = true;
        cam.transform.position = new Vector3(0, 1, -10);


        // GROUND
        GameObject ground = new GameObject("Ground");
        ground.transform.position = new Vector3(0, -1, 0);

        BoxCollider2D col = ground.AddComponent<BoxCollider2D>();
        ground.tag = "Ground";


        // GAME MANAGER
        new GameObject("GameManager");

        Debug.Log("Runner scene created (no player yet)");
    }
}