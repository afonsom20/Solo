using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    public float modifier;

    Vector3 _pos;
    Vector3 _startPos;
    Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        _startPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        _pos = _cam.ScreenToViewportPoint(Input.mousePosition);
        _pos.z = 0;
        transform.position = _pos;
        transform.position = new Vector3(_startPos.x + (_pos.x * modifier), _startPos.y + (_pos.y * modifier), 0f);
    }
}
