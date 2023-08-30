using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public Button[] Characters;
    public Button[] Selects;
    public Button startButton;
    private int max_selection = 3;
    private int current_selection = 0;
    private int[] selectedIndices;

    private void Start() 
    {
        selectedIndices = new int[max_selection];
        for(int i = 0 ; i < max_selection; i++){
            selectedIndices[i] = -1;
        }
        startButton.interactable = false;
    }

    public void Select(int charNum)
    {
        if (current_selection < max_selection)
        {
            for(int i = 0 ; i < max_selection; i++)
            {
                if(selectedIndices[i] == -1)
                {
                    selectedIndices[i] = charNum;
                    Selects[i].gameObject.GetComponent<Image>().sprite = Characters[charNum-1].gameObject.GetComponent<Image>().sprite;
                    current_selection += 1;
                    break;
                }
            }
            Characters[charNum-1].interactable = false;
            if(current_selection == max_selection)
            {
                startButton.interactable = true;
            }
        }
        Debug.Log(selectedIndices[0].ToString()+""+selectedIndices[1]+""+selectedIndices[2]);
    }

    public void Cancel(int index)
    {
        if(selectedIndices[index] != -1) {
            Characters[selectedIndices[index]-1].interactable = true;
            selectedIndices[index] = -1;
            Selects[index].gameObject.GetComponent<Image>().sprite = null;
            current_selection -= 1;
            startButton.interactable = false;
        }
        Debug.Log(selectedIndices[0].ToString()+""+selectedIndices[1]+""+selectedIndices[2]);
    }

    public void SetTeam()
    {
        foreach(int index in selectedIndices)
        {
            GameManager.Instance.characters.Add(index);
        }
    }
}
