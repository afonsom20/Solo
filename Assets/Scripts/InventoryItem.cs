using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [HideInInspector] public string ItemName;
    [HideInInspector] public bool ItemIsUsable;

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
}
