using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailsButton : MonoBehaviour
{
    public void ShowActivityDetails()
    {
        int index = transform.GetSiblingIndex();

        ActivityBoard.Instance.ShowActivityDescription(index);
    }

    public void PlayDrawSound()
    {
        AudioManager.Instance.PlayRandomPitch("Draw", 0.9f, 1.2f);
    }
}
