using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationManager : MonoBehaviour
{
    public static InformationManager Instance { get; set; }

    [SerializeField] GameObject infoPrefab;
    [SerializeField] Transform infoHolder;

    [Header("Info type customization"), Space]
    [SerializeField, 
        Tooltip("0 - info, " +
        "1 - alert, " +
        "2 - sickness, " +
        "3 - wound, " +
        "4 - fight, " +
        "5 - visitor" + 
        "6 - warning")] Sprite[] icons; 
    [SerializeField] Color[] iconColors;
    [SerializeField] Sprite[] panels;

    void Awake()
    {
        Instance = this;        
    }

    /// <summary>
    /// Function to send info to the player
    /// </summary>
    /// <param name="infoType">0 - info, 1 - alert, 2 - sickness, 3 - wound, 4 - fight, 5 - visitor, 6 - warning</param>
    public void SendInfo(int infoType, string message)
    {
        //Debug.Log("Message: " + infoType + ", " + message);

        GameObject newInfo = Instantiate(infoPrefab, infoHolder);

        // Set the correct panel
        //newInfo.GetComponent<Image>().sprite = panels[infoType];

        // Set the correct icon and icon color; as well as the panel color
        Image icon = newInfo.transform.GetChild(0).gameObject.GetComponent<Image>();
        icon.sprite = icons[infoType];
        icon.color = iconColors[infoType];
        newInfo.GetComponent<Image>().color = iconColors[infoType];

        // Set the correct message
        newInfo.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = message;

        // Set the correct color for the close button
        newInfo.transform.GetChild(2).gameObject.GetComponent<Image>().color = iconColors[infoType];

        // Start the "Appear" animation
        newInfo.GetComponent<Animator>().SetTrigger("Appear");
    }

    public IEnumerator SendDelayedInfo(int infoType, string message)
    {
        // ATTENTION - due to script/method execution orders, this is different from
        // the DelayedChanges method present on the TimeManager
        // Here, this first part means the change into the DAY
        if (TimeManager.Instance.CurrentPhase == TimeManager.DayPhase.Night)
            yield return new WaitForSeconds(TimeManager.Instance.NightDayTransitionTime + 0.5f);
        else
            yield return new WaitForSeconds(TimeManager.Instance.DayNightTransitionTime);

        SendInfo(infoType, message);
    }
}
