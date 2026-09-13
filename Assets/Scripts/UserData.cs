using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public List<FailingQuest.Cards.Card> deck = FailingQuest.Cards.Card.Starter();
    public List<FailingQuest.Cards.CardPhrase> cardPhrases = new();
    public int money;
    public int battlesWon;
    public int elitesWon;
    public int retreats;
    public int battleRounds;
    public int nodesVisited;
    public Dictionary<CharacterType, float> partyHealth = new()
    {
        { CharacterType.character1, 1f }, { CharacterType.character2, 1f },
        { CharacterType.character3, 1f }, { CharacterType.character4, 1f },
        { CharacterType.caharcter5, 1f }
    };

    public List<CharacterType> characters;

    public int myTreasureCount;
    [Newtonsoft.Json.JsonIgnore]
    public List<GameObject> myTreasure = new();
    public List<int> myTreasureIndex;

    public int[,] questList; //전체 퀘스트 내용 저장 (11)
    //public List<List<int>> questList;
    public List<List<int>> currentQuest;
    public List<List<int>> newQuest;
    public List<int> questManage;

    public List<Dictionary<string, ForgeType>> currentSkills;
}
