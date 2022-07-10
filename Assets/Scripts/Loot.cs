using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Loot Item")]
public class Loot : ScriptableObject
{
    public string Name;

    [Tooltip("Each expedition has a Loot Level. From what level can this item be found?")]
    public int MinimumLevel;

    public int ChanceToFind = 30;

    public bool OnlyOneFoundPerExpedition = false;

    public bool LimitInventoryAmount = false;

    [ShowIf("LimitInventoryAmount")] 
    public int MaxInventoryAmount = 1;

    [HideIf("OnlyOneFoundPerExpedition")]
    public int AmountFoundMin = 1;

    [HideIf("OnlyOneFoundPerExpedition")]
    public int AmountFoundMax = 5;

    public int MerchantPrice = 3;

    public Sprite Icon;

    public bool ImprovesFighting = false;
    
    public string TooltipDescription = "";
}
