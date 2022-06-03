using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    [SerializeField, Tooltip("0 - Health, 1 - Energy, 2 - Hygiene, 3 - Hope")] int statusIndex;
    [SerializeField] Slider slider;
    [SerializeField] Sprite[] icons;

    int lowThreshold;
    int mediumThreshold;
    Image iconImage;

    void Start()
    {
        iconImage = GetComponent<Image>();

        slider.onValueChanged.AddListener(delegate { CheckForIconChange(); });

        // Set the medium threshold, which is 66 for everything except Hygiene
        mediumThreshold = 66;

        switch (statusIndex)
        {
            case 0: lowThreshold = PlayerStatusManager.Instance.LowHealthThreshold;
                break;
            case 1: lowThreshold = PlayerStatusManager.Instance.LowEnergyThreshold;
                break;
            case 2: lowThreshold = PlayerStatusManager.Instance.LowHygieneThreshold;
                mediumThreshold = 33; // change the medium threshold if this is the Hygiene status icon
                break;
            case 3: lowThreshold = PlayerStatusManager.Instance.LowHopeThreshold;
                break;
        }

        CheckForIconChange();
       
    }

    void CheckForIconChange()
    {
        // If this status meter is too low (i.e. damages player) set the icon to the "low status" sprite
        if (slider.value <= lowThreshold)
            iconImage.sprite = icons[0];
        // If the status meter is below 2/3 of the max, set it to the intermediate icon
        else if (slider.value <= mediumThreshold)
            iconImage.sprite = icons[1];
        // If not, set it to the "normal status" sprite
        else
        {
            iconImage.sprite = icons[2];
        }
    }
}
