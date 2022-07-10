using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; set; }

    public int DaysUntilRescue = 30;

    [Header("Time Variables")] public float DayNightTransitionTime = 1.5f;
    public float NightDayTransitionTime = 3.5f;

    [Header("Art")]
    [SerializeField] GameObject[] backgrounds;

    [Header("UI")]
    [SerializeField] Animator blackScreen;
    [SerializeField] TextMeshProUGUI dayText;
    //[SerializeField] GameObject darkPanel;
    [SerializeField] GameObject winScreen;

    [Space, Header("Volume & VFX")]
    [SerializeField] Volume volume;

    public enum DayPhase { Day, Night };

    [HideInInspector] public int Day = 1;

    [HideInInspector] public DayPhase CurrentPhase = DayPhase.Day;

    void Awake()
    {
        Instance = this;

        blackScreen.Play("FadeOut");    
    }

    public void AdvancePhase()
    {
        if (CurrentPhase == DayPhase.Day)
        {
            // Go to night
            CurrentPhase = DayPhase.Night;

            blackScreen.Play("Night Fade");

            StartCoroutine(DelayedChanges());
        }
        else
        {
            // Transition to next day
            Day++;

            CurrentPhase = DayPhase.Day;
            StartCoroutine(PlayerStatusManager.Instance.Eat());
            DaysUntilRescue -= 1; 
            if (DaysUntilRescue == 0)            
                winScreen.SetActive(true);            

            dayText.text = "Day " + Day.ToString();

            blackScreen.Play("FadeIn");

            // These changes are delayed so the status' don't change before the black screen hides them
            StartCoroutine(DelayedChanges());
        }

        Merchant.Instance.Reset();
        CanvasManager.Instance.HideAllBoards();
    }

    IEnumerator DelayedChanges()
    {
        // Change into the night
        if (CurrentPhase == DayPhase.Night)
            yield return new WaitForSeconds(DayNightTransitionTime);        
        else
            yield return new WaitForSeconds(NightDayTransitionTime);

        // Go into the day
        if (CurrentPhase == DayPhase.Day)
        {
            //darkPanel.SetActive(false);
            
            backgrounds[0].SetActive(true);
            backgrounds[1].SetActive(false);

            Merchant.Instance.NewDay();
        }
        // Go into the night
        else
        {
            //darkPanel.SetActive(true);

            backgrounds[0].SetActive(false);
            backgrounds[1].SetActive(true);

            //ChangeVolume(true);
        }

        PlayerStatusManager.Instance.StatusDecreasePerPhase();
        PlayerStatusManager.Instance.CheckPlayerCondition();                
    }
}
