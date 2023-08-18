using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour //전투의 진행을 담당
{
    private int time;
    public List<GameObject> location;
    public Character _target_test;
    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        time = 0;
    }

    public int FindMinCoolTime()
    {
        throw new NotImplementedException();
    }

    
    /*
     *  SKill Methods
     */
    public Character getTarget()
    {
        return _target_test;
        //throw new NotImplementedException();
    }
    
    public int getPosition()
    {
        throw new NotImplementedException();
    }
}