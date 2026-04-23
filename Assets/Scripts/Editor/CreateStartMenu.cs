using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CreateStartMenu
{
    [MenuItem("Tools/Create THRYFT Start Menu")]
    static void CreateMenu()
    {
        // Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f; // 🔥 FIXED scaling

        canvasObj.AddComponent<GraphicRaycaster>();

        // =========================
        // BACKGROUND
        // =========================
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasObj.transform);

        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color32(203, 244, 176, 255);

        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // =========================
        // CONTAINER
        // =========================
        GameObject container = new GameObject("Container");
        container.transform.SetParent(canvasObj.transform);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = new Vector2(900, 1400);

        VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 100;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        ContentSizeFitter fitter = container.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // =========================
        // LOGO
        // =========================
        GameObject logoObj = new GameObject("Logo");
        logoObj.transform.SetParent(container.transform);

        Image logoImage = logoObj.AddComponent<Image>();
        Sprite logoSprite = Resources.Load<Sprite>("logoThrift");

        if (logoSprite != null)
        {
            logoImage.sprite = logoSprite;
            logoImage.preserveAspect = true;
        }

        RectTransform logoRect = logoObj.GetComponent<RectTransform>();
        logoRect.sizeDelta = new Vector2(300, 150);

        // Shadow
        var logoShadow = logoObj.AddComponent<Shadow>();
        logoShadow.effectColor = new Color(0, 0, 0, 0.3f);
        logoShadow.effectDistance = new Vector2(3, -3);

        // =========================
        // HUNT TEXT
        // =========================
        GameObject huntObj = new GameObject("HuntText");
        huntObj.transform.SetParent(container.transform);

        TextMeshProUGUI huntText = huntObj.AddComponent<TextMeshProUGUI>();
        huntText.text = "HUNT";
        huntText.fontSize = 140;
        huntText.alignment = TextAlignmentOptions.Center;
        huntText.color = Color.white;

        huntText.outlineWidth = 0.2f;
        huntText.outlineColor = Color.black;

        // Shadow
        var shadow = huntObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.4f);
        shadow.effectDistance = new Vector2(4, -4);

        // =========================
        // START BUTTON
        // =========================
        GameObject buttonObj = new GameObject("StartButton");
        buttonObj.transform.SetParent(container.transform);

        Image buttonImage = buttonObj.AddComponent<Image>();

        // 🔥 Proper rounded UI sprite
        buttonImage.sprite = UnityEngine.Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        buttonImage.type = Image.Type.Sliced;

        buttonImage.color = new Color32(60, 190, 100, 255);

        Button button = buttonObj.AddComponent<Button>();

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(500, 160);

        // Shadow
        var buttonShadow = buttonObj.AddComponent<Shadow>();
        buttonShadow.effectColor = new Color(0, 0, 0, 0.25f);
        buttonShadow.effectDistance = new Vector2(5, -5);

        // Button colors
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = new Color32(60, 190, 100, 255);
        colors.highlightedColor = new Color32(80, 210, 120, 255);
        colors.pressedColor = new Color32(40, 150, 80, 255);
        colors.selectedColor = colors.normalColor;
        button.colors = colors;

        StartButton startScript = buttonObj.AddComponent<StartButton>();
        button.onClick.AddListener(startScript.StartGame);

        // =========================
        // BUTTON TEXT
        // =========================
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "START";
        text.fontSize = 65;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        text.margin = new Vector4(0, 10, 0, 10);

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Debug.Log("🔥 FINAL MOBILE UI READY");
    }
}