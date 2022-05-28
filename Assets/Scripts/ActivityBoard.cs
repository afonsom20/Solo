using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityBoard : MonoBehaviour
{
    public static ActivityBoard Instance { get; set; }

    [SerializeField] GameObject activityButtonPrefab;
    [SerializeField] Transform activityTextHolder;
    [SerializeField] GameObject activityDetailsHolder;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject[] dangerIcons;
    [SerializeField] GameObject[] lootIcons;
    [SerializeField] ExpeditionManager expeditionManager;

    string[] descriptions;
    int[] dangers;
    int[] lootLevels;

    void Start()
    {
        Instance = this;

        SetUpActivityBoard();
    }

    public void DoActivity()
    {
        Debug.Log("Do activity");
    }

    void SetUpActivityBoard()
    {
        descriptions = new string[expeditionManager.Expeditions.Length];
        dangers = new int[expeditionManager.Expeditions.Length];
        lootLevels = new int[expeditionManager.Expeditions.Length];

        // Loop through every expedition/activity
        for (int i = 0; i < expeditionManager.Expeditions.Length; i++)
        {
            // Create a text button for it
            GameObject newButton = Instantiate(activityButtonPrefab, activityTextHolder);

            // Give it the  name
            newButton.GetComponent<TextMeshProUGUI>().text = expeditionManager.Expeditions[i].Name;

            descriptions[i] = expeditionManager.Expeditions[i].Description;
            dangers[i] = expeditionManager.Expeditions[i].Danger;
            lootLevels[i] = expeditionManager.Expeditions[i].LootLevel;
        }
    }

    public void ShowActivityDescription(int index)
    {
        AudioManager.Instance.Play("ExpeditionDetails");

        descriptionText.text = descriptions[index];

        for (int i = 0; i < lootIcons.Length; i++)
        {
            if (i == lootLevels[index])
                lootIcons[i].SetActive(true);
            else
                lootIcons[i].SetActive(false);
            
            if (i == dangers[index])
                dangerIcons[i].SetActive(true);
            else
                dangerIcons[i].SetActive(false);
        }

        activityTextHolder.gameObject.SetActive(false);
        activityDetailsHolder.SetActive(true);
    
    }

    public void CloseActivityBoard()
    {
        activityDetailsHolder.SetActive(false);
        activityTextHolder.gameObject.SetActive(true);

        gameObject.SetActive(false);
    }
}
