using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    [SerializeField] private int skillNumber = -1;
    [SerializeField] protected int coolTime = -1;
    [SerializeField] protected SkillType skillType = SkillType._UNDEFINED;
    [SerializeField] protected SkillType skillType2 = SkillType._UNDEFINED;
    // public string name = "Skill Name";
    // public string description = "Description";
    [SerializeField] private int damage = 0; // attack skill
    [SerializeField] private BufType buf1; // buf/debuf1
    [SerializeField] private BufType buf2; // buf/debuf2
    [SerializeField] private int buf1Intensity = 0;
    [SerializeField] private int buf2Intensity = 0;
    [SerializeField] private TargetType targetType1;
    [SerializeField] private TargetType targetType2;


    protected ForgeType forgeType = ForgeType.UNFORGED;

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
                buttonDisable = true;
                if (skillType2 != SkillType._UNDEFINED)
                {
                    StartCoroutine(_UseMulti());
                }
                else
                {
                    StartCoroutine(_Use());
                }
            }
        });
        button.onClick.AddListener(() => BattleManager.Instance.SetSkillText(skillNumber));
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


        switch (targetType1)
        {
            case TargetType._UNDEFINED:
                throw new Exception("target type undefined");
            case TargetType.ally1:
            case TargetType.enemy1:
                Debug.Log("getTarget");
                BattleManager.Instance.resetTarget();
                BattleManager.Instance.HandleLocationCollider(false);

                Character target = BattleManager.Instance.getTarget(targetType1);
                while (target == null)
                {
                    yield return new WaitForSeconds(0.1f);
                    target = BattleManager.Instance.getTarget(targetType1);
                }

                BattleManager.Instance.HandleLocationCollider(true);

                Use(target, new Vector3(0, 0, 0), 0);
                break;
            case TargetType.all:
                var all = BattleManager.Instance.GetAllCharacter();
                foreach (var character in all)
                {
                    Use(character, new Vector3(0, 0, 0), 0, false);
                }

                BattleManager.Instance.ApplyCoolTime(coolTime);
                break;
            case TargetType.allyAll:
                var allies = BattleManager.Instance.GetAllies();
                foreach (var character in allies)
                {
                    Use(character, new Vector3(0, 0, 0), 0, false);
                }

                BattleManager.Instance.ApplyCoolTime(coolTime);
                break;
            case TargetType.enemyAll:
                var enemies = BattleManager.Instance.GetEnemies();
                foreach (var character in enemies)
                {
                    Use(character, new Vector3(0, 0, 0), 0, false);
                }

                BattleManager.Instance.ApplyCoolTime(coolTime);
                break;
            case TargetType.self:
                Use(BattleManager.Instance.getCurrent(), new Vector3(0, 0, 0), 0);
                break;
        }
    }

    public IEnumerator _UseMulti()
    {
        // 이동 + 다른 효과가 있는 경우 자신에게 거는 효과만 있는 현재 상황 가정
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

            Use(BattleManager.Instance.getCurrent(), locationHelper.GetPosition(), locationHelper.Index, false, 0);
            Use(BattleManager.Instance.getCurrent(), locationHelper.GetPosition(), locationHelper.Index, true, 1);
            yield break;
        }

        // 타겟팅 필요한 경우 반드시 SkillType 에 넣어야 함 (targetType1에 대응)
        switch (targetType1)
        {
            case TargetType._UNDEFINED:
                throw new Exception("target type undefined");
            //need targeting
            case TargetType.ally1:
            case TargetType.enemy1:
                Debug.Log("getTarget");
                BattleManager.Instance.resetTarget();
                BattleManager.Instance.HandleLocationCollider(false);

                Character target = BattleManager.Instance.getTarget(targetType1);
                while (target == null)
                {
                    yield return new WaitForSeconds(0.1f);
                    target = BattleManager.Instance.getTarget(targetType1);
                }

                BattleManager.Instance.HandleLocationCollider(true);
                Use(target, new Vector3(0, 0, 0), 0, false, 0);
                Use(target, new Vector3(0, 0, 0), 0, true, 1);
                break;
            case TargetType.all:
                var all = BattleManager.Instance.GetAllCharacter();
                foreach (var character in all)
                {
                    Use(character, new Vector3(0, 0, 0), 0, false, 0);
                    Use(character, new Vector3(0, 0, 0), 0, false, 1);
                }

                BattleManager.Instance.ApplyCoolTime(coolTime);
                break;
            case TargetType.allyAll:
                var allies = BattleManager.Instance.GetAllies();
                foreach (var character in allies)
                {
                    Use(character, new Vector3(0, 0, 0), 0, false, 0);
                    Use(character, new Vector3(0, 0, 0), 0, false, 1);
                }

                BattleManager.Instance.ApplyCoolTime(coolTime);
                break;
            case TargetType.enemyAll:
                var enemies = BattleManager.Instance.GetEnemies();
                foreach (var character in enemies)
                {
                    Use(character, new Vector3(0, 0, 0), 0, false, 0);
                    Use(character, new Vector3(0, 0, 0), 0, false, 1);
                }

                BattleManager.Instance.ApplyCoolTime(coolTime);
                break;
            case TargetType.self:
                Use(BattleManager.Instance.getCurrent(), new Vector3(0, 0, 0), 0, false, 0);
                Use(BattleManager.Instance.getCurrent(), new Vector3(0, 0, 0), 0, true, 1);
                break;
        }
    }

    public void Use(Character target, Vector3 pos, int posIndex, bool applyCoolTime = true, int skillIndex = 0)
    {
        Debug.Log("Use");
        var _skillType = skillIndex == 0 ? skillType : skillType2;
        switch (_skillType)
        {
            case SkillType.Attack:
                target.getDamage(damage);
                break;
            case SkillType.Move:
                ExchangePosition(target, pos, posIndex);
                break;
            case SkillType.Buf:
            case SkillType.DeBuf:
                target.addBufDebuf(skillIndex == 0 ? buf1 : buf2, skillIndex == 0 ? buf1Intensity : buf2Intensity);
                break;
            case SkillType.custom:
                throw new NotImplementedException("Custom Skills");
            default:
                throw new Exception("Undefined skill type");
        }

        if (applyCoolTime)
        {
            Debug.Log("buttonDisable=false, applycoolTime:" + coolTime);
            buttonDisable = false;
            BattleManager.Instance.ApplyCoolTime(coolTime);
        }
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