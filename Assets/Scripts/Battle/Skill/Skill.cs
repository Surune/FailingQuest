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

    protected ForgeType forgeType = ForgeType.UNFORGED;
    [SerializeField] protected ForgeType[] forgeAvailableType;

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


    public IEnumerator _Use()
    {
        Debug.Log("getTarget");
        BattleManager.Instance.resetTarget();
        BattleManager.Instance.HandleLocationCollider(false);
        Character target = BattleManager.Instance.getTarget();
        while (target == null)
        {
            yield return new WaitForSeconds(0.1f);
            target = BattleManager.Instance.getTarget();
        }

        BattleManager.Instance.HandleLocationCollider(true);
        if (skillType == SkillType.Move)
        {
            Debug.Log("getPosition");
            BattleManager.Instance.resetTargetPosition();
            LocationHelper locationHelper = BattleManager.Instance.getTargetPosition();
            while (locationHelper == null)
            {
                yield return new WaitForSeconds(0.1f);
                locationHelper = BattleManager.Instance.getTargetPosition();
            }

            Use(target, locationHelper.GetPosition());
        }
        else
        {
            Use(target, new Vector3(0, 0, 0));
        }
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