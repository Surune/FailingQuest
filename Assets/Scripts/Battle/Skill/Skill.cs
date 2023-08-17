using System;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected int coolTime = -1;
    [SerializeField] protected SkillType skillType = SkillType._UNDEFINED;
    public string description = "Description";
    [SerializeField] private int damage = 0; // attack skill

    private void Start()
    {
        if (coolTime == -1 || skillType == SkillType._UNDEFINED)
        {
            throw new Exception("Skill Not Implemented");
        }
    }

    public int Use(Character target, int pos)
    {
        switch (skillType)
        {
            case SkillType.Attack:
                target.getDamage(damage);
                return coolTime;
            case SkillType.Move:
                target.move(pos);
                return coolTime;
            case SkillType.Buf:
                target.addBuf(this);
                return coolTime;
            case SkillType.DeBuf:
                target.addDebuf(this);
                return coolTime;
            default:
                throw new Exception("Skill use error");
        }
    }
}