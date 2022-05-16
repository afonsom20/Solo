using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionManager : MonoBehaviour
{
    public Expeditions[] Expeditions;
    public Loot[] Loot;

    [SerializeField] Inventory inventory;

    public void GoOnExpedition(int index)
    {
        int danger = Expeditions[index].Danger;
        int lootLevel = Expeditions[index].LootLevel;
        int fightChance = Expeditions[index].FightEncounterChance;

        GetLoot(lootLevel);

        if (Random.Range(0, 100) <= fightChance)
        {
            Debug.Log("A fight occurred!");
        }
        
    }

    void GetLoot(int expeditionLootLevel)
    {
        // Create a list where we'll store the loot that is possible to get in the current expedition
        //List<Loot> lootFound = new List<Loot>();

        // For every Loot possible...
        for (int i = 0; i < Loot.Length; i++)
        {            
            // ... "roll the dice" to see if it will be found on this expedition
            if (Loot[i].ChanceToFind >= Random.Range(0, 100))
            {
                // ... and, if so, add it to the possible expedition loot list
                //lootFound.Add(Loot[i]);
                inventory.AddLoot(Loot[i], 1);
            }

        }

    }
}
