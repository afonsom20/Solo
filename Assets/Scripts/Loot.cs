using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot
{
    public string Name;

    [Tooltip("Each expedition has a Loot Level. At what levels can this item be found?")]
    public int[] Levels;

    public int ChanceToFind = 30;
}
