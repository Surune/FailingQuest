using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour //전투의 진행을 담당
{
    private int time;
    private Character target = null; // 스킬 타겟
    private LocationHelper location = null; // 이동 스킬 타겟 위치

    public static BattleManager Instance;

    [SerializeField] private List<LocationHelper> locations = new List<LocationHelper>(); // 타겟팅시 collider 잠깐 비활성화 위해

    private List<Character> CharacterList=new List<Character>();
    private Character current; //현재 차례
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
        Reset();
    }

    public void Reset()
    {
        time = 0;
    }

    public int FindMinCoolTime()
    {
        throw new NotImplementedException();
    }

    public void EnrollCharacter(Character character){
        CharacterList.Add(character)
    }

    public void ApplyCoolTime(int coolTime){
        current.remainCoolTime=coolTime;
    }


    /*
     *  SKill Methods
     */
    public Character getTarget()
    {
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
}