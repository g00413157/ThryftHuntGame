using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CreateRunnerHUD
{
    [MenuItem("Tools/Create Subway HUD")]
    static void CreateHUD()
    {
        // Canvas
        GameObject canvasGO = new GameObject("HUD Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        // =========================
        // HUD TEXT
        // =========================

        // SCORE
        GameObject scoreGO = CreateText("ScoreText", canvasGO.transform);
        SetTopLeft(scoreGO, new Vector2(30, -30));
        scoreGO.GetComponent<TextMeshProUGUI>().text = "Clothes: 0   Score: 0";

        // DISTANCE
        GameObject distGO = CreateText("DistanceText", canvasGO.transform);
        SetTopRight(distGO, new Vector2(-30, -30));
        distGO.GetComponent<TextMeshProUGUI>().text = "0m";

        // COMBO
        GameObject comboGO = CreateText("ComboText", canvasGO.transform);
        SetTopCenter(comboGO, new Vector2(0, -40));
        var comboText = comboGO.GetComponent<TextMeshProUGUI>();
        comboText.text = "x1";
        comboText.alignment = TextAlignmentOptions.Center;

        // =========================
        // PAUSE BUTTON (TOP RIGHT)
        // =========================
        Button pauseButton = CreateButton("PauseButton", canvasGO.transform, "II", new Vector2(-30, -120));
        SetTopRight(pauseButton.gameObject, new Vector2(-30, -120));

        // =========================
        // PAUSE PANEL
        // =========================
        GameObject pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasGO.transform, false);

        Image panelImage = pausePanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        RectTransform panelRT = pausePanel.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        pausePanel.SetActive(false);

        // Title
        CreateCenteredText("PAUSED", pausePanel.transform, new Vector2(0, 200), 80);

        // Buttons
        Button resumeBtn = CreateButton("ResumeButton", pausePanel.transform, "RESUME", new Vector2(0, 50));
        Button restartBtn = CreateButton("RestartButton", pausePanel.transform, "RESTART", new Vector2(0, -50));
        Button quitBtn = CreateButton("QuitButton", pausePanel.transform, "QUIT", new Vector2(0, -150));

        // =========================
        // LINK TO GAME MANAGER
        // =========================
        GameManager gm = Object.FindFirstObjectByType<GameManager>();

        if (gm != null)
        {
            gm.scoreText = scoreGO.GetComponent<TextMeshProUGUI>();
            gm.distanceText = distGO.GetComponent<TextMeshProUGUI>();
            gm.comboText = comboGO.GetComponent<TextMeshProUGUI>();

            gm.pausePanel = pausePanel;

            pauseButton.onClick.AddListener(gm.TogglePause);
            resumeBtn.onClick.AddListener(gm.ResumeGame);
            restartBtn.onClick.AddListener(gm.RestartGame);
            quitBtn.onClick.AddListener(gm.QuitToMenu);
        }

        Debug.Log("✅ HUD + Pause UI created!");
        Selection.activeGameObject = canvasGO;
    }

    // =========================
    // HELPERS
    // =========================

    static GameObject CreateText(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(500, 120);

        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.fontSize = 42;
        text.color = Color.white;

        return go;
    }

    static void SetTopLeft(GameObject go, Vector2 pos)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos;
    }

    static void SetTopRight(GameObject go, Vector2 pos)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = pos;
    }

    static void SetTopCenter(GameObject go, Vector2 pos)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = pos;
    }

    static Button CreateButton(string name, Transform parent, string label, Vector2 pos)
    {
        GameObject btn = new GameObject(name);
        btn.transform.SetParent(parent, false);

        Image img = btn.AddComponent<Image>();
        img.color = new Color32(50, 50, 50, 200);

        Button button = btn.AddComponent<Button>();

        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 120);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btn.transform, false);

        TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = 50;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;

        RectTransform txtRT = txt.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;

        return button;
    }

    static void CreateCenteredText(string textValue, Transform parent, Vector2 pos, int size)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);

        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        text.text = textValue;
        text.fontSize = size;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(800, 200);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
    }
}