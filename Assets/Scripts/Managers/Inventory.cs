using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject UIPrefab;
    [SerializeField] Transform itemHolder;
    [SerializeField] int firstAidKitHealAmount = 30;
    [SerializeField] int drugsHealAmount = 20;

    public List<Loot> Loot = new List<Loot>();
    public List<int> Amounts = new List<int>();

    public void AddLoot(Loot lootFound, int amountFound)
    {
        //Debug.Log(lootFound.Name);

        // Check every current inventory item
        for (int i = 0; i < Loot.Count; i++)
        {
            // If the recently found loot is already present in the inventory
            if (lootFound.Name == Loot[i].Name)
            {
                // If this item has a limited amount in the inventory (e.g. 1 pistol, 3 wood)...
                if (lootFound.LimitInventoryAmount)
                {
                    // ...then we check if the amount found + what we already have would be too much
                    if (Amounts[i] + amountFound >= lootFound.MaxInventoryAmount)                    
                        // if so, we just set the amount to the maximum possible
                        Amounts[i] = lootFound.MaxInventoryAmount;
                    // If it's below the limit, just add the amount normally
                    else
                        Amounts[i] += amountFound;
                }
                // If there's not limit, just increase the amount in the inventory
                else
                    Amounts[i] += amountFound;
    

                // Change the text to represent this change in amount
                itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = Amounts[i].ToString();
                
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
        Loot.Add(lootFound);
        Amounts.Add(amountFound);       

        // Add the UI prefab object to the inventory UI
        GameObject newItem = Instantiate(UIPrefab, itemHolder);
        Image image = newItem.GetComponent<Image>(); 
        image.sprite = lootFound.Icon;
        newItem.transform.GetComponentInChildren<TextMeshProUGUI>().text = amountFound.ToString();
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.ItemName = lootFound.Name;
        inventoryItem.DescriptionText = lootFound.TooltipDescription;

        if (lootFound.Name == "First Aid Kit" || lootFound.Name == "Drugs")
        {
            image.color = Color.green;
            inventoryItem.ItemIsUsable = true;
        }
        else if (lootFound.ImprovesFighting)
        {
            image.color = Color.cyan;
        }
    }


    public bool CheckIfItemMaxAmountReached(Loot item)
    {
        bool maxAmountReached = false; //only doing it this way (making a variable instead of writing "return true/false") because it's giving me a weird error

        for (int i = 0; i < Loot.Count; i++)
        {
            if (item.Name == Loot[i].Name)
            {
                // If this item has a limited amount in the inventory (e.g. 1 pistol, 3 wood)...
                if (item.LimitInventoryAmount)
                {
                    // ...then we check if the amount we already is at the maximum
                    if (Amounts[i] == item.MaxInventoryAmount)
                    {
                        maxAmountReached = true;
                    }
                    else
                        maxAmountReached = false;
                }
            }
        }

        return maxAmountReached;
    }

    public void UseItem(string name)
    {        
        // Check if the item clicked was Drugs, and also if the player is Sick
        if ((name == "Drugs") && (PlayerStatusManager.Instance.Sick))
        {
            // If both of these are true, we're gonna use the Drugs to cure the illness.
            // For this, we loop through every inventory item (Loot List)...
            for (int i = 0; i < Loot.Count; i++)
            {
                // ... until we find the Drugs
                if (Loot[i].Name == "Drugs")
                {
                    // 1 amount of Drugs is used
                    Amounts[i] -= 1;

                    // The player stops being sick
                    PlayerStatusManager.Instance.Sick = false;
                    PlayerStatusManager.Instance.ToggleSicknessIndicator();
                    PlayerStatusManager.Instance.IncreaseHealth(drugsHealAmount);
                    InformationManager.Instance.SendInfo(0, "You take some medicine and quickly start to feel better");
                    AudioManager.Instance.Play("Drugs");

                    // If there are still Drugs
                    if (Amounts[i] != 0)
                    {
                        // Change the text to represent this change in amount
                        itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = Amounts[i].ToString();
                    }
                    // If not, remove them from the inventory
                    else
                    {
                        Loot.Remove(Loot[i]);
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
            for (int i = 0; i < Loot.Count; i++)
            {
                // ... until we find the First Aid Kit
                if (Loot[i].Name == "First Aid Kit")
                {
                    // 1 First Aid Kit is used
                    Amounts[i] -= 1;

                    // The player stops being wounded
                    PlayerStatusManager.Instance.Wounded = false;
                    PlayerStatusManager.Instance.ToggleWoundIndicator();
                    PlayerStatusManager.Instance.IncreaseHealth(firstAidKitHealAmount);
                    InformationManager.Instance.SendInfo(0, "You patched yourself up, stopping the bleeding");
                    AudioManager.Instance.Play("First Aid Kit");

                    // If there are still First Aid Kits
                    if (Amounts[i] > 0)
                    {
                        // Change the text to represent this change in amount
                        itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = Amounts[i].ToString();
                    }
                    // If not, remove them from the inventory
                    else
                    {
                        Destroy(itemHolder.GetChild(i).gameObject);
                        Loot.Remove(Loot[i]);
                    }

                }
            }
        }
        // This will happen only when the function is called from the RaidManager
        // We just need to remove one amount of wood
        else if (name == "Wood")
        {
            for (int i = 0; i < Loot.Count; i++)
            {
                if (Loot[i].Name == "Wood")
                {
                    Amounts[i] -= 1;

                    // If there are still Wood logs
                    if (Amounts[i] > 0)
                    {
                        // Change the text to represent this change in amount
                        itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = Amounts[i].ToString();
                    }
                    // If not, remove them from the inventory
                    else
                    {
                        Destroy(itemHolder.GetChild(i).gameObject);
                        Loot.Remove(Loot[i]);
                    }
                }
            }
        }
    }

    public bool CheckIfHasUniqueItemAlready(string name)
    {
        // Find the item in the inventory
        Loot item = Loot.Find(item => item.Name == name);

        // If the item is present in the inventory
        if (item != null)
        {
            // Check if it's a unique item
            if (item.LimitInventoryAmount && item.MaxInventoryAmount == 1)
                return true;
            else
                return false;
        }        
        else
            return false;
       
    }

    public void RemoveItem(string name, int amount)
    {
        for (int i = 0; i < Loot.Count; i++)
        {
            if (Loot[i].name == name)
            {
                Amounts[i] -= amount;

                if (Amounts[i] > 0)
                {
                    // Change the text to represent this change in amount
                    itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = Amounts[i].ToString();
                }
                // If not, remove them from the inventory
                else
                {
                    Destroy(itemHolder.GetChild(i).gameObject);
                    Loot.Remove(Loot[i]);
                }
            }
        }
    }
}
