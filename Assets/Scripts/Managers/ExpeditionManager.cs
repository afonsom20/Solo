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
    //[SerializeField] int lootChanceIncreaseFactor = 4;
    [SerializeField] int minimumWoundChance = 40;
    [SerializeField] int minimumFightHealthDecrease = 10;
    [SerializeField] int fightHealthDecreaseDeviation = 5;

    int danger;
    int lootLevel;
    int fightChance;

    public void GoOnExpedition()
    {
        // Before going on an expedition, the player must see its details
        // When this happens, the ActivityBoard registers the index of the currently selected expedition
        // Therefore, if the player decides to go on this one, this script knows what the correct index is
        int index = ActivityBoard.Instance.SelectedActivity;

        danger = Expeditions[index].Danger;
        lootLevel = Expeditions[index].LootLevel;
        fightChance = Expeditions[index].FightEncounterChance;

        AudioManager.Instance.Play("Expedition");
        RaidManager.Instance.UpdateVariables(false, false);

        // Reset the expedition board
        foreach (Transform child in expeditionRecapTextHolder)
            Destroy(child.gameObject);
        

        StartCoroutine(GetLoot());

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
            yield return new WaitForSeconds(TimeManager.Instance.DayNightTransitionTime);
        else
            yield return new WaitForSeconds(TimeManager.Instance.NightDayTransitionTime + 0.5f);

        int finalHopeIncrease = hopeIncreaseBase + lootLevel + danger;
        PlayerStatusManager.Instance.IncreaseHope(finalHopeIncrease);
        CanvasManager.Instance.ToggleExpeditionRecapBoard();
    }

    IEnumerator GetLoot()
    {
        // Change into the night
        if (TimeManager.Instance.CurrentPhase == TimeManager.DayPhase.Night)
            yield return new WaitForSeconds(0.1f + TimeManager.Instance.NightDayTransitionTime);

        // Each expedition has “Danger” and “Loot Level” values associated to it. The higher these are, the higher the chance to find each loot item. 
        //int chanceIncreaseFactor = lootChanceIncreaseFactor * lootLevel;

        // For every Loot possible...
        for (int i = 0; i < Loot.Length; i++)
        {
            // ... check if this loot can be found on this expedition's loot level
            // If not, skip this item
            if (Loot[i].MinimumLevel > lootLevel)
                continue;

            // If so, "roll the dice" to see if it will be found on this expedition
            // Note that the an expedition with a higher loot and danger levels will increase the chances of finding that loot item
            if (Loot[i].ChanceToFind >= Random.Range(0, 100))
            {                
                // ... and, if so, add it to the player's inventory...
                // ... but not before randomizing the amount of loot found, if the loot does not have FixedAmount = true
                int lootAmount = Random.Range(Loot[i].AmountFoundMin, Loot[i].AmountFoundMax + 1 + lootLevel); // +1 because Random.Range is (minInclusive, maxExclusive)
                
                // However, if the loot has a fix amount found (e.g. only 1 First Aid Kit can be found per expedition), change the value
                if (Loot[i].OnlyOneFoundPerExpedition)
                    lootAmount = 1;

                if (inventory.CheckIfItemMaxAmountReached(Loot[i]))
                    yield break;                
                          

                inventory.AddLoot(Loot[i], lootAmount);

                // Update the recap board UI to show the 
                UpdateRecapBoard(Loot[i].Name, lootAmount);
            }

        }

    }

    void Fight()
    {
        // Calculate factors for decreasing the player's Health
        int deviationFactor = Random.Range(0, 2) * 2 - 1; // returns -1 or 1
        int damage = (minimumFightHealthDecrease * danger) + (deviationFactor * fightHealthDecreaseDeviation);

        // If this fight would kill the player, just lower their health to 5
        if (PlayerStatusManager.Instance.Health - damage <= 0)
            PlayerStatusManager.Instance.Health = 5;
        // If not, simply decrease the player's Health
        else
            PlayerStatusManager.Instance.Health -= damage;

        
        // Send info about the fight
        StartCoroutine(InformationManager.Instance.SendDelayedInfo(4, "You got ambushed while exploring and had to fight."));
        
        // Check if the player got wounded during the fight
        if (!PlayerStatusManager.Instance.Wounded)
        {
            int woundChance = minimumWoundChance + (10 * danger);

            if (Random.Range(0, 100) <= woundChance)
            {
                PlayerStatusManager.Instance.Wounded = true;
                StartCoroutine(PlayerStatusManager.Instance.ToggleWoundIndicatorWithDelay());
                StartCoroutine(InformationManager.Instance.SendDelayedInfo(3, "You got wounded in a fight. Treat it with a First Aid Kit"));
            }
        }

    }
}
