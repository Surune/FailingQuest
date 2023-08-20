using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    [SerializeField] protected int coolTime = -1;
    [SerializeField] protected SkillType skillType = SkillType._UNDEFINED;
    public string name = "Skill Name";
    public string description = "Description";
    [SerializeField] private int damage = 0; // attack skill

    public Button button = null;

    public bool buttonDisable = false; // 동시에 스킬 발동 x

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

        button.onClick.AddListener(() =>
        {
            if (!buttonDisable)
            {
                StartCoroutine(_Use());
            }

            buttonDisable = true;
        });
    }


    // 타겟팅 방식 개선 필요
    public IEnumerator _Use()
    {
        Debug.Log("getTarget");
        BattleManager.Instance.resetTarget();
        Character target = BattleManager.Instance.getTarget();
        while (target == null)
        {
            yield return new WaitForSeconds(0.1f);
            target = BattleManager.Instance.getTarget();
        }

        if (skillType == SkillType.Move)
        {
            Debug.Log("getPosition");
            BattleManager.Instance.resetTargetPosition();
            Vector3 pos = BattleManager.Instance.getPosition();
            // Vector3 none = new Vector3(0, 0, 0);
            // while (pos.x == none.x && pos.y==none.y)
            // {
            //     yield return new WaitForSeconds(0.1f);
            //     pos = BattleManager.Instance.getTarget()
            // }
            // Debug.Log("POSITION:"+pos);
            Use(target, pos);
        }

        Use(target, new Vector3(0, 0, 0));
    }

    public int Use(Character target, Vector3 pos)
    {
        Debug.Log("Use");
        switch (skillType)
        {
            case SkillType.Attack:
                target.getDamage(damage);
                buttonDisable = false;
                return coolTime;
            case SkillType.Move:
                if (pos.x == 0 && pos.y == 0)
                {
                    throw new Exception("Invalid Move Position");
                }

                target.move(pos);
                buttonDisable = false;
                return coolTime;
            case SkillType.Buf:
                target.addBuf(this);
                buttonDisable = false;
                return coolTime;
            case SkillType.DeBuf:
                target.addDebuf(this);
                buttonDisable = false;
                return coolTime;
            default:
                throw new Exception("Skill use error");
        }
    }
}