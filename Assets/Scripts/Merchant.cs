using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Merchant : MonoBehaviour
{
    public static Merchant Instance { get; set; }

    [SerializeField] GameObject graphics;
    [SerializeField] GameObject inventoryHolder;
    [SerializeField] GameObject doorInteractable;
    [SerializeField] int minimumDaysUntilVisit = 7;
    [SerializeField] int maximumDaysUntilVisit = 12;

    [Space, Header("Dialogue")]
    [SerializeField] TextMeshProUGUI dialogueBox;
    [SerializeField] GameObject choiceButtons;
    [SerializeField] float typingSpeed = 0.02f;
    [SerializeField] string firstGreeting;
    [SerializeField] string generalGreeting;
    [SerializeField] string rejectedDialogue;
    [SerializeField] string checkedInventoryDialogue;
    [SerializeField] string notEnoughFoodDialogue;
    [SerializeField] string playerAlreadyHasUniqueItemDialogue;
    [SerializeField] List<string> boughtSomethingDialogues;
    [SerializeField] string goodbyeDialogue;

    [Space, Header("Loot/Items")]
    [SerializeField] Transform merchantItemHolder;
    [SerializeField] GameObject merchantItemPrefab;
    [SerializeField] List<Loot> items;
    [Tooltip("The first index has the value 0 but that's not the probability" +
        "of finding the first item. That probability is in the index 1"), SerializeField] List<int> itemProbability;
    [SerializeField] List<int> prices;

    bool isFirstVisit = true;
    bool alreadyBoughtSomething = false;
    int nextVisit;

    Inventory inventory;

    void Start()
    {
        Instance = this;

        inventory = FindObjectOfType<Inventory>();

        DecideNextVisit();
    }

    void DecideNextVisit()
    {
        alreadyBoughtSomething = false;

        // Calculate the number of days until the next visit
        nextVisit = Random.Range(minimumDaysUntilVisit, maximumDaysUntilVisit + 1);
    }

    public void NewDay()
    {
        // The day of next visit gets closer
        nextVisit -= 1;

        // If the time for the visit has arrived
        if (nextVisit == 0)
        {
            doorInteractable.SetActive(true);
            AudioManager.Instance.Play("Door Knock");
            InformationManager.Instance.SendInfo(5, "There's someone at the door");            
            DecideNextVisit();
        }
    }

    public void OpenDoor()
    {
        doorInteractable.SetActive(false);
        choiceButtons.SetActive(true);
        graphics.SetActive(true);
        doorInteractable.GetComponent<OutlineToggler>().ToggleOutline(); // if we don't do this, next time the outline will be on when the mouse is not on top of the door, which is wrong
        RandomizeItems();
        AudioManager.Instance.Play("Open Door");

        // Dialogue
        if (isFirstVisit)
        {
            Speak(firstGreeting, false);
            isFirstVisit = false;
        }
        else
            Speak(generalGreeting, false);
    }

    public void Leave()
    {
        choiceButtons.SetActive(false);

        if (!alreadyBoughtSomething)
        {
            AudioManager.Instance.Play("RejectMerchant");
            Speak(rejectedDialogue, true);    
        }
        else
        {
            AudioManager.Instance.Play("CheckMerchantStuff");
            Speak(goodbyeDialogue, true);
        }

        Reset();
    }

    public void BoughtSomething(string itemName, int amount)
    {
        inventory.AddLoot(items.Find(item => item.Name == itemName), amount);

        int index = Random.Range(0, boughtSomethingDialogues.Count);

        if (!alreadyBoughtSomething)
            Speak(boughtSomethingDialogues[index], false);
        else
            Speak(goodbyeDialogue, true);

        InformationManager.Instance.SendInfo(0, "You have purchased an item");
        AudioManager.Instance.Play("BuyFromMerchant");
        alreadyBoughtSomething = true;
    }

    void RandomizeItems()
    {
        for (int i = 0; i < 2; i++)
        {
            int random = Random.Range(0, 100);

            for (int j = 0; j < items.Count; j++)
            {
                if (random >= itemProbability[j] && random < itemProbability[j + 1])
                {
                    MerchantItem newItem = Instantiate(merchantItemPrefab, merchantItemHolder).GetComponent<MerchantItem>();
                    newItem.Name = items[j].Name;
                    newItem.DescriptionText = items[j].TooltipDescription;
                    Image image = newItem.GetComponent<Image>();
                    image.sprite = items[j].Icon;

                    if (!items[j].OnlyOneFoundPerExpedition)
                    {
                        int amount = Random.Range(items[j].AmountFoundMin, items[j].AmountFoundMax + 1);
                        newItem.AmountText.text = amount.ToString();
                    }

                    int price = prices[j] + Random.Range(-1, 2);
                    if (price < 1)
                        price = 1;

                    newItem.Price = price;
                    newItem.PriceText.text = price.ToString();
                }
            }

        }
    }

    public void CheckInventory()
    {
        inventoryHolder.SetActive(true);
        AudioManager.Instance.Play("CheckMerchantStuff");
        Speak(checkedInventoryDialogue, false);
    }

    public void NotEnoughFood()
    {
        AudioManager.Instance.Play("RejectMerchant");
        Speak(notEnoughFoodDialogue, false);
    }

    public void PlayerAlreadyHasUniqueItem()
    {
        AudioManager.Instance.Play("RejectMerchant");
        Speak(playerAlreadyHasUniqueItemDialogue, false);
    }

    IEnumerator Type(string text, bool endDialogue)
    {
        dialogueBox.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (endDialogue)
        {
            yield return new WaitForSeconds(2f);
            AudioManager.Instance.Play("Close Door"); 
            graphics.SetActive(false);
        }
    }

    void Speak(string text, bool endDialogue)
    {
        StopAllCoroutines();
        StartCoroutine(Type(text, endDialogue));
    }

    public void Reset()
    {
        doorInteractable.SetActive(false);
        inventoryHolder.SetActive(false);

        // Discard items that were not bought 
        for (int i = 0; i < merchantItemHolder.childCount; i++)
        {
            Destroy(merchantItemHolder.GetChild(i).gameObject);
        }
    }
}
