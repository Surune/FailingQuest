using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour //전투의 진행을 담당
{
    private Character target = null; // 스킬 타겟
    private LocationHelper location = null; // 이동 스킬 타겟 위치

    public static BattleManager Instance;

    [SerializeField] private List<LocationHelper> locations = new List<LocationHelper>(); // 타겟팅시 collider 잠깐 비활성화 위해

    private List<Character> CharacterList = new List<Character>();
    private Character current; //현재 차례
    public GameObject CurrentTag; //현재 캐릭터 위의 표시
    public SpriteRenderer BottomCurrentSprite; //아래에 있는 현재 캐릭터
    public Slider BottomCurrentHP; //아래에 있는 현재 캐릭터
    public GameObject BottomCurrentBuf; //아래에 있는 현재 캐릭터
    private List<GameObject> BottomCurrentTempBuf = new(); // bottom 영역 현재 캐릭터 버프영역

    public Transform coolTimeInitPosition; //속도 표기 기준위치
    public Transform coolTimeEndPosition; //속도 표기 기준위치
    private int maxRemainCooltime;


    [HideInInspector] public List<Dictionary<string, object>> skillInfo;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillCooltimeText;
    public TextMeshProUGUI skillDescriptionText;

    public string prevClickSkillName; // 버튼 두번 클릭시에 스킬 사용되도록
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    void Start()
    {
        FindMinCoolTime();
        skillInfo = CSVReader.Read("SkillInfo");
    }


    public void FindMinCoolTime()
    {
        Character nextCharacter = CharacterList[0];
        foreach (var character in CharacterList)
        {
            if (character.remainCoolTime < nextCharacter.remainCoolTime)
            {
                nextCharacter = character;
            }
            else if (character.remainCoolTime == nextCharacter.remainCoolTime)
            {
                if (character.type == CharacterType.enemy) // 적이 우선순위
                {
                    nextCharacter = character.position > nextCharacter.position ? character : nextCharacter;
                }
                else if (nextCharacter.type != CharacterType.enemy)
                {
                    //바깥에 있는 캐릭터 우선
                    nextCharacter = character.position < nextCharacter.position ? character : nextCharacter;
                }
            }
        }

        current = nextCharacter;
        var coolTimeOffset = current.remainCoolTime;
        foreach (var character in CharacterList)
        {
            character.remainCoolTime -= coolTimeOffset;
            if (character.remainCoolTime > maxRemainCooltime)
            {
                maxRemainCooltime = character.remainCoolTime;
            }
        }

        UpdateCurrentTag();
        UpdateCoolTimeStatus();
        UpdateBottomCurrentCharacter();
    }

    public void EnrollCharacter(Character character)
    {
        if (character.type == CharacterType._UNDEFINED)
        {
            throw new Exception("Character Type Undefined");
        }

        CharacterList.Add(character);
    }

    public List<Character> GetCharacterList()
    {
        return CharacterList;
    }

    /*
     * 스킬 사용 직후 호출, 현재 스킬 사용한 캐릭터에 쿨타임 적용
     * 다음 차례의 캐릭터를 찾음
     */
    public void ApplyCoolTime(int coolTime)
    {
        current.remainCoolTime = coolTime;
        FindMinCoolTime();
    }


    /*
     *  SKill Methods
     */
    public Character getCurrent()
    {
        return current;
    }

    public Character getTarget(TargetType specificTarget = TargetType._UNDEFINED)
    {
        if (specificTarget == TargetType.ally1)
        {
            if (target != null && target.type != CharacterType.enemy)
            {
                return target;
            }

            return null;
        }

        if (specificTarget == TargetType.enemy1)
        {
            if (target != null && target.type == CharacterType.enemy)
            {
                return target;
            }

            return null;
        }

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

    public LocationHelper getTargetPosition()
    {
        return location;
    }

    public void setTargetPosition(LocationHelper locationHelper)
    {
        location = locationHelper;
    }

    public void resetTargetPosition()
    {
        location = null;
    }

    public void UpdateCurrentTag()
    {
        switch (current.position)
        {
            case 1:
                CurrentTag.transform.localPosition = new Vector3(-6.73f, 1.57f, 0);
                break;
            case 2:
                CurrentTag.transform.localPosition = new Vector3(-4.12f, 1.57f, 0);
                break;
            case 3:
                CurrentTag.transform.localPosition = new Vector3(-1.55f, 1.57f, 0);
                break;
            case 4:
                CurrentTag.transform.localPosition = new Vector3(2.18f, 1.57f, 0);
                break;
            case 5:
                CurrentTag.transform.localPosition = new Vector3(4.9f, 1.57f, 0);
                break;
            case 6:
                CurrentTag.transform.localPosition = new Vector3(7.22f, 1.57f, 0);
                break;
        }
    }

    public void UpdateCoolTimeStatus()
    {
        var xOffset = (coolTimeEndPosition.position - coolTimeInitPosition.position).x;

        foreach (var character in CharacterList)
        {
            var Status = character.transform.GetChild(1).gameObject;
            Status.transform.position = coolTimeInitPosition.position +
                                        new Vector3(
                                            xOffset * character.remainCoolTime /
                                            (maxRemainCooltime == 0 ? 100 : maxRemainCooltime), 0, 0);
        }
    }

    public void UpdateBottomCurrentCharacter()
    {
        foreach (var prevBuf in BottomCurrentTempBuf)
        {
            Destroy(prevBuf);
        }
        BottomCurrentTempBuf = new();
        BottomCurrentSprite.sprite = current.GetComponentInChildren<SpriteRenderer>().sprite;
        BottomCurrentHP.value = current.GetComponentInChildren<Slider>().value;
        foreach (var bufCanvas in current.BufStatus.GetComponentsInChildren<Canvas>())
        {
            var buf = bufCanvas.transform.parent.gameObject;
            var instance = Instantiate(buf, BottomCurrentBuf.transform);
            BottomCurrentTempBuf.Add(instance);
            instance.transform.localPosition = buf.transform.localPosition;
        }
    }

    public List<Character> GetAllies()
    {
        List<Character> allies = new();
        foreach (var character in CharacterList)
        {
            if (character.type != CharacterType.enemy)
            {
                allies.Add(character);
            }
        }

        return allies;
    }

    public List<Character> GetEnemies()
    {
        List<Character> allies = new();
        foreach (var character in CharacterList)
        {
            if (character.type == CharacterType.enemy)
            {
                allies.Add(character);
            }
        }

        return allies;
    }

    public List<Character> GetAllCharacter()
    {
        return CharacterList;
    }


    /*
     * Skill 사용 시 캐릭터 타겟팅 도중에는 location collider 비활성화
     */
    public void HandleLocationCollider(bool enable)
    {
        foreach (var locationHelper in locations)
        {
            Debug.Log(locationHelper);
            locationHelper.gameObject.SetActive(enable);
        }
    }

    public void SetSkillText(int skillNum)
    {
        var row = CSVReader.FindRowWithNum(skillInfo, skillNum);
        skillNameText.text = row["NAME"].ToString();
        skillCooltimeText.text = row["COOLTIME"].ToString();
        skillDescriptionText.text = row["DESCRIPTION"].ToString();
    }
}