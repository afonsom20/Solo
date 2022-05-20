using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionManager : MonoBehaviour
{
    public Expeditions[] Expeditions;
    public Loot[] Loot;

    [SerializeField] Inventory inventory;
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

        GetLoot(lootLevel);

        if (Random.Range(0, 100) <= fightChance)
        {
            Fight();
        }

        PlayerStatusManager.Instance.IncreaseHope(hopeIncreaseBase + lootLevel + danger);
        TimeManager.Instance.AdvancePhase();

        int finalHopeIncrease = hopeIncreaseBase + lootLevel + danger;
        PlayerStatusManager.Instance.IncreaseHope(finalHopeIncrease);
        Debug.Log("Completed expedition. Increased hope by " + finalHopeIncrease);

    }

    void GetLoot(int expeditionLootLevel)
    {
        // Create a list where we'll store the loot that is possible to get in the current expedition
        //List<Loot> lootFound = new List<Loot>();

        int chanceIncreaseFactor = lootChanceIncreaseFactor * lootLevel + danger;
        Debug.Log("Chance Increase Factor = " + chanceIncreaseFactor);

        // For every Loot possible...
        for (int i = 0; i < Loot.Length; i++)
        {            
            // ... "roll the dice" to see if it will be found on this expedition
            // Note that the an expedition with a higher loot and danger levels will increase the chances of finding that loot item
            if (Loot[i].ChanceToFind + chanceIncreaseFactor >= Random.Range(0, 100))
            {
                // ... and, if so, add it to the possible expedition loot list
                //lootFound.Add(Loot[i]); 
                Debug.Log("Loot found = " + Loot[i].name);
                inventory.AddLoot(Loot[i], 1);
            }

        }

    }

    void Fight()
    {
        Debug.Log("A fight occurred!");
    }
}
