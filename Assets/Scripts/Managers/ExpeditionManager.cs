using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionManager : MonoBehaviour
{
    public Expeditions[] Expeditions;
    public Loot[] Loot;

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

    void GetLoot(int level)
    {

    }
}
