using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHelper : MonoBehaviour
{
    public BattleManager battleManager;

    public void OnMouseUp()
    {
        battleManager.setTargetPosition(gameObject.transform.position);
    }
}