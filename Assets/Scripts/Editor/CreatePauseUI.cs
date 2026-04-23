using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CreatePauseUI
{
    [MenuItem("Tools/Create Pause UI")]
    public static void CreateUI()
    {
        // Find Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("No Canvas found!");
            return;
        }

        // =========================
        // PAUSE PANEL
        // =========================
        GameObject panel = new GameObject("PausePanel");
        panel.transform.SetParent(canvas.transform, false);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.6f);
        panelImage.raycastTarget = false;

        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        panel.SetActive(false);

        // =========================
        // PAUSED TEXT
        // =========================
        GameObject textGO = new GameObject("PausedText");
        textGO.transform.SetParent(panel.transform, false);

        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "PAUSED";
        text.fontSize = 80;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        RectTransform textRT = text.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.5f, 0.5f);
        textRT.anchorMax = new Vector2(0.5f, 0.5f);
        textRT.anchoredPosition = new Vector2(0, 200);
        textRT.sizeDelta = new Vector2(800, 200);

        // =========================
        // CREATE BUTTON FUNCTION
        // =========================
        GameObject CreateButton(string name, string label, Vector2 pos, Color color)
        {
            GameObject btnGO = new GameObject(name);
            btnGO.transform.SetParent(panel.transform, false);

            Image img = btnGO.AddComponent<Image>();
            img.color = color;

            Button btn = btnGO.AddComponent<Button>();

            RectTransform rt = btnGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(400, 120);

            GameObject txtGO = new GameObject("Text");
            txtGO.transform.SetParent(btnGO.transform, false);

            TextMeshProUGUI txt = txtGO.AddComponent<TextMeshProUGUI>();
            txt.text = label;
            txt.fontSize = 40;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = Color.white;

            RectTransform txtRT = txtGO.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.offsetMin = Vector2.zero;
            txtRT.offsetMax = Vector2.zero;

            return btnGO;
        }

        // =========================
        // BUTTONS
        // =========================
        GameObject resumeBtn = CreateButton("ResumeButton", "RESUME", new Vector2(0, 50), Color.green);
        GameObject restartBtn = CreateButton("RestartButton", "RESTART", new Vector2(0, -50), Color.yellow);
        GameObject quitBtn = CreateButton("QuitButton", "QUIT", new Vector2(0, -150), Color.red);

        // =========================
        // LINK TO GAME MANAGER
        // =========================
        GameManager gm = Object.FindFirstObjectByType<GameManager>();

        if (gm != null)
        {
            gm.pausePanel = panel;

            resumeBtn.GetComponent<Button>().onClick.AddListener(gm.ResumeGame);
            restartBtn.GetComponent<Button>().onClick.AddListener(gm.RestartGame);
            quitBtn.GetComponent<Button>().onClick.AddListener(gm.QuitToMenu);
        }

        Debug.Log("✅ Pause UI created!");
    }
}