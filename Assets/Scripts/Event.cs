using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//event 조건 enable 되면 발동 
public class Event : MonoBehaviour

{ 
    public Text typeText;
    public Text eventText;
    public Text choiceText;
    public Button choicePrefabs;
    private Button[] choiceButtons;
    private Vector2 buttonPosition = new Vector2(0, -30);
    

    private string[] choiceArray = new string[10] { "보물획득", "전투", "아무 일 없음", "코인획득", "체력감소", "코인지불", "스킬삭제", "스킬획득", "스킬강화", "체력회복" };
    private string[,] eventArray = new string[11, 2] {
            {"수상한 상자를 발견했다. 열어볼까?", "012"},
            {"적을 마주쳤다.", "1" },
            {"누군가의 해골과 함께 보물이 놓여있다.", "0" },
            {"누군가의 해골과 함께 금화가 놓여있다.", "3" },
            {"아무래도 제물을 바쳐야 지나갈 수 있는 모양이다.", "456" },
            {"함정이다!", "4" },
            {"돈을 주면 무언가 받을 수 있을 것 같다.", "50" },
            {"어둠이 우리를 감쌌다.", "6" },
            {"전투를 대비하기 위해 열심히 훈련했다.", "78" },
            {"무얼 강화해줄까?", "8"},
            {"편안히 휴식을 취했다.", "9" },
            };

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            int num = (int)Random.Range(0, 11);
            typeText.text = "이벤트 제목";
            eventText.text = eventArray[num, 0];
            
            string choices = eventArray[num, 1];
            buttonPosition = eventText.transform.position;
            buttonPosition.y -= 60;
            choiceButtons = new Button[choices.Length];
            for (int idx = 0; idx < choices.Length; idx++)
            {
                choiceButtons[idx] = Instantiate(choicePrefabs, buttonPosition, Quaternion.identity);
                choiceButtons[idx].transform.SetParent(eventText.transform.parent); // Set the parent of the button
                Text choiceButtonText = choiceButtons[idx].GetComponentInChildren<Text>();
                choiceButtonText.text = choiceArray[choices[idx] - '0'];
                choiceButtons[idx].gameObject.SetActive(true);
                buttonPosition.y -= 60;
            }
        }
        
    }
}
