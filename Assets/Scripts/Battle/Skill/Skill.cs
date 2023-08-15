using System;
using UnityEngine;

public abstract class Skill
{
    [SerializeField] protected int coolTime = -1;
    [SerializeField] protected SkillType skillType = SkillType._UNDEFINED;
    public string description = "Description";

    public Skill()
    {
        if (coolTime == -1 || skillType == SkillType._UNDEFINED)
        {
            throw new Exception("Skill Not Implemented");
        }
    }
}

public class AttackSkill : Skill
{
    [SerializeField] private int damage = 0;

    public int Use(Character target)
    {
        target.getDamage(damage);
        return coolTime;
    }
}

public class MoveSkill : Skill
{
    public int Use(Character target, int pos)
    {
        target.move(pos);
        return coolTime;
    }
}

public class BufSkill : Skill
{
    public int Use(Character target)
    {
        target.addBuf(this);
        return coolTime;
    }
}

public class DebufSkill : Skill
{
    public int Use(Character target)
    {
        target.addDebuf(this);
        return coolTime;
    }
}