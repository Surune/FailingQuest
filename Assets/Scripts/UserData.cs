using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public int money;

    public List<CharacterType> characters;

    public int myTreasureCount;
    public List<GameObject> myTreasure;
    public List<int> myTreasureIndex;

    public int[,] questList; //전체 퀘스트 내용 저장 (11)
    //public List<List<int>> questList;
    public List<List<int>> currentQuest;
    public List<List<int>> newQuest;
    public List<int> questManage;

    public List<Dictionary<string, ForgeType>> currentSkills;
}
