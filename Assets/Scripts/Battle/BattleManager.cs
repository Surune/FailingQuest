using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour //전투의 진행을 담당
{
    private int _time = 0;

    [SerializeField] public List<Vector2> speeds;
    //TODO location
    //TODO mySkillList
    //TODO enemySkillList
    
    
    void Start()
    {
        Reset();
    }

    void Update()
    {
        
    }

    public void Reset()
    {
        _time = 0;
    }

    public int FindMinCoolTime()
    {
        throw new NotImplementedException();
    }
}
