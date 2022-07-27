using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusManager : MonoBehaviour
{
    public static PlayerStatusManager Instance { get; set; }

    public int Food = 6;
    
    [HideInInspector] public int Health;
    [HideInInspector] public int Energy;
    [HideInInspector] public int Hygiene;
    [HideInInspector] public int Hope;

    [HideInInspector] public bool Sick = false;
    [HideInInspector] public bool Wounded = false;

    [Space, Header("Death Screen")]
    [SerializeField] GameObject deathScreen;
    [Space, Header("UI Meters")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider energySlider;
    [SerializeField] Slider hygieneSlider;
    [SerializeField] Slider hopeSlider;
    [SerializeField] TextMeshProUGUI foodText;
    [SerializeField] GameObject sicknessIndicator;
    [SerializeField] GameObject woundIndicator;

    [Space, Header("Action Effects")]
    [SerializeField] int restEffectOnHealth = 10;
    [SerializeField] int restEffectOnEnergy = 50;
    [SerializeField] int hygienizeEffectOnHygiene = 44;
    [SerializeField] int hygienizeEffectOnHope = 15;
    [SerializeField] int hopeLossFromKeepingWatch = 5;
    [SerializeField] int energyLossFromKeepingWatch = 20;

    [Space, Header("Daily Status Decrease")]
    [SerializeField] int energyDecreasePerPhase = 15;
    [SerializeField] int hygieneDecreasePerPhase = 15;
    [SerializeField] int hopeDecreasePerPhase = 5;

    [Space, Header("Sickness and Wounds")]
    [SerializeField] int chanceOfSicknessOnLowHygiene = 10;
    [SerializeField] int sickenssHealthDecrease = 5;
    [SerializeField] int probabilityOfHealingSicknessPerPhase = 7;
    [SerializeField] int minimumSicknessPhases = 2; 
    [SerializeField] int woundHealthDecrease = 10;
    [SerializeField] int probabilityOfHealingWoundPerPhase = 4;
    [SerializeField] int minimumWoundedPhases = 4;

    [Space, Header("Thresholds and Effects")]
    public int LowHealthThreshold = 30; // these are public because they're read by the StatusIcon script on start
    public int LowEnergyThreshold = 30;
    public int LowHygieneThreshold = 25;
    public int LowHopeThreshold = 25;
    [SerializeField] int foodEatenPerDay = 2;
    [Space]
    [SerializeField] int lowEnergyHealthEffect = 15;
    [SerializeField] int lowHygieneHealthEffect = 15;
    [SerializeField] int lowHopeHealthEffect = 10;
    [SerializeField] int noFoodHealthEffect = 20;

    int maxHealth;
    int maxEnergy;
    int maxHygiene;
    int maxHope;

    int phasesSinceBecameSick;
    int phasesSinceBecameWounded;

    void Awake()
    {
        Instance = this;

        // Get max values of each status
        maxHealth = (int)healthSlider.maxValue;    
        maxEnergy = (int)energySlider.maxValue;    
        maxHygiene = (int)hygieneSlider.maxValue;    
        maxHope = (int)hopeSlider.maxValue;

        // Set initial values of each status
        Health = maxHealth;
        Energy = Mathf.RoundToInt(maxEnergy / 1.5f);
        Hygiene = Mathf.RoundToInt(maxHygiene / 2.5f);
        Hope = Mathf.RoundToInt(maxHope / 3f);

        UpdateStatusMeters();
    }

    public void UpdateStatusMeters()
    {
        healthSlider.value = Health;
        energySlider.value = Energy;
        hygieneSlider.value = Hygiene;
        hopeSlider.value = Hope;
    }

    void CheckFoodAmount()
    {
        // The player has enough food to feed themselves for one day
        if (Food > foodEatenPerDay)
        {
            if (Food <= 2 * foodEatenPerDay)
            {
                InformationManager.Instance.SendInfo(6, "You're running low on Food!");
                foodText.color = Color.yellow;
            }
            else
                foodText.color = Color.white;
        }
        // If the player does not have enough food to feed themselves for one day
        else if (Food == 0)
        {
            foodText.color = Color.red;
            InformationManager.Instance.SendInfo(1, "You're out of Food!");
        }
        else
        {
            InformationManager.Instance.SendInfo(6, "You're running low on Food!");
            foodText.color = Color.yellow;
        }
    }

    // Eating is done AUTOMATICALLY at the start of every day
    public IEnumerator Eat()
    {
        yield return new WaitForSeconds(TimeManager.Instance.NightDayTransitionTime);

        // The player has enough food, so they eat it
        if (Food > foodEatenPerDay)
        {
            if (Food <= 2 * foodEatenPerDay)
            {
                InformationManager.Instance.SendInfo(6, "You're running low on Food!");
                foodText.color = Color.yellow;
            }
            else
                foodText.color = Color.white;

            Food -= foodEatenPerDay;
        }
        // If the player does not have enough food to feed themselves for today
        else
        {
            foodText.color = Color.red;

            // If the player still has some food, they get a lower health decrease
            if (Food > 0)
            {
                Health -= (noFoodHealthEffect - Food * 5);
                Food = 0;
            }
            // If the player doesn't have any food, they get the biggest health decrease
            else
            {
                InformationManager.Instance.SendInfo(1, "You're out of Food!");
                Health -= noFoodHealthEffect;
                Food = 0;
            }
        }

        // Update UI
        foodText.text = Food.ToString();
        UpdateStatusMeters();
    }

    // When Food is found on an expedition
    public void FindFood(int amount)
    {
        Food += amount;
        StartCoroutine(DelayedFoodUIUpdate());
    }

    public void FoodStolen(int amount)
    {
        Food -= amount;

        if (Food < 0)
            Food = 0;

        // Update UI
        CheckFoodAmount();
        foodText.text = Food.ToString();
        UpdateStatusMeters();
    }

    IEnumerator DelayedFoodUIUpdate()
    {
        if (TimeManager.Instance.CurrentPhase == TimeManager.DayPhase.Night)
            yield return new WaitForSeconds(TimeManager.Instance.NightDayTransitionTime);
        else
            yield return new WaitForSeconds(TimeManager.Instance.DayNightTransitionTime);

        // Update UI
        CheckFoodAmount();
        foodText.text = Food.ToString();
        UpdateStatusMeters();
    }

    public void StatusDecreasePerPhase()
    {
        Energy -= energyDecreasePerPhase;
        if (Energy < 0) 
            Energy = 0;

        Hygiene -= hygieneDecreasePerPhase;
        if (Hygiene < 0)
            Hygiene = 0;

        Hope -= hopeDecreasePerPhase;
        if (Hope < 0)
            Hope = 0;


        // SICKNESS and WOUNDS
        // If the player is sick
        if (Sick)
        {
            // Decrease their health
            Health -= sickenssHealthDecrease;

            // Check how many phases have passed since the player became sick
            if (phasesSinceBecameSick > minimumSicknessPhases)
            {
                // If enough time has passed, there's a chance that the player fights off the illness. This chance increases each phase
                if (Random.Range(0, 100) <= (phasesSinceBecameSick * probabilityOfHealingSicknessPerPhase))
                {
                    Sick = false;
                    ToggleSicknessIndicator();
                    InformationManager.Instance.SendInfo(0, "Your body managed to overcome your dangerous illness.");
                }             
                
            }

            phasesSinceBecameSick += 1;
        }
        else
        {
            phasesSinceBecameSick = 0;
        }


        // If the player is wounded
        if (Wounded)
        {
            // Decrease their health
            Health -= woundHealthDecrease;

            // Check how many phases have passed since the player became sick
            if (phasesSinceBecameWounded > minimumWoundedPhases)
            {
                Debug.Log("Check for wound auto-heal - probability = " + (phasesSinceBecameWounded * probabilityOfHealingWoundPerPhase));
                // If enough time has passed, there's a chance that the player fights off the illness. This chance increases each phase
                if (Random.Range(0, 100) <= (phasesSinceBecameWounded * probabilityOfHealingWoundPerPhase))
                {
                    Wounded = false;
                    ToggleWoundIndicator();
                    InformationManager.Instance.SendInfo(0, "Your body managed to heal your wounds");
                }

            }

            phasesSinceBecameWounded += 1;
        }
        else
        {
            phasesSinceBecameWounded = 0;
        }

    }

    public void ToggleSicknessIndicator()
    {
        if (Sick)
            sicknessIndicator.SetActive(true);
        else
            sicknessIndicator.SetActive(false);
    }

    public void ToggleWoundIndicator()
    {
        if (Wounded)
            woundIndicator.SetActive(true);
        else
            woundIndicator.SetActive(false);
    }

    public IEnumerator ToggleWoundIndicatorWithDelay()
    {
        //reversed, for some reason
        if (TimeManager.Instance.CurrentPhase == TimeManager.DayPhase.Night)
            yield return new WaitForSeconds(TimeManager.Instance.NightDayTransitionTime);
        else
            yield return new WaitForSeconds(TimeManager.Instance.DayNightTransitionTime);

        if (Wounded)
            woundIndicator.SetActive(true);
        else
            woundIndicator.SetActive(false);
    }

    public void CheckPlayerCondition()
    {
        if (Health <= 0)
        {
            deathScreen.SetActive(true);
        }

        // Low energy effect on health
        if (Energy <= LowEnergyThreshold)
        {
            Health -= lowEnergyHealthEffect;
        }

        // Low hygiene effect on health
        if (Hygiene <= LowHygieneThreshold)
        {
            Health -= lowHygieneHealthEffect;

            // If the player is not currently sick, low hygiene could lead to the contraction of an illness
            if ((!Sick) && (Random.Range(0, 100) <= chanceOfSicknessOnLowHygiene))
            {
                Sick = true;
                ToggleSicknessIndicator();
                InformationManager.Instance.SendInfo(2, "You became sick due to low Hygiene. Take some Drugs to heal");
                AudioManager.Instance.Play("Cough");
            }
        }

        // Low hope effect on health
        if (Hope <= LowHopeThreshold)
        {
            Health -= lowHopeHealthEffect;
        }

        // Update UI
        UpdateStatusMeters();

        // LOW STATUS ALERTS - these must be done AFTER the UI update
        if (Health <= LowHealthThreshold)
        {
            InformationManager.Instance.SendInfo(1, "Your Health is dangerously low");
        }

        if (Energy <= LowEnergyThreshold)
        {
            InformationManager.Instance.SendInfo(1, "Your Energy is dangerously low");
        }

        if (Hygiene <= LowHygieneThreshold)
        {
            InformationManager.Instance.SendInfo(1, "Your Hygiene is dangerously low");
        }

        if (Hope <= LowHopeThreshold)
        {
            InformationManager.Instance.SendInfo(1, "Your Hope is dangerously low");
        }
    }
    
    public void Rest()
    {
        // Gain energy when resting
        Energy += restEffectOnEnergy;

        // Heal when resting;
        Health += restEffectOnHealth;

        if (Energy > maxEnergy) 
            Energy = maxEnergy;        
        
        if (Health > maxHealth) 
            Health = maxHealth;

        UpdateStatusMeters();

        RaidManager.Instance.UpdateVariables(true, true);
        TimeManager.Instance.AdvancePhase();

        AudioManager.Instance.Play("Rest");
    }

    public void Hygienize()
    {
        // There's no water during the evening, so the player can't hygienize at that time
        if (TimeManager.Instance.CurrentPhase == TimeManager.DayPhase.Night)
        {
            InformationManager.Instance.SendInfo(0, "Due to shortages, running water is only available during the day");
        }
        // In the morning
        else
        {
            Hygiene += hygienizeEffectOnHygiene;
            Hope += hygienizeEffectOnHope;

            if (Hygiene > maxHygiene)
                Hygiene = maxHygiene;

            UpdateStatusMeters();

            RaidManager.Instance.UpdateVariables(true, false);
            TimeManager.Instance.AdvancePhase();

            AudioManager.Instance.Play("Hygienize");
        }
    }

    public void KeepWatch()
    {
        IncreaseHope(-hopeLossFromKeepingWatch);
        LoseEnergy(energyLossFromKeepingWatch);
        RaidManager.Instance.UpdateVariables(true, false);
        AudioManager.Instance.Play("Keep Watch");
        TimeManager.Instance.AdvancePhase();
    }

    // This function is called in external scripts every time an action that increases hope is performed
    public void IncreaseHope(int amount)
    {
        Hope += amount;

        if (Hope >= maxHope)
            Hope = maxHope;

        UpdateStatusMeters();
    }

    public void IncreaseHealth(int amount)
    {
        Health += amount;

        if (Health >= maxHealth)
            Health = maxHealth;

        UpdateStatusMeters();
    }

    public void PayMerchant(int price)
    {
        Food -= price;

        CheckFoodAmount();

        // Update UI
        foodText.text = Food.ToString();
        UpdateStatusMeters();
    }

    void LoseEnergy(int amount)
    {
        Energy -= amount;

        if (Energy < 0)
            Energy = 0;

        UpdateStatusMeters();
    }
}
