using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Expeditions
{
    public string Name;

    [Tooltip("Danger of the expedition and of encounters that may happen. 0 - None; 1 - Low; 2 - Medium, 3 - High")]
    [Range(0, 3)]
    public int Danger;

    [Tooltip("0 - None; 1 - Average; 2 - Good, 3 - Great")]
    [Range(0, 3)]
    public int LootLevel;

    [Range(0, 100)]
    public int FightEncounterChance;

    public string[] PossibleLoot;
}
