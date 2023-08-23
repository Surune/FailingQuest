using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHelper : MonoBehaviour
{
    public int Index = 1;
    private Vector3 _location;

    private void Start()
    {
        _location = gameObject.transform.localPosition;
        Debug.Log(_location);
    }

    public Vector3 GetPosition()
    {
        return _location + new Vector3(0, 0, 0.2f);
    }

    public void OnMouseUp()
    {
        Debug.Log("location helper onclick" + transform.localPosition);
        BattleManager.Instance.setTargetPosition(this);
    }
}