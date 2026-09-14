using System.Linq;
using FailingQuest.Combat;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static FailingQuest.Combat.RankBattleController;

// Editor-only authoring. The game loads the saved prefab with all UI and asset references already wired.
public static class RankCombatAuthoring
{
    private static TMP_FontAsset font;
    private static readonly Color Ink = Tone(0.065f, 0.062f, 0.07f);
    private static readonly Color Gold = Tone(0.76f, 0.61f, 0.36f);

    [MenuItem("FailingQuest/Author rank combat prefab")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene("Assets/01_Scenes/BattleScene.unity", OpenSceneMode.Single);
        // Preserve every original object and reference for comparison or rollback.
        var originalRoots = scene.GetRootGameObjects();
        var legacyRoots = originalRoots.Where(r => r.name == "Legacy Battle (preserved)").ToArray();
        var legacy = legacyRoots.Length == 1 ? legacyRoots[0] : new GameObject("Legacy Battle (preserved)");
        legacy.SetActive(false);
        foreach (var root in originalRoots)
        {
            // Only replace this tool's generated prefab instance, never original scene content.
            if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root) == "Assets/02_Prefabs/Battle/RankBattle.prefab")
                Object.DestroyImmediate(root);
            else if (root != legacy) root.transform.SetParent(legacy.transform, true);
        }
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/04_Fonts/Galmuri11.asset");
        var rootObject = new GameObject("Rank Battle", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = rootObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = rootObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = V2(1440, 900);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        var controller = rootObject.AddComponent<RankBattleController>();
        var stage = Rect("Composition", rootObject.transform, 0, 0, 1440, 900);
        stage.anchorMin = stage.anchorMax = V2(0.5f, 0.5f);
        stage.pivot = V2(0.5f, 0.5f);
        controller.Stage = stage;
        Panel("Backdrop", stage, 0, 0, 1440, 900, Ink);
        var backdrop = Panel("Ruins", stage, 0, 235, 1440, 610, Color.white);
        backdrop.sprite = SpriteAt("Assets/Sprites/battlebackground 1.png");
        backdrop.color = Tone(0.48f, 0.44f, 0.43f);
        Panel("Sky shade", stage, 0, 725, 1440, 175, Tone(0.03f, 0.025f, 0.04f, 0.8f));
        Panel("Ground shade", stage, 0, 245, 1440, 115, Tone(0.06f, 0.045f, 0.055f, 0.86f));
        Text("Chapter", stage, 40, 839, 400, 32, "FAILING QUEST  /  잊힌 지하묘지", 18, Gold);
        controller.RoundText = Text("Round", stage, 560, 806, 320, 60, "ROUND  01", 34, Gold, TextAlignmentOptions.Center);
        controller.TurnText = Text("Initiative", stage, 150, 746, 1140, 35, "행동 순서", 17, Tone(0.82f, 0.79f, 0.72f), TextAlignmentOptions.Center);
        Text("Party", stage, 260, 667, 500, 24, "원 정 대", 17, Gold);
        Text("Enemy", stage, 800, 667, 520, 24, "폐 허 의 주 인", 17, Tone(0.72f, 0.38f, 0.32f));
        Text("Divider", stage, 460, 456, 62, 80, "/", 27, Gold, TextAlignmentOptions.Center);
        controller.Views = new CombatUnitView[MaxPlayerCount + MaxEnemyCount];
        for (int i = 0; i < MaxPlayerCount + MaxEnemyCount; i++) controller.Views[i] = Unit(stage, i, controller);

        Panel("HUD", stage, 0, 0, 1440, 276, Ink);
        Panel("Gold divider", stage, 28, 274, 1384, 2, Gold);
        Panel("Portrait border", stage, 28, 73, 140, 176, Tone(0.3f, 0.24f, 0.16f));
        controller.HeroPortrait = Panel("Active portrait", stage, 36, 82, 124, 159, Color.white);
        controller.HeroPortrait.preserveAspect = true;
        controller.HeroText = Text("Hero", stage, 189, 218, 463, 36, "수호자", 25, Gold);
        controller.StatsText = Text("Stats", stage, 189, 148, 456, 65, "", 18, Tone(0.83f, 0.8f, 0.73f));
        controller.SkillButtons = new Button[4];
        controller.SkillLabels = new TMP_Text[4];
        controller.SkillIcons = new Image[4];
        for (int i = 0; i < 4; i++)
        {
            var button = MakeButton("Skill " + i, stage, 190 + i * 111, 49, 103, 90, "", 17);
            controller.SkillButtons[i] = button;
            controller.SkillLabels[i] = button.GetComponentInChildren<TMP_Text>();
            Place(controller.SkillLabels[i].rectTransform, 2, 2, 99, 41);
            controller.SkillLabels[i].fontSize = 14;
            controller.SkillLabels[i].enableWordWrapping = false;
            controller.SkillIcons[i] = Panel("Icon", button.transform, 32, 47, 38, 38, Color.white);
            controller.SkillIcons[i].preserveAspect = true;
        }
        Panel("HUD divider 1", stage, 653, 48, 1, 200, Tone(0.35f, 0.28f, 0.2f));
        controller.SkillText = Text("Skill description", stage, 678, 128, 410, 123, "", 18, Tone(0.87f, 0.84f, 0.78f));
        controller.TargetText = Text("Target", stage, 1110, 123, 305, 125, "", 17, Tone(0.8f, 0.77f, 0.7f));
        controller.PassButton = MakeButton("Pass", stage, 902, 65, 93, 41, "대기", 16);
        controller.CancelButton = MakeButton("Cancel", stage, 1004, 65, 88, 41, "취소", 16);
        controller.PromptText = Text("Prompt", stage, 180, 7, 1110, 29, "", 17, Gold, TextAlignmentOptions.Center);
        controller.LogText = Text("Combat log", stage, 36, 286, 900, 48, "", 13, Tone(0.79f, 0.75f, 0.66f));
        controller.RetreatButton = MakeButton("Retreat", stage, 1185, 835, 213, 39, "전투 이탈  ·  75%", 16);
        controller.HelpButton = MakeButton("Help", stage, 1275, 788, 123, 31, "전투 안내 [?]", 14);
        Text("Legend", stage, 1112, 62, 284, 44, "붉은 바: 체력\n흰 바: 스트레스 (최대 200)", 14, Tone(0.57f, 0.55f, 0.51f));

        controller.HelpPanel = Panel("Help overlay", stage, 0, 0, 1440, 900, Tone(0, 0, 0, 0.9f)).gameObject;
        controller.HelpPanel.GetComponent<Image>().raycastTarget = true;
        Text("Help heading", controller.HelpPanel.transform, 260, 740, 920, 60, "폐허에서 살아남는 법", 35, Gold);
        Text("Help body", controller.HelpPanel.transform, 260, 245, 930, 480,
            "01  참가자와 차례\n플레이어 1명과 적 최대 5명이 참가합니다. 매 라운드 플레이어가 먼저 행동하고, 적은 1열부터 순서대로 행동합니다.\n\n" +
            "02  스킬 → 대상\n[1–4] 또는 스킬 클릭 후, 강조된 대상을 클릭하세요. 위치에 관계없이 스킬을 사용할 수 있습니다.\n[Space]로 대기, [ESC]로 선택을 취소합니다. 대기는 스트레스 +5.\n\n" +
            "03  상태와 처치\n출혈·중독은 자기 차례 시작에 피해를 줍니다. 기절은 행동을 소모합니다.\n쓰러진 적은 전투에서 제외됩니다. 전체 공격은 살아 있는 대상 모두에게 적용됩니다.\n\n" +
            "04  스트레스와 죽음\n스트레스 100에서 붕괴/각성을 판정하고, 200에서 심장마비가 발생합니다.\n영웅은 체력 0에서 죽음의 문턱에 들어갑니다. 추가 피해는 사망 위험! 치유로 벗어나세요.\n\n" +
            "05  귀환\n적을 모두 쓰러뜨리면 승리합니다. 전투 이탈은 75% 확률이며 실패하면 행동을 잃습니다.",
            21, Tone(0.87f, 0.84f, 0.77f));
        controller.CloseHelpButton = MakeButton("Close help", controller.HelpPanel.transform, 560, 132, 320, 52, "전투로 돌아가기", 22);
        controller.HelpPanel.SetActive(false);

        controller.ResultPanel = Panel("Result overlay", stage, 0, 0, 1440, 900, Tone(0.02f, 0.015f, 0.025f, 0.96f)).gameObject;
        controller.ResultPanel.GetComponent<Image>().raycastTarget = true;
        Text("Result ornament", controller.ResultPanel.transform, 420, 622, 600, 45, "+   F A I L I N G  Q U E S T   +", 21, Gold, TextAlignmentOptions.Center);
        controller.ResultTitle = Text("Result title", controller.ResultPanel.transform, 320, 504, 800, 95, "전투 승리", 55, Gold, TextAlignmentOptions.Center);
        controller.ResultBody = Text("Result body", controller.ResultPanel.transform, 320, 341, 800, 135, "", 25, Tone(0.82f, 0.78f, 0.7f), TextAlignmentOptions.Center);
        controller.ContinueButton = MakeButton("Continue", controller.ResultPanel.transform, 510, 241, 420, 58, "지도로 돌아가기", 22);
        controller.ContinueLabel = controller.ContinueButton.GetComponentInChildren<TMP_Text>();
        controller.ResultPanel.SetActive(false);
        controller.Heroes = new[] {
            Hero(CharacterType.character1, 0, "Assets/02_Prefabs/Battle/Character/Character1.prefab"),
            Hero(CharacterType.character2, 1, "Assets/02_Prefabs/Battle/Character/Character2.prefab"),
            Hero(CharacterType.character3, 2, "Assets/02_Prefabs/Battle/Character/Character3.prefab"),
            new CombatAppearance { CharacterType = CharacterType.character4, Template = RankCombatTemplates.Hero(3), Sprite = SpriteAt("Assets/Sprites/Characters/char4.png") },
            new CombatAppearance { CharacterType = CharacterType.caharcter5, Template = RankCombatTemplates.Hero(3), Sprite = SpriteAt("Assets/Sprites/Characters/char5.png") }
        };
        string[] enemies = { "Ghoul", "RatfolkAxe", "RatfolkMage", "Witch", "Ghoul" };
        controller.Enemies = enemies.Select((name, index) => new CombatAppearance {
            Template = RankCombatTemplates.Enemy(index), Sprite = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02_Prefabs/Battle/Enemies/Monster/" + name + ".prefab")
                .GetComponentsInChildren<SpriteRenderer>(true).First(s => s.gameObject.name == "Body").sprite }).ToArray();
        for (int i = 0; i < controller.Enemies.Length; i++)
            controller.Enemies[i].Sprite = TrimSprite(controller.Enemies[i].Sprite, i);
        int[] icons = { 2, 203, 102, 205, 105, 103, 101, 202, 104 };
        controller.AbilityIcons = icons.Select(i => SpriteAt($"Assets/Resources/SkillIcons/skill_{i:000}.png")).ToArray();
        for (int i = 0; i < MaxPlayerCount + MaxEnemyCount; i++)
        {
            var view = controller.Views[i];
            var look = i < MaxPlayerCount ? controller.Heroes[i] : controller.Enemies[i - MaxPlayerCount];
            view.Portrait.sprite = look.Sprite;
            view.NameText.text = look.Template.Name;
            view.HealthText.text = $"{look.Template.Health} / {look.Template.Health}";
            view.Rect.anchoredPosition = V2(i < MaxPlayerCount ? 260 : 590 + (i - MaxPlayerCount) * 155, 355);
        }

        var events = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        events.transform.SetParent(rootObject.transform);
        var cameraObject = new GameObject("Battle Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.transform.SetParent(rootObject.transform);
        cameraObject.transform.localPosition = V3(0, 0, -10);
        var camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Ink;
        cameraObject.tag = "MainCamera";
        const string folder = "Assets/02_Prefabs/Battle";
        var prefab = PrefabUtility.SaveAsPrefabAsset(rootObject, folder + "/RankBattle.prefab");
        Object.DestroyImmediate(rootObject);
        PrefabUtility.InstantiatePrefab(prefab, scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
    }

    private static CombatAppearance Hero(CharacterType type, int role, string path) => new CombatAppearance {
        CharacterType = type, Template = RankCombatTemplates.Hero(role), Sprite = AssetDatabase.LoadAssetAtPath<GameObject>(path)
            .GetComponentsInChildren<SpriteRenderer>(true).First(s => s.gameObject.name == "Body").sprite };

    private static Sprite SpriteAt(string path) => AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().First();

    private static CombatUnitView Unit(Transform parent, int index, RankBattleController controller)
    {
        var rect = Rect("Combatant " + index, parent, 0, 355, 136, 292);
        var view = rect.gameObject.AddComponent<CombatUnitView>();
        view.Rect = rect;
        view.Controller = controller;
        view.Id = index;
        view.Highlight = Panel("Turn and target indicator", rect, 4, 61, 128, 4, Gold);
        view.Portrait = Panel("Fighter", rect, 5, 86, 126, 174, Color.white);
        view.Portrait.rectTransform.pivot = V2(0.5f, 0);
        view.Portrait.rectTransform.anchoredPosition = V2(68, 86);
        view.Portrait.preserveAspect = true;
        view.Portrait.raycastTarget = true;
        view.Button = view.Portrait.gameObject.AddComponent<Button>();
        view.Button.transition = Selectable.Transition.None;
        view.NameText = Text("Name", rect, -5, 256, 146, 30, "", 17, Tone(0.88f, 0.83f, 0.74f), TextAlignmentOptions.Center);
        Panel("HP track", rect, 12, 42, 112, 10, Tone(0.15f, 0.1f, 0.1f));
        view.HealthFill = Panel("HP fill", rect, 12, 42, 112, 10, Tone(0.72f, 0.17f, 0.15f));
        Fill(view.HealthFill);
        Panel("Stress track", rect, 12, 33, 112, 4, Tone(0.2f, 0.18f, 0.17f));
        view.StressFill = Panel("Stress fill", rect, 12, 33, 112, 4, Tone(0.8f, 0.79f, 0.73f));
        Fill(view.StressFill);
        view.StressFill.fillAmount = 0;
        view.HealthText = Text("HP value", rect, -8, 9, 152, 23, "", 14, Tone(0.84f, 0.81f, 0.74f), TextAlignmentOptions.Center);
        view.StatusText = Text("Ailments", rect, -10, -23, 156, 30, "", 12, Tone(0.81f, 0.52f, 0.32f), TextAlignmentOptions.Center);
        view.FloatingText = Text("Damage", rect, 0, 180, 136, 50, "", 33, Gold, TextAlignmentOptions.Center);
        view.TargetMarker = Text("Target marker", rect, 0, 287, 136, 24, "", 15, Gold, TextAlignmentOptions.Center);
        return view;
    }

    private static Sprite TrimSprite(Sprite source, int index)
    {
        string path = $"Assets/02_Prefabs/Battle/CombatEnemy{index}.asset";
        if (AssetDatabase.AssetPathExists(path)) return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        // Author a tighter sprite rectangle; the original texture and importer remain unchanged.
        var readable = new Texture2D(2, 2);
        readable.LoadImage(System.IO.File.ReadAllBytes(AssetDatabase.GetAssetPath(source.texture)));
        var bounds = source.rect;
        int left = (int)bounds.xMax, bottom = (int)bounds.yMax, right = (int)bounds.xMin, top = (int)bounds.yMin;
        for (int y = (int)bounds.yMin; y < bounds.yMax; y++)
            for (int x = (int)bounds.xMin; x < bounds.xMax; x++)
                if (readable.GetPixel(x, y).a > 0.05f)
                { left = Mathf.Min(left, x); right = Mathf.Max(right, x); bottom = Mathf.Min(bottom, y); top = Mathf.Max(top, y); }
        Rect trimmed = default;
        trimmed.x = left; trimmed.y = bottom; trimmed.width = right - left + 1; trimmed.height = top - bottom + 1;
        var sprite = Sprite.Create(source.texture, trimmed, V2(0.5f, 0.5f), source.pixelsPerUnit, 0, SpriteMeshType.FullRect);
        sprite.name = "Combat " + source.name;
        AssetDatabase.CreateAsset(sprite, path);
        Object.DestroyImmediate(readable);
        return sprite;
    }

    private static void Fill(Image image)
    {
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Filled; image.fillMethod = Image.FillMethod.Horizontal; image.fillOrigin = 0;
    }
    private static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        Place(rect, x, y, w, h);
        return rect;
    }

    private static void Place(RectTransform rect, float x, float y, float w, float h)
    {
        rect.anchorMin = rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;
        rect.anchoredPosition = V2(x, y);
        rect.sizeDelta = V2(w, h);
    }

    private static Image Panel(string name, Transform parent, float x, float y, float w, float h, Color color)
    {
        var rect = Rect(name, parent, x, y, w, h);
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TMP_Text Text(string name, Transform parent, float x, float y, float w, float h, string value,
        float size, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft)
    {
        var rect = Rect(name, parent, x, y, w, h);
        TMP_Text text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.fontSize = size;
        text.color = color;
        text.text = value;
        text.alignment = alignment;
        text.raycastTarget = false;
        text.enableWordWrapping = true;
        return text;
    }

    private static Button MakeButton(string name, Transform parent, float x, float y, float w, float h, string label, int size)
    {
        var image = Panel(name, parent, x, y, w, h, Tone(0.17f, 0.15f, 0.14f));
        image.raycastTarget = true;
        var button = image.gameObject.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Tone(1.35f, 1.2f, 0.9f);
        colors.selectedColor = Color.white;
        colors.disabledColor = Tone(0.35f, 0.35f, 0.35f, 0.7f);
        button.colors = colors;
        var navigation = button.navigation;
        navigation.mode = Navigation.Mode.None;
        button.navigation = navigation;
        Panel("Top edge", button.transform, 0, h - 1, w, 1, Gold);
        Text("Label", button.transform, 4, 2, w - 8, h - 4, label, size, Tone(0.9f, 0.85f, 0.73f), TextAlignmentOptions.Center);
        return button;
    }
}
