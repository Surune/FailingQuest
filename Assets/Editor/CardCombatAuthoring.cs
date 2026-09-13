using System.Linq;
using FailingQuest.Cards;
using FailingQuest.Combat;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static FailingQuest.Combat.RankBattleController;

public static class CardCombatAuthoring
{
    private static TMP_Text textTemplate;
    private static readonly Color Gold = Tone(.85f,.68f,.4f);

    [MenuItem("FailingQuest/Format card reward nodes")]
    public static void FormatNodes()
    {
        foreach (string name in new[] { "SkillScene", "ForgeScene" })
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/"+name+".unity",OpenSceneMode.Single);
            foreach (var view in Object.FindObjectsByType<ForgeButton>(FindObjectsSortMode.None))
            {
                view.button.targetGraphic.color=Tone(.09f,.18f,.21f);
                EditorUtility.SetDirty(view.button.targetGraphic);
                PrefabUtility.RecordPrefabInstancePropertyModifications(view.button.targetGraphic);
                Layout(view.skillNameText,.04f,.74f,.96f,.95f,25);
                Layout(view.skillDescriptionText,.06f,.08f,.94f,.65f,20);
                if(name=="ForgeScene") Layout(view.forgeText,.04f,.65f,.96f,.73f,16);
            }
            foreach(var label in Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
                label.text=label.text.Replace("스킬","카드");
            EditorSceneManager.SaveScene(scene);
        }
    }
    private static void Layout(TMP_Text text,float x0,float y0,float x1,float y1,float size)
    {
        text.rectTransform.anchorMin=V2(x0,y0); text.rectTransform.anchorMax=V2(x1,y1);
        text.rectTransform.offsetMin=text.rectTransform.offsetMax=Vector2.zero;
        text.fontSize=size; text.enableAutoSizing=true; text.fontSizeMin=14; text.fontSizeMax=size;
        text.color=Gold; text.alignment=TextAlignmentOptions.Center;
        EditorUtility.SetDirty(text);
        PrefabUtility.RecordPrefabInstancePropertyModifications(text);
        PrefabUtility.RecordPrefabInstancePropertyModifications(text.rectTransform);
    }

