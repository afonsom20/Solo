using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot
{
    public string Name;

    [Tooltip("Each expedition has a Loot Level. From what level can this item be found?")]
    public int MinimumLevel;

    public int ChanceToFind = 30;
}
