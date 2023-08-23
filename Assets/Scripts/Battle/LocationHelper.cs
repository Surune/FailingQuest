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
        return _location;
        //+ new Vector3(0, 0, 0.2f); //character collider는 location collider보다 z index가 더 크기 때문에 여기서 유지해줌
    }

    public void OnMouseUp()
    {
        Debug.Log("location helper onclick"+transform.localPosition);
        BattleManager.Instance.setTargetPosition(this);
    }
}