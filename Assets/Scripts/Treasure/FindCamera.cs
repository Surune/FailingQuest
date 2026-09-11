using UnityEngine;

public class FindCamera : MonoBehaviour
{
    void Start() => GetComponent<Canvas>().worldCamera = Camera.main;
}
