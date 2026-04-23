using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CreateGameOverUI
{
    [MenuItem("Tools/Create Game Over UI")]
    public static void CreateUI()
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

        // =========================
        // GAME OVER PANEL
        // =========================
        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvas.transform, false);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        panel.SetActive(false);

        // =========================
        // GAME OVER TITLE
        // =========================
        GameObject textObj = new GameObject("GameOverLabel");
        textObj.transform.SetParent(panel.transform, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "GAME OVER";
        text.fontSize = 100;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.red;

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(0, 200);
        textRect.sizeDelta = new Vector2(800, 200);

        // =========================
        // STATS TEXT
        // =========================
        GameObject statsObj = new GameObject("GameOverStats");
        statsObj.transform.SetParent(panel.transform, false);

        TextMeshProUGUI statsText = statsObj.AddComponent<TextMeshProUGUI>();
        statsText.text = "Score: 0\nClothes: 0\nDistance: 0m\nBest Combo: x1";
        statsText.fontSize = 40;
        statsText.alignment = TextAlignmentOptions.Center;
        statsText.color = Color.white;

        RectTransform statsRect = statsText.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0.5f, 0.5f);
        statsRect.anchorMax = new Vector2(0.5f, 0.5f);
        statsRect.anchoredPosition = new Vector2(0, 60);
        statsRect.sizeDelta = new Vector2(600, 200);

        // =========================
        // RESTART BUTTON
        // =========================
        GameObject buttonObj = new GameObject("RestartButton");
        buttonObj.transform.SetParent(panel.transform, false);

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color32(50, 180, 80, 255);

        Button button = buttonObj.AddComponent<Button>();

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = new Vector2(0, -50);
        buttonRect.sizeDelta = new Vector2(400, 120);

        // Button Text
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(buttonObj.transform, false);

        TextMeshProUGUI btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnText.text = "RESTART";
        btnText.fontSize = 50;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;

        RectTransform btnTextRect = btnText.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;

        // =========================
        // LINK TO GAME MANAGER
        // =========================
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.gameOverPanel = panel;
            gm.gameOverStats = statsText;

            button.onClick.AddListener(gm.RestartGame);
        }

        Debug.Log("✅ Game Over UI created!");
    }
}