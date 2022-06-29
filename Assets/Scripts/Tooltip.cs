using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; set; }

    [SerializeField] GameObject graphics;
    [SerializeField] TextMeshProUGUI text;

    void Start()
    {
        Instance = this;    
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void Activate(string descriptionText)
    {
        text.text = descriptionText;

        graphics.SetActive(true);
    }

    public void Deactivate()
    {
        graphics.SetActive(false);
    }
}
