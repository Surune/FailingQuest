using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    [SerializeField] private int position = -1;

    [SerializeField] private int HP = 100;
    private List<BufSkill> bufList = new List<BufSkill>();
    private List<DebufSkill> DeBufList = new List<DebufSkill>();

    public int remainCoolTime = 0;

    // 보유한 스킬 리스트
    public List<Skill> Skills = new List<Skill>();

    public Character()
    {
        if (Skills.Count > 8)
        {
            throw new Exception("Skill Count Exceeds 8");
        }
    }

    public void getDamage(int damage)
    {
        HP -= damage;
    }

    public void move(int position)
    {
        this.position = position;
    }

    public void addBuf(BufSkill buf)
    {
        bufList.Add(buf);
    }

    public void addDebuf(DebufSkill debuf)
    {
        DeBufList.Add(debuf);
    }


    public int getHP()
    {
        return HP;
    }
}