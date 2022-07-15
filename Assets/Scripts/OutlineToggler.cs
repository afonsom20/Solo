using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class OutlineToggler : MonoBehaviour
{
    [SerializeField] bool manualBorders = false;
    [ShowIf("manualBorders"), SerializeField] Vector2 topLeft;
    [ShowIf("manualBorders"), SerializeField] Vector2 bottomRight;
    [ShowIf("manualBorders"), SerializeField] Vector2 borderResolution;

    Image image;
    bool insideBorder = false;
    AudioSource audioSource;

    void Start()
    {
        image = GetComponent<Image>();

        if (manualBorders)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // If the borders should be checked manually and not with the EventSystem (OnPointerEnter/Exit)
        if (manualBorders)
        {
            // If the cursor wasn't inside the borders on the last frame
            if (!insideBorder)
            {
                // Check if it is now inside the borders
                if (MouseInsideManualBorders())
                {
                    // If so, activate the outline
                    ToggleOutline();
                    audioSource.Play();
                    insideBorder = true;
                }
            }
            // If the cursor was inside the borders on the last frame
            else
            {
                // Check if it left the borders this frame
                if (!MouseInsideManualBorders())
                {
                    // If so, deactivate the outline
                    ToggleOutline();
                    insideBorder = false;
                }
            }
        }    
    }

    public void ToggleOutline()
    {
        Color color = image.color;

        if (color.a == 0)
            image.color = new Color (color.r, color.g, color.b, 1);
        else
            image.color = new Color(color.r, color.g, color.b, 0);
    }

    bool MouseInsideManualBorders()
    {
        // These calculations are made to make sure that the detection is resolution independent - the borders are set when the resolution is 4K, 
        // but if we take these absolute values they will only work when the resolution is 4K, since Input.mousePosition is resolution-dependent
        float mouseRatioX = Input.mousePosition.x / Screen.width;
        float mouseRatioY = Input.mousePosition.y / Screen.height;
        Vector2 topLeftRatio = topLeft / borderResolution;
        Vector2 bottomRightRatio = bottomRight / borderResolution;

        //Debug.Log(mouseRatioX + "; " + topLeftRatio.x + ", " + bottomRightRatio.x);

        if ((mouseRatioX >= topLeftRatio.x) && (mouseRatioX <= bottomRightRatio.x) &&
            (mouseRatioY <= topLeftRatio.y) && (mouseRatioY >= bottomRightRatio.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
