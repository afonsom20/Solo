using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject UIPrefab;
    [SerializeField] Transform itemHolder;

    List<Loot> loot = new List<Loot>();
    List<int> amounts = new List<int>();

    public void AddLoot(Loot lootFound, int amountFound)
    {
        // Check every current inventory item
        for (int i = 0; i < loot.Count; i++)
        {
            // If the recently found loot is already present in the inventory
            if (lootFound.Name == loot[i].Name)
            {
                // Increase the amount in the inventory
                amounts[i] += amountFound;

                // Change the text to represent this change in amount
                itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = amounts[i].ToString();
                
                // Exit the method, since there's no need to create a new item in the inventory
                return;            
            }
        }

        // If the look found was food, don't add it to the inventory, but to the special food slot
        if (lootFound.Name == "Food")
        {
            PlayerStatusManager.Instance.FindFood(amountFound);
            return;
        }

        // If the loot is still not present in the inventory, add it
        loot.Add(lootFound);
        amounts.Add(amountFound);       

        // Add the UI prefab object to the inventory UI
        GameObject newItem = Instantiate(UIPrefab, itemHolder);
        Image image = newItem.GetComponent<Image>(); 
        image.sprite = lootFound.Icon;
        newItem.transform.GetComponentInChildren<TextMeshProUGUI>().text = amountFound.ToString();
        newItem.GetComponent<InventoryItem>().ItemName = lootFound.Name;
        if (lootFound.Name == "First Aid Kit" || lootFound.Name == "Drugs")
        {
            image.color = Color.green;
            newItem.GetComponent<InventoryItem>().ItemIsUsable = true;
        }

    }

    public void UseItem(string name)
    {        
        // Check if the item clicked was Drugs, and also if the player is Sick
        if ((name == "Drugs") && (PlayerStatusManager.Instance.Sick))
        {
            // If both of these are true, we're gonna use the Drugs to cure the illness.
            // For this, we loop through every inventory item (Loot List)...
            for (int i = 0; i < loot.Count; i++)
            {
                // ... until we find the Drugs
                if (loot[i].Name == "Drugs")
                {
                    // 1 amount of Drugs is used
                    amounts[i] -= 1;

                    // The player stops being sick
                    PlayerStatusManager.Instance.Sick = false;
                    InformationManager.Instance.SendInfo(0, "You take some medicine and quickly start to feel better");


                    // If there are still Drugs
                    if (amounts[i] != 0)
                    {
                        // Change the text to represent this change in amount
                        itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = amounts[i].ToString();
                    }
                    // If not, remove them from the inventory
                    else
                    {
                        loot.Remove(loot[i]);
                        Destroy(itemHolder.GetChild(i).gameObject);
                    }

                }
            }
        }
        // If not, check if this is the First Aid Kit, and if the player is Wounded
        else if ((name == "First Aid Kit") && (PlayerStatusManager.Instance.Wounded))
        {
            // If both of these are true, we're gonna use the First Aid Kit to cure the wound.
            // For this, we loop through every inventory item (Loot List)...
            for (int i = 0; i < loot.Count; i++)
            {
                // ... until we find the First Aid Kit
                if (loot[i].Name == "First Aid Kit")
                {
                    // 1 First Aid Kit is used
                    amounts[i] -= 1;

                    // The player stops being wounded
                    PlayerStatusManager.Instance.Wounded = false;
                    InformationManager.Instance.SendInfo(0, "You patched yourself up, stopping the bleeding");

                    // If there are still First Aid Kits
                    if (amounts[i] > 0)
                    {
                        // Change the text to represent this change in amount
                        itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = amounts[i].ToString();
                    }
                    // If not, remove them from the inventory
                    else
                    {
                        Debug.Log("Remove from inventory");
                        Destroy(itemHolder.GetChild(i).gameObject);
                        loot.Remove(loot[i]);
                    }

                }
            }
        }
    }
}
