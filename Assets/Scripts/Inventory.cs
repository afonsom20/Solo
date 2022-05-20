using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject UIPrefab;
    [SerializeField] Transform itemHolder;

    List<Loot> Loot = new List<Loot>();
    List<int> Amounts = new List<int>();

    public void AddLoot(Loot loot, int amount)
    {
        // Check every current inventory item
        for (int i = 0; i < Loot.Count; i++)
        {
            // If the recently found loot is already present in the inventory
            if (loot.Name == Loot[i].Name)
            {
                // Increase the amount in the inventory
                Amounts[i] += amount;

                // Change the text to represent this change in amount
                itemHolder.GetChild(i).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = Amounts[i].ToString();
                
                // Exit the method, since there's no need to create a new item in the inventory
                return;            
            }
        }

        // If the look found was food, don't add it to the inventory, but to the special food slot
        if (loot.Name == "Food")
        {
            int foodAmount = Random.Range(1, 4);
            PlayerStatusManager.Instance.FindFood(foodAmount);
            return;
        }

        // If the loot is still not present in the inventory, add it
        Loot.Add(loot);
        Amounts.Add(amount);

        // Add the UI prefab object to the inventory UI
        GameObject newItem = Instantiate(UIPrefab, itemHolder);
        newItem.GetComponent<Image>().sprite = loot.Icon;

    }
}
