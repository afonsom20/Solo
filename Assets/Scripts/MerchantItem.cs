using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class MerchantItem : MonoBehaviour
{
    public string Name;

    public int Price;

    public string DescriptionText;

    public TextMeshProUGUI AmountText;

    public TextMeshProUGUI PriceText;

    Inventory inventory;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();    
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

    public void BuyItem()
    {
        int food = PlayerStatusManager.Instance.Food;

        if (food >= Price)
        {
            if (inventory.CheckIfHasUniqueItemAlready(Name))
            {
                Merchant.Instance.PlayerAlreadyHasUniqueItem();
                return;
            }
            else
            HideTooltipDescription();
            Merchant.Instance.BoughtSomething(Name, int.Parse(AmountText.text));
            PlayerStatusManager.Instance.PayMerchant(Price);
            Destroy(gameObject);
        }
        else
        {
            Merchant.Instance.NotEnoughFood();
        }
    }
}
