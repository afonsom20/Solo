using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationManager : MonoBehaviour
{
    public static InformationManager Instance { get; set; }

    [SerializeField] GameObject[] icons; // 0 - info, 1 - alert, 2 - sickness, 3 - visitor
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] Animator infoAnimator;

    void Awake()
    {
        Instance = this;        
    }

    /// <summary>
    /// Function to send info to the player
    /// </summary>
    /// <param name="infoType">0 - info, 1 - alert, 2 - sickness, 3 - visitor</param>
    public void SendInfo(int infoType, string message)
    {
        // Choose the correct icon
        for (int i = 0; i < icons.Length; i++)
        {
            if (i == infoType)
                icons[i].SetActive(true);
            else
                icons[i].SetActive(false);
        }

        infoText.text = message;

        infoAnimator.SetTrigger("Appear");
    }
}
