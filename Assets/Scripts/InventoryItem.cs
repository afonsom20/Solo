using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    // These values are all set by the Inventory script once the item is created.
    // They can then be accessed by other scripts to check the information about this item
    [HideInInspector] public string ItemName;
    [HideInInspector] public bool ItemIsUsable;
    [HideInInspector] public string DescriptionText; 

    void Start()
    {
        if (!ItemIsUsable)
        {
            Destroy(GetComponent<Button>());
        }
    }

    public void UseItem()
    {
        FindObjectOfType<Inventory>().UseItem(ItemName);
    }

    public void ShowTooltipDescription()
    {
        if (DescriptionText != "")
            Tooltip.Instance.Activate(DescriptionText);
    }

    public void HideTooltipDescription()
    {
        Tooltip.Instance.Deactivate();
    }
}
