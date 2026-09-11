using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UserData userData = CreateData();
    public bool firstQuestLoaded;
    public GameObject[] charPrefabs;
    public TreasureCatalog treasureCatalog;
    public FailingQuest.Combat.CombatSkillCatalog skillCatalog;
    public Vector2 treasurePosition;
    public bool sceneLoadedTriger;

    private static UserData CreateData() => new()
    {
        characters = new() { CharacterType.character1, CharacterType.character2, CharacterType.character3 },
        myTreasure = new(), myTreasureIndex = new(),
        currentQuest = new() { new() {-1,-1}, new() {-1,-1}, new() {-1,-1} },
        newQuest = new() { new() {-1,-1}, new() {-1,-1}, new() {-1,-1} },
        questManage = new() {0,0,0},
        questList = new int[,] {{30,60,90,0},{3,5,7,0},{3,4,5,0},{1,2,3,0},{3,4,5,0},{3,4,5,0},{20,30,40,0},{100,125,150,0},{3,4,5,0},{5,7,10,0},{3,5,7,0}},
        currentSkills = new()
        {
            new() {{"001",ForgeType.UNFORGED},{"002",ForgeType.UNFORGED}},
            new() {{"001",ForgeType.UNFORGED},{"002",ForgeType.UNFORGED}},
            new() {{"001",ForgeType.UNFORGED},{"002",ForgeType.UNFORGED}}
        }
    };
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResetRun();
    }
    public void ResetRun()
    {
        userData = CreateData();
        firstQuestLoaded = false;
        treasurePosition.x = -616; treasurePosition.y = 174;
        var available = new List<int>();
        for (int i=0;i<11;i++) available.Add(i);
        for (int i=0;i<3;i++)
        {
            int index = Random.Range(0, available.Count);
            int quest = available[index]; available.RemoveAt(index);
            userData.currentQuest[i][0] = quest;
            userData.currentQuest[i][1] = Random.Range(0,3);
            userData.questList[quest,3] = 1;
        }
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("UserData")) DataManager.instance.LoadData();
    }
    private void OnApplicationQuit() => DataManager.instance.SaveData();
}
