using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public int position = -1;

    [SerializeField] private int HP = 100;
    private List<Skill> bufList = new List<Skill>();
    private List<Skill> DeBufList = new List<Skill>();

    public int remainCoolTime = 0;
    public Slider HPStatus;

    private int _initialHP;

    // 보유한 스킬 리스트
    public List<Skill> Skills = new List<Skill>();
    public BattleManager battleManager = null;

    private void Start()
    {
        Debug.Log(gameObject.name);
        if (Skills.Count > 8)
        {
            throw new Exception("Skill Count Exceeds 8");
        }

        if (position == -1)
        {
            throw new Exception("Position not initialized");
        }

        if (battleManager == null)
        {
            throw new Exception("Character: Battlemanager not initialized");
        }

        _initialHP = HP;
    }

    public void getDamage(int damage)
    {
        HP -= damage;
        updateHPBar();
    }

    public void move(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public void addBuf(Skill buf)
    {
        bufList.Add(buf);
    }

    public void addDebuf(Skill debuf)
    {
        DeBufList.Add(debuf);
    }


    public void updateHPBar()
    {
        HPStatus.value = (float)HP / _initialHP;
    }

    public void OnMouseUp()
    {
        battleManager.setTarget(this);
    }
}