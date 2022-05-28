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
}
