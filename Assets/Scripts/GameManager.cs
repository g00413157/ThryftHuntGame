using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player")]
    public Transform player;
    private PlayerLaneRunner runner;

    [Header("Stats")]
    public int clothesCollected;
    public float distanceTravelled;
    public int score;
    public int comboCount;
    public int bestCombo;
    public int missedClothes;
    public int maxMissedClothes = 3;

    [Header("HUD UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI comboText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverStats;

    [Header("Pause UI")]
    public GameObject pausePanel;

    [Header("Settings")]
    public float comboWindow = 4f;

    private float startZ;
    private float comboTimer;

    private bool gameEnded;
    private bool paused;

    public bool IsGameActive => !gameEnded && !paused;

    // =========================
    // SETUP
    // =========================

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        EnsureEventSystem();
        WirePauseUI();
        SetupPlayer();
        ResetStats();
        ConfigureLoseScreen();
        UpdateUI();

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (!IsGameActive || player == null) return;

        UpdateDistance();
        UpdateCombo();
        UpdateUI();
    }

    void SetupPlayer()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
                player = obj.transform;
        }

        if (player != null)
        {
            startZ = player.position.z;
            runner = player.GetComponent<PlayerLaneRunner>();
        }
    }

    void ResetStats()
    {
        clothesCollected = 0;
        distanceTravelled = 0f;
        score = 0;
        comboCount = 0;
        bestCombo = 0;
        missedClothes = 0;

        gameEnded = false;
        paused = false;
    }

    // =========================
    // GAMEPLAY
    // =========================

    void UpdateDistance()
    {
        distanceTravelled = Mathf.Max(0f, player.position.z - startZ);
    }

    void UpdateCombo()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0f)
                comboCount = 0;
        }
    }

    public void AddClothing(int amount)
    {
        clothesCollected += amount;

        comboCount++;
        bestCombo = Mathf.Max(bestCombo, comboCount);
        comboTimer = comboWindow;

        score += amount * (10 + comboCount * 2);

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void AddCombo(int amount)
    {
        comboCount += amount;
        bestCombo = Mathf.Max(bestCombo, comboCount);
        comboTimer = comboWindow;

        UpdateUI();
    }

    public void ActivateSpeedBoost(float duration)
    {
        runner?.ActivateSpeedBoost(duration);
    }

    public void HitObstacle()
    {
        UpdateUI();
    }

    public void MissClothing()
    {
        if (gameEnded)
            return;

        missedClothes++;

        if (missedClothes >= maxMissedClothes)
            GameOver();

        UpdateUI();
    }

    // =========================
    // UI
    // =========================

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Missed: {missedClothes}/{maxMissedClothes}   Clothes: {clothesCollected}   Score: {score}";

        if (distanceText != null)
            distanceText.text = $"{Mathf.FloorToInt(distanceTravelled)}m";

        if (comboText != null)
            comboText.text = $"x{Mathf.Max(1, comboCount)}";
    }

    // =========================
    // GAME STATES
    // =========================

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverStats != null)
        {
            gameOverStats.text =
                $"Score: {score}\n" +
                $"Clothes: {clothesCollected}\n" +
                $"Missed: {missedClothes}/{maxMissedClothes}\n" +
                $"Distance: {Mathf.FloorToInt(distanceTravelled)}m\n" +
                $"Best Combo: x{Mathf.Max(1, bestCombo)}";
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // =========================
    // PAUSE SYSTEM
    // =========================

    public void TogglePause()
    {
        if (gameEnded) return;

        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;

        if (pausePanel != null)
            pausePanel.SetActive(paused);
    }

    public void ResumeGame()
    {
        paused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    void ConfigureLoseScreen()
    {
        if (gameOverPanel == null)
            return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null && gameOverPanel.transform.parent != canvas.transform)
            gameOverPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.localScale = Vector3.one;
            panelRect.SetAsLastSibling();
        }

        Image overlay = gameOverPanel.GetComponent<Image>();
        if (overlay != null)
            overlay.color = new Color(0.04f, 0.08f, 0.12f, 0.9f);

        Transform existingTitle = gameOverPanel.transform.Find("GameOverLabel");
        if (existingTitle != null)
            existingTitle.gameObject.SetActive(false);

        Transform existingRestart = gameOverPanel.transform.Find("RestartButton");
        if (existingRestart != null)
            existingRestart.gameObject.SetActive(false);

        GameObject card = GetOrCreateUIObject("LoseCard", gameOverPanel.transform);
        Image cardImage = GetOrAddComponent<Image>(card);
        cardImage.color = new Color(0.1f, 0.16f, 0.19f, 0.96f);
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.sizeDelta = new Vector2(820f, 1080f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.localScale = Vector3.one;

        TextMeshProUGUI title = GetOrCreateText(
            "LoseTitle",
            card.transform,
            "THRIFT TRIP OVER",
            82,
            new Color(1f, 0.95f, 0.8f, 1f)
        );
        PlaceRect(title.rectTransform, new Vector2(0.5f, 0.82f), new Vector2(0.5f, 0.82f), Vector2.zero, new Vector2(700f, 120f));

        TextMeshProUGUI subtitle = GetOrCreateText(
            "LoseSubtitle",
            card.transform,
            "Too many clothes were missed.",
            34,
            new Color(0.76f, 0.87f, 0.9f, 1f)
        );
        PlaceRect(subtitle.rectTransform, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), Vector2.zero, new Vector2(700f, 70f));

        if (gameOverStats != null)
        {
            gameOverStats.transform.SetParent(card.transform, false);
            gameOverStats.alignment = TextAlignmentOptions.Center;
            gameOverStats.fontSize = 38;
            gameOverStats.color = new Color(0.93f, 0.97f, 0.98f, 1f);
            PlaceRect(gameOverStats.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700f, 260f));
        }

        Button restartButton = GetOrCreateButton(
            "LoseRestartButton",
            card.transform,
            "RESTART",
            new Color(0.12f, 0.7f, 0.53f, 1f),
            new Color(0.96f, 0.98f, 0.98f, 1f)
        );
        PlaceRect(restartButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f), new Vector2(-160f, 0f), new Vector2(280f, 96f));
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartGame);

        Button quitButton = GetOrCreateButton(
            "LoseQuitButton",
            card.transform,
            "QUIT GAME",
            new Color(0.76f, 0.24f, 0.2f, 1f),
            new Color(1f, 0.97f, 0.95f, 1f)
        );
        PlaceRect(quitButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f), new Vector2(160f, 0f), new Vector2(280f, 96f));
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(QuitToMenu);
    }

    void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    void WirePauseUI()
    {
        if (pausePanel == null)
        {
            GameObject panelObject = GameObject.Find("PausePanel");
            if (panelObject != null)
                pausePanel = panelObject;
        }

        Button pauseButton = FindButton("PauseButton");
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveListener(TogglePause);
            pauseButton.onClick.AddListener(TogglePause);
        }

        if (pausePanel == null)
            return;

        Button resumeButton = FindButton("ResumeButton", pausePanel.transform);
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
            resumeButton.onClick.AddListener(ResumeGame);
        }

        Button restartButton = FindButton("RestartButton", pausePanel.transform);
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
            restartButton.onClick.AddListener(RestartGame);
        }

        Button quitButton = FindButton("QuitButton", pausePanel.transform);
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitToMenu);
            quitButton.onClick.AddListener(QuitToMenu);
        }
    }

    Button FindButton(string objectName, Transform parent = null)
    {
        Transform target = parent == null
            ? FindTransformByName(objectName)
            : parent.Find(objectName);

        return target != null ? target.GetComponent<Button>() : null;
    }

    Transform FindTransformByName(string objectName)
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            Transform found = FindInChildren(rootObject.transform, objectName);
            if (found != null)
                return found;
        }

        return null;
    }

    Transform FindInChildren(Transform parent, string objectName)
    {
        if (parent.name == objectName)
            return parent;

        foreach (Transform child in parent)
        {
            Transform found = FindInChildren(child, objectName);
            if (found != null)
                return found;
        }

        return null;
    }

    GameObject GetOrCreateUIObject(string objectName, Transform parent)
    {
        Transform existing = parent.Find(objectName);
        if (existing != null)
            return existing.gameObject;

        GameObject obj = new GameObject(objectName, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        return obj;
    }

    TextMeshProUGUI GetOrCreateText(string objectName, Transform parent, string textValue, float fontSize, Color color)
    {
        GameObject obj = GetOrCreateUIObject(objectName, parent);
        TextMeshProUGUI text = GetOrAddComponent<TextMeshProUGUI>(obj);
        text.text = textValue;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    Button GetOrCreateButton(string objectName, Transform parent, string label, Color backgroundColor, Color textColor)
    {
        GameObject obj = GetOrCreateUIObject(objectName, parent);
        Image image = GetOrAddComponent<Image>(obj);
        image.color = backgroundColor;

        Button button = GetOrAddComponent<Button>(obj);
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.5f);
        button.colors = colors;

        TextMeshProUGUI text = GetOrCreateText("Label", obj.transform, label, 34, textColor);
        PlaceRect(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        return button;
    }

    T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
            component = obj.AddComponent<T>();

        return component;
    }

    void PlaceRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = Vector3.one;
    }
}
