using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExpeditionManager : MonoBehaviour
{
    public Expeditions[] Expeditions;
    public Loot[] Loot;

    [Header("UI")]
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject handwrittenTextPrefab;
    [SerializeField] Transform expeditionRecapTextHolder;

    [Space, Header("Variables")]
    [SerializeField] int hopeIncreaseBase = 5;
    [SerializeField] int lootChanceIncreaseFactor = 4;

    int danger;
    int lootLevel;
    int fightChance;

    public void GoOnExpedition(int index)
    {
        danger = Expeditions[index].Danger;
        lootLevel = Expeditions[index].LootLevel;
        fightChance = Expeditions[index].FightEncounterChance;

        // Reset the expedition board
        foreach (Transform child in expeditionRecapTextHolder)
            Destroy(child.gameObject);
        

        GetLoot();

        if (Random.Range(0, 100) <= fightChance)
        {
            Fight();
        }

        TimeManager.Instance.AdvancePhase();

        StartCoroutine(DelayedChanges());

    }

    void UpdateRecapBoard(string lootName, int amount)
    {
        GameObject newText = Instantiate(handwrittenTextPrefab, expeditionRecapTextHolder);
        newText.GetComponent<TextMeshProUGUI>().text = "- " + amount + "x " + lootName;
    }

    IEnumerator DelayedChanges()
    {
        if (TimeManager.Instance.CurrentPhase == TimeManager.DayPhase.Night)
            yield return new WaitForSeconds(1.5f);
        else
            yield return new WaitForSeconds(4f);

        int finalHopeIncrease = hopeIncreaseBase + lootLevel + danger;
        PlayerStatusManager.Instance.IncreaseHope(finalHopeIncrease);
        CanvasManager.Instance.ToggleExpeditionRecapBoard();
    }

    void GetLoot()
    {
        // Each expedition has “Danger” and “Loot Level” values associated to it. The higher these are, the higher the chance to find each loot item. 
        int chanceIncreaseFactor = lootChanceIncreaseFactor * (lootLevel + danger);

        // For every Loot possible...
        for (int i = 0; i < Loot.Length; i++)
        {            
            // ... "roll the dice" to see if it will be found on this expedition
            // Note that the an expedition with a higher loot and danger levels will increase the chances of finding that loot item
            if (Loot[i].ChanceToFind + chanceIncreaseFactor >= Random.Range(0, 100))
            {
                // ... and, if so, add it to the player's inventory...
                // ... but not before randomizing the amount of loot found
                int lootAmount = Random.Range(Loot[i].AmountMin, Loot[i].AmountMax + lootLevel);
                inventory.AddLoot(Loot[i], lootAmount);

                // Update the recap board UI to show the 
                UpdateRecapBoard(Loot[i].Name, lootAmount);
            }

        }

    }

    void Fight()
    {
        Debug.Log("A fight occurred!");
    }
}
