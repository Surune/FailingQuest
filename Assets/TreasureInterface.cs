using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureInterface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GameManager.Instance.userData.myTreasure);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
