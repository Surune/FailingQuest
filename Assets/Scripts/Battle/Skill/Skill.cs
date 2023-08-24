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

            Use(BattleManager.Instance.getCurrent(), locationHelper.GetPosition(), locationHelper.Index);
            yield break;
        }

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

        Use(target, new Vector3(0, 0, 0), 0);
    }

    public void Use(Character target, Vector3 pos, int posIndex)
    {
        Debug.Log("Use");
        switch (skillType)
        {
            case SkillType.Attack:
                target.getDamage(damage);
                break;
            case SkillType.Move:
                ExchangePosition(target, pos, posIndex);
                break;
            case SkillType.Buf:
                target.addBuf(this);
                break;
            case SkillType.DeBuf:
                target.addDebuf(this);
                break;
            default:
                throw new Exception("Skill use error");
        }

        buttonDisable = false;
        BattleManager.Instance.ApplyCoolTime(coolTime);
    }

    public void ExchangePosition(Character target, Vector3 pos, int posIndex)
    {
        bool find = false;
        foreach (var character in BattleManager.Instance.GetCharacterList())
        {
            if (character.position == posIndex)
            {
                var prevPosition = character.transform.localPosition;
                var prevPosIndex = character.position;
                character.move(target.transform.localPosition, target.position);
                target.move(prevPosition, prevPosIndex);
                find = true;
                break;
            }
        }

        if (!find)
        {
            target.move(pos, posIndex);
        }
    }
}