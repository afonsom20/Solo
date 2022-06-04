using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionText : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "v" + Application.version;
    }
}
