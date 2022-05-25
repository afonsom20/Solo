using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivityBoard : MonoBehaviour
{
    [SerializeField] GameObject activityButtonPrefab;
    [SerializeField] Transform activityTextHolder;
    [SerializeField] GameObject activityDetailsHolder;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject[] dangerIcons;
    [SerializeField] GameObject[] lootIcons;
    [SerializeField] ExpeditionManager expeditionManager;

    int selectedActivity;

    string currentDescription;
    int currentDanger;
    int currentLootLevel;

    void Start()
    {
        UpdateActivityBoard();
    }

    public void DoActivity()
    {
        Debug.Log("Do activity");
    }

    void UpdateActivityBoard()
    {
        // Loop through every expedition/activity
        for (int i = 0; i < expeditionManager.Expeditions.Length; i++)
        {
            // Create a text button for it
            GameObject newButton = Instantiate(activityButtonPrefab, activityTextHolder);

            // Give it the  name
            newButton.GetComponent<TextMeshProUGUI>().text = expeditionManager.Expeditions[i].Name;

            currentDescription = expeditionManager.Expeditions[i].Description;
            currentDanger = expeditionManager.Expeditions[i].Danger;
            currentLootLevel = expeditionManager.Expeditions[i].LootLevel;
            // Make it so the button activates the description with the specific values for this expedition/activity
            newButton.GetComponent<Button>().onClick.AddListener(delegate {
                ShowActivityDescription(i);
            });
        }
    }

    void ShowActivityDescription(int index)
    {
        selectedActivity = index;

        descriptionText.text = currentDescription;

        for (int i = 0; i < lootIcons.Length; i++)
        {
            if (i == currentLootLevel)
                lootIcons[i].SetActive(true);
            else
                lootIcons[i].SetActive(false);
            
            if (i == currentDanger)
                dangerIcons[i].SetActive(true);
            else
                dangerIcons[i].SetActive(false);
        }

        activityTextHolder.gameObject.SetActive(false);
        activityDetailsHolder.SetActive(true);
    }
}
