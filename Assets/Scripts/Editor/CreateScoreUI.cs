using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class CreateScoreUI
{
    [MenuItem("Tools/Create Score UI")]
    static void CreateUI()
    {
        // Find or create Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create Score Text
        GameObject textObj = new GameObject("ScoreText");
        textObj.transform.SetParent(canvas.transform);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Clothes: 0";
        text.fontSize = 60;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.TopLeft;

        // RectTransform setup
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);

        rect.anchoredPosition = new Vector2(50, -50);
        rect.sizeDelta = new Vector2(400, 100);

        // Try assign to GameManager
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.scoreText = text;
        }

        Debug.Log("✅ Score UI created and linked!");
    }
}
