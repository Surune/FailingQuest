using System;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    [SerializeField] protected int coolTime = -1;
    [SerializeField] protected SkillType skillType = SkillType._UNDEFINED;
    public string description = "Description";
    [SerializeField] private int damage = 0; // attack skill

    public BattleManager battleManager = null;
    public Button button = null;

    private void Start()
    {
        if (coolTime == -1 || skillType == SkillType._UNDEFINED)
        {
            throw new Exception("Skill Not Implemented");
        }

        if (button == null)
        {
            throw new Exception("Button Not Assigned");
        }

        if (battleManager == null)
        {
            throw new Exception("BattleManager Not Assigned");
        }

        button.onClick.AddListener(() => _Use());
    }

    public int _Use()
    {
        Debug.Log("use");
        Character target = battleManager.getTarget();
        if (skillType == SkillType.Move)
        {
            int pos = battleManager.getPosition();
            return Use(target, pos);
        }
        return Use(target, -1);
    }

    public int Use(Character target, int pos)
    {
        switch (skillType)
        {
            case SkillType.Attack:
                target.getDamage(damage);
                return coolTime;
            case SkillType.Move:
                if (pos == -1)
                {
                    throw new Exception("Invalid Move Position");
                }

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