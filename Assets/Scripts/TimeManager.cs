using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; set; }

    [SerializeField] Animator blackScreen;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] GameObject[] apartmentBackgrounds; // 0 - morning; 1 - evening

    public enum DayPhase { Morning, Evening };

    [HideInInspector] public int Day = 1;

    [HideInInspector] public DayPhase CurrentPhase = DayPhase.Morning;

    void Awake()
    {
        Instance = this;

        blackScreen.Play("FadeOut");    
    }

    public void AdvancePhase()
    {
        if (CurrentPhase == DayPhase.Morning)
        {
            // Go to evening
            CurrentPhase = DayPhase.Evening;

            blackScreen.Play("Night Fade");

            StartCoroutine(DelayedChanges());
        }
        else
        {
            // Transition to next day
            Day++;

            CurrentPhase = DayPhase.Morning;

            PlayerStatusManager.Instance.Eat();

            dayText.text = "Day " + Day.ToString();

            blackScreen.Play("FadeIn");

            // These changes are delayed so the status' don't change before the black screen hides them
            StartCoroutine(DelayedChanges());
        }
    }

    IEnumerator DelayedChanges()
    {
        yield return new WaitForSeconds(1.5f);

        if (CurrentPhase == DayPhase.Morning)
        {
            apartmentBackgrounds[0].SetActive(true);
            apartmentBackgrounds[1].SetActive(false);
        }
        else
        {
            apartmentBackgrounds[0].SetActive(false);
            apartmentBackgrounds[1].SetActive(true);
        }

        PlayerStatusManager.Instance.StatusDecreasePerPhase();
        PlayerStatusManager.Instance.CheckPlayerCondition();        
    }
}
