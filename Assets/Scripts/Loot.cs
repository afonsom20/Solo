using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot Item")]
public class Loot : ScriptableObject
{
    public string Name;

    [Tooltip("Each expedition has a Loot Level. From what level can this item be found?")]
    public int MinimumLevel;

    public int ChanceToFind = 30;

    public int AmountMin = 1;

    public int AmountMax = 5;

    public Sprite Icon;
}