    [MenuItem("FailingQuest/Build card battle")]
    public static void Build()
    {
        var source = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Battle/RankBattle.prefab").GetComponent<RankBattleController>();
        textTemplate = source.RoundText;
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/BattleScene.unity", OpenSceneMode.Single);
        foreach (var old in scene.GetRootGameObjects())
        {
            if (old.name == "Card Battle") Object.DestroyImmediate(old);
            else old.SetActive(false);
        }
        var root = new GameObject("Card Battle", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = V2(1440,900);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        var c = root.AddComponent<CardBattleController>();
        c.Heroes = source.Heroes; c.EnemyArt = source.Enemies.Take(3).ToArray();
        c.AbilityIcons = source.AbilityIcons;
        var stage = Rect("Composition",root.transform,0,0,1440,900);
        stage.anchorMin = stage.anchorMax = stage.pivot = V2(.5f,.5f);
        Panel("Backdrop",stage,0,0,1440,900,Tone(.035f,.05f,.065f));
        var ruins = Panel("Ruins",stage,0,286,1440,614,Tone(.48f,.52f,.56f));
        ruins.sprite = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/battlebackground 1.png").OfType<Sprite>().First();
        Panel("Top shade",stage,0,779,1440,121,Tone(.03f,.04f,.06f,.94f));
        Text("Brand",stage,42,853,650,26,"FAILING QUEST   /   DECKBOUND",19,Gold);
        c.Heading = Text("Encounter",stage,42,805,780,34,"",26,Color.white);
        c.Status = Text("Mana",stage,860,813,530,45,"",29,Gold);
        Panel("Hand background",stage,0,0,1440,300,Tone(.04f,.055f,.075f));
        Panel("Rule",stage,30,299,1380,2,Gold);
        c.HeroPortrait = Panel("Hero",stage,225,438,220,260,Color.white);
        c.HeroPortrait.preserveAspect = true;
        Text("Hero title",stage,180,700,320,38,"방랑자",25,Gold,TextAlignmentOptions.Center);
        c.HeroStatus = Text("Hero stats",stage,150,367,400,70,"",24,Color.white,TextAlignmentOptions.Center);
        c.EnemyButtons = new Button[3]; c.EnemyPortraits = new Image[3]; c.EnemyBars = new Image[3];
        c.EnemyLabels = new TMP_Text[3]; c.IntentLabels = new TMP_Text[3];
        for (int i=0;i<3;i++)
        {
            float x = 690 + i*238;
            var b = Button("Enemy "+i,stage,x,378,224,365,"",18);
            b.GetComponent<Image>().color = Tone(.06f,.075f,.09f,.45f);
            c.EnemyButtons[i] = b;
            c.EnemyPortraits[i] = Panel("Portrait",b.transform,26,72,172,205,Color.white);
            c.EnemyPortraits[i].preserveAspect = true;
            c.EnemyPortraits[i].rectTransform.pivot = V2(.5f,.5f);
            c.EnemyPortraits[i].rectTransform.anchoredPosition = V2(112,174.5f);
            c.EnemyPortraits[i].rectTransform.localScale = V3(-1,1,1);
            c.IntentLabels[i] = Text("Intent",b.transform,3,283,218,76,"",16,Gold,TextAlignmentOptions.Center);
            c.EnemyLabels[i] = Text("Health",b.transform,2,3,220,60,"",18,Color.white,TextAlignmentOptions.Center);
            Panel("Health track",b.transform,12,65,200,5,Tone(.2f,.12f,.13f));
            c.EnemyBars[i] = Panel("Health fill",b.transform,12,65,200,5,Tone(.84f,.25f,.27f));
            c.EnemyBars[i].sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            c.EnemyBars[i].type = Image.Type.Filled; c.EnemyBars[i].fillMethod = Image.FillMethod.Horizontal;
        }
        c.Log = Text("Log",stage,42,312,800,28,"",17,Gold);
        c.Piles = Text("Piles",stage,42,751,700,28,"",19,Tone(.76f,.8f,.83f));
        c.Hint = Text("Hint",stage,130,3,1180,26,"",16,Gold,TextAlignmentOptions.Center);
        c.CardButtons = new Button[10]; c.CardLabels = new TMP_Text[10];
        c.CardIcons = new Image[10]; c.CardNames = new TMP_Text[10]; c.CardCosts = new TMP_Text[10];
        for(int i=0;i<10;i++)
        {
            c.CardButtons[i] = Button("Card "+i,stage,0,35,190,237,"",18);
            c.CardLabels[i] = c.CardButtons[i].GetComponentInChildren<TMP_Text>();
            var label = c.CardLabels[i];
            label.alignment = TextAlignmentOptions.Top;
            label.rectTransform.anchorMin = Vector2.zero; label.rectTransform.anchorMax = V2(1,0);
            label.rectTransform.offsetMin = V2(7,18); label.rectTransform.offsetMax = V2(-7,91);
            c.CardNames[i] = Text("Card name",c.CardButtons[i].transform,5,174,180,27,"",19,Gold,TextAlignmentOptions.Center);
            c.CardNames[i].rectTransform.anchorMin=V2(0,1); c.CardNames[i].rectTransform.anchorMax=Vector2.one;
            c.CardNames[i].rectTransform.offsetMin=V2(3,-62); c.CardNames[i].rectTransform.offsetMax=V2(-3,-35);
            c.CardCosts[i] = Text("Mana cost",c.CardButtons[i].transform,9,201,35,32,"",25,Color.white);
            Text("Shortcut",c.CardButtons[i].transform,48,208,94,20,"["+(i+1)+"]",12,Gold);
            c.CardIcons[i] = Panel("Card art",c.CardButtons[i].transform,0,105,54,54,Color.white);
            c.CardIcons[i].preserveAspect=true;
            c.CardIcons[i].rectTransform.anchorMin=c.CardIcons[i].rectTransform.anchorMax=V2(.5f,0);
            c.CardIcons[i].rectTransform.pivot=V2(.5f,0);
            c.CardIcons[i].rectTransform.anchoredPosition=V2(0,105);
            var edge=Panel("Card border",c.CardButtons[i].transform,0,235,190,2,Gold);
            edge.rectTransform.anchorMin=V2(0,1); edge.rectTransform.anchorMax=Vector2.one;
            edge.rectTransform.offsetMin=V2(0,-2); edge.rectTransform.offsetMax=Vector2.zero;
            c.CardButtons[i].gameObject.SetActive(false);
        }
        c.EndButton = Button("End turn",stage,1175,309,226,49,"턴 종료 [Space]",22);
        c.DeckButton = Button("View deck",stage,1190,750,210,37,"보유 덱 [D]",18);
        c.DeckPanel = Panel("Deck overlay",stage,0,0,1440,900,Tone(.025f,.04f,.06f,.98f)).gameObject;
        c.DeckPanel.GetComponent<Image>().raycastTarget = true;
        c.DeckText = Text("Deck contents",c.DeckPanel.transform,160,135,1110,670,"",24,Color.white);
        c.DeckText.enableAutoSizing = true; c.DeckText.fontSizeMin=14; c.DeckText.fontSizeMax=24;
        c.CloseButton = Button("Close",c.DeckPanel.transform,1130,790,180,55,"닫기 [ESC]",22);
        c.DeckPanel.SetActive(false);
        c.ResultPanel = Panel("Result",stage,0,0,1440,900,Tone(.025f,.04f,.06f,.98f)).gameObject;
        c.ResultPanel.GetComponent<Image>().raycastTarget = true;
        c.ResultText = Text("Result title",c.ResultPanel.transform,200,667,1040,135,"",34,Gold,TextAlignmentOptions.Center);
        c.RewardButtons = new Button[3]; c.RewardLabels = new TMP_Text[3];
        for(int i=0;i<3;i++)
        {
            c.RewardButtons[i] = Button("Reward "+i,c.ResultPanel.transform,300+i*285,263,265,345,"",24);
            c.RewardLabels[i] = c.RewardButtons[i].GetComponentInChildren<TMP_Text>();
        }
        c.ContinueButton = Button("Continue",c.ResultPanel.transform,510,134,420,66,"계속하기",24);
        c.ContinueText = c.ContinueButton.GetComponentInChildren<TMP_Text>();
        c.ResultPanel.SetActive(false);
        var events = new GameObject("Input",typeof(EventSystem),typeof(StandaloneInputModule));
        events.transform.SetParent(root.transform,false);
        var camera = new GameObject("Card Camera",typeof(Camera));
        camera.transform.SetParent(root.transform,false);
        camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        camera.GetComponent<Camera>().backgroundColor = Tone(.03f,.04f,.06f);
        PrefabUtility.SaveAsPrefabAssetAndConnect(root,"Assets/Prefabs/Battle/CardBattle.prefab",InteractionMode.AutomatedAction);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("FailingQuest/Build card workshop")]
    public static void BuildWorkshop()
    {
        textTemplate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Battle/RankBattle.prefab").GetComponent<RankBattleController>().RoundText;
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/ForgeScene.unity", OpenSceneMode.Single);
        foreach (var old in scene.GetRootGameObjects())
        {
            if (old.name == "Card Workshop") Object.DestroyImmediate(old);
            else if (!old.TryGetComponent<GameManager>(out _)) old.SetActive(false);
        }
        var root = new GameObject("Card Workshop", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = V2(1440,900); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        var c = root.AddComponent<CardWorkshop>();
        var stage = Rect("Layout",root.transform,0,0,1440,900);
        stage.anchorMin = stage.anchorMax = stage.pivot = V2(.5f,.5f);
        Panel("Background",stage,0,0,1440,900,Tone(.035f,.05f,.065f));
        Text("Title",stage,40,826,1000,44,"대장간 / 문장으로 만드는 카드",32,Gold);
        c.Status = Text("Status",stage,40,745,1360,70,"",21,Color.white);
        c.UpgradeTab = Button("Upgrade",stage,40,675,230,52,"강화",23);
        c.DismantleTab = Button("Dismantle",stage,285,675,230,52,"분해",23);
        c.CombineTab = Button("Combine",stage,530,675,230,52,"조합",23);
        c.Items = new Button[8]; c.Labels = new TMP_Text[8];
        for (int i=0;i<8;i++)
        {
            c.Items[i] = Button("Item "+i,stage,40+(i%2)*380,505-(i/2)*137,365,126,"",19);
            c.Labels[i] = c.Items[i].GetComponentInChildren<TMP_Text>();
            c.Labels[i].enableAutoSizing = true; c.Labels[i].fontSizeMin = 12; c.Labels[i].fontSizeMax = 19;
        }
        Panel("Preview panel",stage,820,155,580,480,Tone(.075f,.11f,.14f));
        c.Preview = Text("Preview",stage,845,175,530,440,"",23,Gold);
        c.Preview.enableAutoSizing = true; c.Preview.fontSizeMin = 10; c.Preview.fontSizeMax = 23;
        c.Confirm = Button("Confirm",stage,820,82,280,55,"",23);
        c.Clear = Button("Clear",stage,1120,82,280,55,"선택 초기화",23);
        c.Previous = Button("Previous",stage,40,30,160,45,"이전",20);
        c.Page = Text("Page",stage,215,30,145,45,"",21,Gold,TextAlignmentOptions.Center);
        c.Next = Button("Next",stage,375,30,160,45,"다음",20);
        c.Return = Button("Return",stage,1120,827,280,48,"지도로 돌아가기",21);
        var events = new GameObject("Input",typeof(EventSystem),typeof(StandaloneInputModule));
        events.transform.SetParent(root.transform,false);
        var camera = new GameObject("Workshop Camera",typeof(Camera));
        camera.transform.SetParent(root.transform,false);
        camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        PrefabUtility.SaveAsPrefabAssetAndConnect(root,"Assets/Prefabs/Forge/CardWorkshop.prefab",InteractionMode.AutomatedAction);
        EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
    }
    private static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h)
    {
        var go = new GameObject(name,typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>(); rect.SetParent(parent,false);
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
        rect.anchoredPosition = V2(x,y); rect.sizeDelta = V2(w,h); return rect;
    }
    private static Image Panel(string name,Transform parent,float x,float y,float w,float h,Color color)
    {
        var image = Rect(name,parent,x,y,w,h).gameObject.AddComponent<Image>();
        image.color = color; image.raycastTarget = false; return image;
    }
    private static TMP_Text Text(string name,Transform parent,float x,float y,float w,float h,string value,float size,Color color,TextAlignmentOptions align=TextAlignmentOptions.TopLeft)
    {
        TMP_Text text = Object.Instantiate(textTemplate,parent);
        text.name = name;
        var rect = text.rectTransform;
        rect.anchorMin=rect.anchorMax=rect.pivot=Vector2.zero;
        rect.anchoredPosition=V2(x,y); rect.sizeDelta=V2(w,h);
        text.text=value; text.fontSize=size; text.color=color; text.alignment=align;
        text.raycastTarget=false; text.enableAutoSizing=false;
        return text;
    }
    private static Button Button(string name,Transform parent,float x,float y,float w,float h,string label,int size)
    {
        var image=Panel(name,parent,x,y,w,h,Tone(.12f,.18f,.22f)); image.raycastTarget=true;
        var button=image.gameObject.AddComponent<Button>();
        var colors=button.colors; colors.highlightedColor=Tone(1.3f,1.2f,1); colors.disabledColor=Tone(.45f,.45f,.45f); button.colors=colors;
        var navigation=button.navigation; navigation.mode=Navigation.Mode.None; button.navigation=navigation;
        var text=Text("Label",button.transform,8,8,w-16,h-16,label,size,Color.white,TextAlignmentOptions.Center);
        text.rectTransform.anchorMin=Vector2.zero; text.rectTransform.anchorMax=Vector2.one;
        text.rectTransform.offsetMin=V2(8,8); text.rectTransform.offsetMax=V2(-8,-8);
        return button;
    }
}
