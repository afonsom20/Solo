using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaidManager : MonoBehaviour
{
    public static RaidManager Instance { get; set; }

    [HideInInspector] public bool StayedHome;
    [HideInInspector] public bool Sleeping;

    [SerializeField] int raidProbability = 25;
    [SerializeField] int weakRaidProbability = 30;
    [SerializeField] int averageRaidProbability = 50;
    [SerializeField, Tooltip("How much the vest reduces the chance of getting wounded")] int bulletproofVestProtectionPercentage = 50;
    [SerializeField] int hopeLossFromSurrendering = 30;
    
    [Space, Header("Fighting"), SerializeField] int baseFightHealthDecrease = 25;
    [SerializeField] int fightHealthDecreaseDeviation = 20;
    [SerializeField] int baseWoundChance = 75;
    [SerializeField] int playerDefenceWeight = 30;

    [Space, Header("UI")] 
    [SerializeField] TextMeshProUGUI dangerText;
    [SerializeField] TextMeshProUGUI playerPreparationText;
    [SerializeField] Button blockDoorButton;
    [SerializeField] GameObject raidScreen;
    [SerializeField] GameObject raidRecapBoard;
    [SerializeField] TextMeshProUGUI raidRecapTextHolder;
    [SerializeField] TextMeshProUGUI raidRecapStolenItems;
    [SerializeField] TextMeshProUGUI raidRecapItemsUsedInFight;
    [SerializeField] string[] raidRecapText;

    [Space, Header("References"), SerializeField] Inventory inventory;
    [SerializeField] Animator raidUIAnimator;

    float playerDefenceLevel = 0f;
    bool hasBulletproofVest = false;
    bool hasGun = false;
    bool hasKnife = false;
    int ammoAmount;
    int raidDanger;
    bool fought;

    void Start()
    {
        Instance = this;    
    }

    public void NightToDayTransition()
    {
        if (raidProbability > Random.Range(0, 100))
        {
            CheckRaidDanger();
            CheckPlayerPreparation();
            UpdateRecapBoard();

            // Only activate the UI (i.e. enabling the player to react to the raid) if they were home on watch, not asleep
            if (StayedHome && !Sleeping)           
                raidScreen.SetActive(true);          
            else if (Sleeping) // if they were home but were sleeping, fighting is mandatory          
                Fight();

            DetermineStolenItems();
            raidRecapBoard.SetActive(true);
        }
    }

    public void Fight()
    {       
        AudioManager.Instance.Play("Gunfight");

        // Calculate factors for decreasing the player's Health
        float deviationFactor = Random.Range(0f, 2f) * 2f - 1f; // returns a value between -1 and 1
        float damage = (baseFightHealthDecrease * raidDanger - playerDefenceWeight * playerDefenceLevel) + (deviationFactor * fightHealthDecreaseDeviation);

        if (hasBulletproofVest)
            damage *= (bulletproofVestProtectionPercentage / 100f);

        if (damage < 0)
            damage = 0;

        // Use ammo in the fight, if the player has a gun and ammo
        if (hasGun && ammoAmount > 0)
        {
            inventory.UseItem("Ammunition");
        }

        // If this fight would kill the player, just lower their health to 5
        if (PlayerStatusManager.Instance.Health - damage <= 0)
            PlayerStatusManager.Instance.Health = 5;
        // If not, simply decrease the player's Health
        else
            PlayerStatusManager.Instance.Health -= Mathf.RoundToInt(damage);

        PlayerStatusManager.Instance.UpdateStatusMeters();

        // Send info about the fight
        InformationManager.Instance.SendInfo(4, "You fought the raiders.");

        // Check if the player got wounded during the fight
        if (!PlayerStatusManager.Instance.Wounded)
        {
            float woundChance = (baseWoundChance - playerDefenceWeight * playerDefenceLevel);
            
            if (hasBulletproofVest)
                woundChance *= (bulletproofVestProtectionPercentage / 100f);           

            if (Random.Range(0, 100) <= woundChance)
            {
                PlayerStatusManager.Instance.Wounded = true;
                PlayerStatusManager.Instance.ToggleWoundIndicator();
                InformationManager.Instance.SendInfo(3, "You got wounded in a fight. Treat it with a First Aid Kit");
            }
        }

        if (hasGun)
            Debug.Log("has gun");

        // Items used in fight
        if (hasGun && ammoAmount > 0)
        {
            Debug.Log("has loaded gun");
            raidRecapItemsUsedInFight.text += "- Pistol";
        }
        if (hasKnife)
            raidRecapItemsUsedInFight.text += "\n- Knife";
        if (hasBulletproofVest)
            raidRecapItemsUsedInFight.text += "\n- Bulletproof Vest";

        if (!Sleeping)
            raidRecapTextHolder.text = raidRecapText[2];

        fought = true;
        raidUIAnimator.SetTrigger("FadeOut");
    }

    public void BlockDoor()
    {
        AudioManager.Instance.Play("Wood Log");
        inventory.UseItem("Wood");

        InformationManager.Instance.SendInfo(0, "You used 1 wood log to block the door.");

        raidRecapBoard.SetActive(false);

        raidUIAnimator.SetTrigger("FadeOut");
    }

    public void Surrender()
    {
        AudioManager.Instance.Play("Surrender");

        PlayerStatusManager.Instance.IncreaseHope(-hopeLossFromSurrendering);

        raidRecapTextHolder.text = raidRecapText[3];
        raidUIAnimator.SetTrigger("FadeOut");
    }

    public void UpdateVariables(bool stayedHome, bool sleeping)
    {
        StayedHome = stayedHome;
        Sleeping = sleeping;
    }

    void DetermineStolenItems()
    {
        if (StayedHome)
        {
            if (playerDefenceLevel - raidDanger > 0.5f)
            {
                //Debug.Log("Player strong enough to deter raid");
                return;
            }
        }

        int numberOfItemsToSteal = raidDanger - Mathf.RoundToInt(playerDefenceLevel);       
        int foodStolen = Random.Range(1, 1 + 3 * raidDanger);

        if (fought)
        {
            foodStolen -= 1;
            numberOfItemsToSteal -= 1;
        }

        if (numberOfItemsToSteal > inventory.Loot.Count)
            numberOfItemsToSteal = inventory.Loot.Count;

        //Debug.Log("no. of items to steal = " + numberOfItemsToSteal);

        if (foodStolen > 0)
        {
            PlayerStatusManager.Instance.FoodStolen(foodStolen);
            raidRecapStolenItems.text += "- " + foodStolen.ToString() + " Food";
        }

        if (numberOfItemsToSteal > 0)
        {
            for (int i = 0; i < numberOfItemsToSteal; i++)
            {
                int itemIndex = Random.Range(0, inventory.Loot.Count);
                if (inventory.Loot[itemIndex].Name == "Food") //skip the food, since it's always stolen
                    continue;

                int amountToSteal = Random.Range(1, inventory.Loot[itemIndex].MaxInventoryAmount);

                raidRecapStolenItems.text += "\n- " + amountToSteal.ToString() + " " + inventory.Loot[itemIndex].Name;
                
                inventory.RemoveItem(inventory.Loot[itemIndex].Name, amountToSteal);
            }
        }

        fought = false;
    }

    // This is done in case to update the UI in case the player has no action in the raid - they're not home or they're sleeping
    // In case the player has an action, the specific method for that action (Fight() and Surrender()) will change the recap text accordingly
    void UpdateRecapBoard()
    {
        raidRecapItemsUsedInFight.text = "";
        raidRecapStolenItems.text = "";

        if (StayedHome && Sleeping)       
            raidRecapTextHolder.text = raidRecapText[1];        
        else if (!StayedHome)
            raidRecapTextHolder.text = raidRecapText[0];
    }

    void CheckRaidDanger()
    {
        int random = Random.Range(0, 100);

        // Weak Raid
        if (weakRaidProbability > random)
        {
            dangerText.text = "- There's <color=\"orange\">little</color=\"orange\"> commotion outside";
            raidDanger = 1;
        }
        // Average Raid
        else if (averageRaidProbability + weakRaidProbability > random)
        {
            dangerText.text = "- There's <color=\"orange\">some</color=\"orange\"> commotion outside";
            raidDanger = 2;
        }
        // Strong raid
        else
        {
            dangerText.text = "- There's <color=\"orange\">a lot</color=\"orange\"> of commotion outside";
            raidDanger = 3;
        }
    }

    void CheckPlayerPreparation()
    {
        playerDefenceLevel = 0;
        blockDoorButton.interactable = false;

        // Get ammo amount
        for (int i = 0; i < inventory.Loot.Count; i++)
        {
            if (inventory.Loot[i].Name == "Ammunition")
            {
                ammoAmount = inventory.Amounts[i];
                break;
            }
            else
            {
                ammoAmount = 0;
            }
        }

        // Check weapons and vest
        for (int i = 0; i < inventory.Loot.Count; i++)
        {
            //Debug.Log("Check inventory for guns and vest");
            // Search for weapons and vest in the inventory
            if (inventory.Loot[i].ImprovesFighting)
            {
                //Debug.Log("Has gun or knife or vest");
                // Check if the weapon is a gun
                if (inventory.Loot[i].IsGun)
                {
                    //Debug.Log("Has gun");
                    hasGun = true;
                    // If so, check if there's ammo
                    if (ammoAmount > 0)
                    {                        
                        // If there is, then the player's defence is dramatically increased, especially with more ammo
                        playerDefenceLevel += 2f + Mathf.Log10(ammoAmount);
                        //Debug.Log("increase defence level from gun and ammo");
                    }
                }
                // If it's a knife, the defence only slightly increases
                else if (inventory.Loot[i].Name == "Knife")
                {
                    hasKnife = true;
                    playerDefenceLevel += 0.5f;
                }
                // If it's a bulletproof vest, the defence increases a lot
                else
                {
                    hasBulletproofVest = true;
                    playerDefenceLevel += 2f;
                }
            }
            // Search for wood logs in the inventory
            else if (inventory.Loot[i].Name == "Wood")
            {
                blockDoorButton.interactable = true;
            }
        }


        // If the player is sleeping, then their defence level is actually half of if they were on watch
        if (Sleeping)
        {
            playerDefenceLevel /= 2f;
        }

        // Chance UI to reflect the player's preparation
        if (playerDefenceLevel < 0.5)
        {
            playerPreparationText.text = "- You feel <color=\"orange\">inadequatly armed</color=\"orange\"> to fight";
        }
        else if (playerDefenceLevel < 2)
        {
            playerPreparationText.text = "- You feel <color=\"orange\">somewhat prepared</color=\"orange\"> to fight";
        }
        else
        {
            playerPreparationText.text = "- You feel <color=\"orange\">very well armed</color=\"orange\"> to fight";
        }
    }
}
