using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleBootstrap : MonoBehaviour
{
    private static Sprite whiteSprite;

    [Header("Battle Tuning")]
    [SerializeField] private int startingDeploys = 8;
    [SerializeField] private int unitsPerDeploy = 3;
    [SerializeField] private float defenderSpawnLineY = 4.8f;
    [SerializeField] private float defenderSpawnJitterY = 0.25f;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.Portrait;

        EnsureCamera();

        CreateBackground();
        Gate gate = CreateGate();
        SpawnEnemyDefenders(gate);

        GameObject allyTemplate = CreateAllyTemplate();
        UnitSpawner spawner = CreateSpawner(allyTemplate, gate);
        BattleManager battleManager = CreateBattleManager();
        BattleUI battleUI = CreateUI();

        battleManager.Configure(gate, spawner, battleUI);
        battleUI.Configure(battleManager, spawner);
        battleUI.SetGateHp(gate.Health.CurrentHealth, gate.Health.MaxHealth);
    }

    private void EnsureCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }

        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.orthographic = true;
        cam.orthographicSize = 9f;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.16f);
    }

    private void CreateBackground()
    {
        GameObject field = CreateSpriteObject("Battlefield", new Vector3(0f, 0f, 5f), new Color(0.19f, 0.32f, 0.22f));
        field.transform.localScale = new Vector3(8f, 18f, 1f);

        GameObject playerLane = CreateSpriteObject("PlayerDeployZone", new Vector3(0f, -6.3f, 4f), new Color(0.12f, 0.26f, 0.36f, 0.8f));
        playerLane.transform.localScale = new Vector3(8f, 2.2f, 1f);
    }

    private Gate CreateGate()
    {
        GameObject gateObj = CreateSpriteObject("EnemyGate", new Vector3(0f, 7f, 0f), new Color(0.85f, 0.3f, 0.2f));
        gateObj.transform.localScale = new Vector3(3.2f, 1.7f, 1f);

        Health gateHealth = gateObj.AddComponent<Health>();
        gateHealth.SetMaxHealth(320f, true);

        Transform hpBg = CreateSpriteObject("GateHealthBg", new Vector3(0f, 1.3f, 0f), new Color(0f, 0f, 0f, 0.6f)).transform;
        hpBg.SetParent(gateObj.transform, false);
        hpBg.localScale = new Vector3(2.4f, 0.28f, 1f);

        Transform hpFill = CreateSpriteObject("GateHealthFill", new Vector3(0f, 1.3f, -0.1f), new Color(0.2f, 0.85f, 0.2f)).transform;
        hpFill.SetParent(gateObj.transform, false);
        hpFill.localScale = new Vector3(1f, 0.24f, 1f);

        Gate gate = gateObj.AddComponent<Gate>();
        gate.Configure(gateHealth, hpFill, gateObj.GetComponent<SpriteRenderer>());

        return gate;
    }

    private void SpawnEnemyDefenders(Gate gate)
    {
        Unit.CombatStats defenderStats = new Unit.CombatStats
        {
            maxHealth = 40f,
            moveSpeed = 1.4f,
            attackDamage = 5f,
            attackRange = 0.8f,
            attackCooldown = 0.75f
        };

        for (int i = 0; i < 5; i++)
        {
            float x = Mathf.Lerp(-2.5f, 2.5f, i / 4f);
            float y = defenderSpawnLineY + Random.Range(-defenderSpawnJitterY, defenderSpawnJitterY);
            CreateCombatUnit($"EnemyDefender_{i + 1}", Team.Enemy, defenderStats, new Vector3(x, y, 0f), gate, new Color(0.9f, 0.2f, 0.2f));
        }
    }

    private GameObject CreateAllyTemplate()
    {
        GameObject template = CreateCombatUnit("AllyTemplate", Team.Ally, default, Vector3.zero, null, new Color(0.2f, 0.75f, 0.95f));
        template.SetActive(false);
        return template;
    }

    private GameObject CreateCombatUnit(string name, Team team, Unit.CombatStats stats, Vector3 position, Gate enemyGate, Color color)
    {
        GameObject unitObj = CreateSpriteObject(name, position, color);
        unitObj.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

        Unit unit = unitObj.AddComponent<Unit>();
        if (stats.maxHealth > 0f)
        {
            unit.Initialize(team, stats, enemyGate);
        }

        return unitObj;
    }

    private UnitSpawner CreateSpawner(GameObject allyTemplate, Gate gate)
    {
        GameObject spawnerObj = new GameObject("UnitSpawner");
        UnitSpawner spawner = spawnerObj.AddComponent<UnitSpawner>();

        Unit.CombatStats allyStats = new Unit.CombatStats
        {
            maxHealth = 35f,
            moveSpeed = 1.9f,
            attackDamage = 6f,
            attackRange = 0.8f,
            attackCooldown = 0.65f
        };

        spawner.Configure(
            allyTemplate,
            gate,
            startingDeploys,
            unitsPerDeploy,
            new Vector2(0f, -6.4f),
            1.7f,
            allyStats
        );

        return spawner;
    }

    private BattleManager CreateBattleManager()
    {
        GameObject managerObj = new GameObject("BattleManager");
        return managerObj.AddComponent<BattleManager>();
    }

    private BattleUI CreateUI()
    {
        EnsureEventSystem();

        GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Text deployCountText = CreateText(canvas.transform, "DeployCount", "Deploys: 0/0", font, 54, TextAnchor.UpperLeft);
        RectTransform deployCountRect = deployCountText.rectTransform;
        deployCountRect.anchorMin = new Vector2(0f, 1f);
        deployCountRect.anchorMax = new Vector2(0f, 1f);
        deployCountRect.pivot = new Vector2(0f, 1f);
        deployCountRect.anchoredPosition = new Vector2(40f, -40f);
        deployCountRect.sizeDelta = new Vector2(480f, 100f);

        Text gateHpText = CreateText(canvas.transform, "GateHp", "Gate HP: 0/0", font, 54, TextAnchor.UpperRight);
        RectTransform gateHpRect = gateHpText.rectTransform;
        gateHpRect.anchorMin = new Vector2(1f, 1f);
        gateHpRect.anchorMax = new Vector2(1f, 1f);
        gateHpRect.pivot = new Vector2(1f, 1f);
        gateHpRect.anchoredPosition = new Vector2(-40f, -40f);
        gateHpRect.sizeDelta = new Vector2(520f, 100f);

        Button deployButton = CreateButton(canvas.transform, "DeployButton", "DEPLOY", font);
        RectTransform deployButtonRect = deployButton.GetComponent<RectTransform>();
        deployButtonRect.anchorMin = new Vector2(0.5f, 0f);
        deployButtonRect.anchorMax = new Vector2(0.5f, 0f);
        deployButtonRect.pivot = new Vector2(0.5f, 0f);
        deployButtonRect.anchoredPosition = new Vector2(0f, 60f);
        deployButtonRect.sizeDelta = new Vector2(440f, 140f);

        GameObject resultPanel = new GameObject("ResultPanel", typeof(RectTransform), typeof(Image));
        resultPanel.transform.SetParent(canvas.transform, false);
        Image panelImage = resultPanel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);
        RectTransform panelRect = resultPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(760f, 520f);

        Text resultText = CreateText(resultPanel.transform, "ResultText", "", font, 64, TextAnchor.MiddleCenter);
        RectTransform resultTextRect = resultText.rectTransform;
        resultTextRect.anchorMin = new Vector2(0.5f, 0.65f);
        resultTextRect.anchorMax = new Vector2(0.5f, 0.65f);
        resultTextRect.pivot = new Vector2(0.5f, 0.5f);
        resultTextRect.anchoredPosition = Vector2.zero;
        resultTextRect.sizeDelta = new Vector2(660f, 220f);

        Button restartButton = CreateButton(resultPanel.transform, "RestartButton", "RESTART", font);
        RectTransform restartRect = restartButton.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0.25f);
        restartRect.anchorMax = new Vector2(0.5f, 0.25f);
        restartRect.pivot = new Vector2(0.5f, 0.5f);
        restartRect.anchoredPosition = Vector2.zero;
        restartRect.sizeDelta = new Vector2(340f, 120f);

        BattleUI ui = canvasObj.AddComponent<BattleUI>();
        ui.SetReferences(deployButton, deployCountText, gateHpText, resultPanel, resultText, restartButton);

        return ui;
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private static Button CreateButton(Transform parent, string name, string label, Font font)
    {
        GameObject buttonObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObj.transform.SetParent(parent, false);

        Image image = buttonObj.GetComponent<Image>();
        image.color = new Color(0.2f, 0.45f, 0.88f, 0.95f);

        Text labelText = CreateText(buttonObj.transform, "Label", label, font, 56, TextAnchor.MiddleCenter);
        labelText.color = Color.white;
        RectTransform labelRect = labelText.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return buttonObj.GetComponent<Button>();
    }

    private static Text CreateText(Transform parent, string name, string value, Font font, int size, TextAnchor anchor)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(parent, false);

        Text text = textObj.GetComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.alignment = anchor;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.color = Color.white;
        text.text = value;

        return text;
    }

    private static GameObject CreateSpriteObject(string name, Vector3 position, Color color)
    {
        GameObject obj = new GameObject(name, typeof(SpriteRenderer));
        obj.transform.position = position;

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = GetWhiteSprite();
        sr.color = color;

        return obj;
    }

    private static Sprite GetWhiteSprite()
    {
        if (whiteSprite != null)
        {
            return whiteSprite;
        }

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return whiteSprite;
    }
}
