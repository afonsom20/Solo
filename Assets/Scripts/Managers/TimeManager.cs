using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; set; }

    [Header("UI")]
    [SerializeField] Animator blackScreen;
    [SerializeField] TextMeshProUGUI dayText;
    //[SerializeField] GameObject[] apartmentBackgrounds; // 0 - morning; 1 - evening
    [SerializeField] GameObject darkPanel;

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

            PlayerStatusManager.Instance.Eat();

            dayText.text = "Day " + Day.ToString();

            blackScreen.Play("FadeIn");

            // These changes are delayed so the status' don't change before the black screen hides them
            StartCoroutine(DelayedChanges());
        }
    }

    IEnumerator DelayedChanges()
    {
        // Change into the night
        if (CurrentPhase == DayPhase.Night)
            yield return new WaitForSeconds(1.5f);        
        else
            yield return new WaitForSeconds(4f);

        // Go into the day
        if (CurrentPhase == DayPhase.Day)
        {
            //apartmentBackgrounds[0].SetActive(true);
            //apartmentBackgrounds[1].SetActive(false);
            darkPanel.SetActive(false);

            //ChangeVolume(false);
        }
        // Go into the night
        else
        {
            //apartmentBackgrounds[0].SetActive(false);
            //apartmentBackgrounds[1].SetActive(true);

            darkPanel.SetActive(true);

            //ChangeVolume(true);
        }

        PlayerStatusManager.Instance.StatusDecreasePerPhase();
        PlayerStatusManager.Instance.CheckPlayerCondition();        
    }

    void ChangeVolume(bool changeToNight)
    {
    
    }
}
