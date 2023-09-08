using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterType
{
    _UNDEFINED,
    character1,
    character2,
    character3,
    character4,
    caharcter5,
    enemy
}

public class Character : MonoBehaviour
{
    public int position = -1;
    public CharacterType type = CharacterType._UNDEFINED;
    [SerializeField]    private int HP = 100;
    [HideInInspector]   public bool isDead = false;
    private Animator animator;
    private List<BufIntensityType> bufList = new();
    // private List<Skill> DeBufList = new List<Skill>();

    public int remainCoolTime = 0;
    public Slider HPStatus;

    private int _initialHP;
    public GameObject status;
    public GameObject BufStatus;

    // 보유한 스킬 리스트
    public List<Skill> Skills = new List<Skill>();
    public Vector3 BufStatusIconScale = new Vector3(3f, 3f, 1); //버프 아이콘 렌더링 시 scale 비율
    public Vector3 BufStatusInitialLocalPosition = new Vector3(-0.3f, 0, 0);
    private float BufStatusXOffset = 0.3f; // 버프 하나당 width

    private void Start()
    {
        //Debug.Log(gameObject.name);
        if (Skills.Count > 8)
        {
            throw new Exception("Skill Count Exceeds 8");
        }

        if (position == -1)
        {
            throw new Exception("Position not initialized");
        }

        _initialHP = HP;
        animator = transform.GetChild(0).GetComponent<Animator>();
        status.GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        status.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        BattleManager.Instance.EnrollCharacter(this);
    }

    public void getDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0) {
            isDead = true;
            animator.SetTrigger("Dead");
        } 
        updateHPBar();
    }
    public void getHeal(int heal)
    {
        HP = Math.Min(HP + heal, _initialHP);
        updateHPBar();
    }

    public void move(Vector3 position, int index)
    {
        //Debug.Log("move >> " + position);
        gameObject.transform.localPosition = position;
        this.position = index;
    }

    public void addBufDebuf(BufType buf, int intensity = 0)
    {
        bool exist = false;
        foreach (var bufSkill in bufList)
        {
            if (bufSkill.bufType.name == buf.name)
            {
                Debug.Log("buf exist");
                exist = true;
                break;
            }
        }

        bufList.Add(new BufIntensityType(buf, intensity));


        if (exist) //기존에 해당 버프가 이미 있었으면
        {
            RefreshBufDebufStatus(buf, intensity);
        }
        else
        {
            AddBufDebufStatus(buf, intensity);
        }
    }


    public void updateHPBar()
    {
        HPStatus.value = (float)HP / _initialHP;
    }

    public void AddBufDebufStatus(BufType buf, int intensity)
    {
        Debug.Log(buf);
        GameObject newBuf = new GameObject("buf_" + buf.name);
        Debug.Log(newBuf);
        var spriteRenderer = newBuf.AddComponent<SpriteRenderer>();
        Debug.Log(spriteRenderer);
        spriteRenderer.sprite = buf.gameObject.GetComponent<SpriteRenderer>().sprite;
        newBuf.transform.parent = BufStatus.transform;
        var totalBufLen = bufList.Count; //+ DeBufList.Count;
        newBuf.transform.localPosition =
            BufStatusInitialLocalPosition + new Vector3(BufStatusXOffset * (totalBufLen - 1), 0, -0.3f);
        newBuf.transform.localScale = BufStatusIconScale;

        // 버프 강도 표시
        var canvasObject = new GameObject("intensity_canvas" + buf.name);
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.SetParent(newBuf.transform);
        var rectTransform = canvasObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(1, 1);
        rectTransform.transform.localPosition = new Vector3(0, 0, 0);

        var intensityText = new GameObject("intensity_buf_" + buf.name);
        var tmpText = intensityText.AddComponent<TextMeshPro>();
        var textRect = intensityText.GetComponent<RectTransform>();
        intensityText.transform.SetParent(canvas.transform);
        textRect.transform.localPosition = new Vector3(0.5f, -0.5f, -1);
        textRect.sizeDelta = new Vector2(1, 1);
        tmpText.fontSize = 3f;
        tmpText.text = intensity.ToString();
    }

    public void RefreshBufDebufStatus(BufType buf, int intensity)
    {
        //TODO Exception handling
        BufIntensityType prevBuf = bufList.Find(_buf => _buf.bufType.name == buf.name);
        prevBuf.intensity += intensity;

        TextMeshPro[] tmpTexts = gameObject.GetComponentsInChildren<TextMeshPro>();
        TextMeshPro tmpText = null;
        foreach (var _tmpText in tmpTexts)
        {
            if (_tmpText.gameObject.name.Contains(buf.name))
            {
                tmpText = _tmpText;
                break;
            }
        }

        if (tmpText == null)
        {
            throw new Exception("Cannot find buf");
        }

        tmpText.text = (int.Parse(tmpText.text) + intensity).ToString();
    }

    public void Pass()
    {
        Debug.Log("ENEMY PASSED");
        BattleManager.Instance.ApplyCoolTime(100);
    }


    /*
     *  Events
     */
    public void OnMouseUp()
    {
        Debug.Log("character onclick");
        BattleManager.Instance.setTarget(this);
    }
}