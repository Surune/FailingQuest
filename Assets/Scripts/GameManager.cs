using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UserData userData = CreateData();
    public Map.Map currentMap = new(new(), new());
    public GameObject[] charPrefabs;
    public TreasureCatalog treasureCatalog;
    public FailingQuest.Cards.CardCatalog cardCatalog;
    public Vector2 treasurePosition;
    public bool sceneLoadedTriger;

    private static UserData CreateData() => new()
    {
        characters = new() { CharacterType.character1 },
        myTreasure = new(), myTreasureIndex = new(),
        currentSkills = new()
        {
            new() {{"002",ForgeType.UNFORGED}}
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
        currentMap = new(new(), new());
        treasurePosition.x = -616; treasurePosition.y = 174;
    }
}
