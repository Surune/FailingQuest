using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour //전투의 진행을 담당
{
    private int time;
    public List<LocationHelper> location;
    private Character target = null; // 스킬 타겟
    private Vector3 target_position = new Vector3(0, 0, 0); // 이동 스킬 타겟 위치

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
        return target;
    }

    public void setTarget(Character character) //이벤트 방식으로 전환 필요
    {
        target = character;
    }

    public void resetTarget()
    {
        target = null;
    }

    public Vector3 getPosition()
    {
        return new Vector3(-3, 3, -0.2f);
    }

    public void setTargetPosition(Vector3 position)
    {
        target_position = position;
    }

    public void resetTargetPosition()
    {
        target_position = new Vector3(0, 0, 0);
    }
}