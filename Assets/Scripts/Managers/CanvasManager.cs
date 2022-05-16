using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] GameObject inventoryBoard;
    [SerializeField] GameObject expeditionsBoard;
    [SerializeField] GameObject expeditionTextHolder;
    [SerializeField] GameObject[] expeditionDetails;

    public void ToggleExpeditionDetails(int expeditionNumber)
    {
        expeditionTextHolder.SetActive(!expeditionTextHolder.activeInHierarchy);
        expeditionDetails[expeditionNumber].SetActive(!expeditionDetails[expeditionNumber].activeInHierarchy);

        AudioManager.Instance.Play("ExpeditionDetails");
    }

    public void ToggleInventory()
    {
        inventoryBoard.SetActive(!inventoryBoard.activeInHierarchy);
        AudioManager.Instance.Play("CheckBoard");
    }    
    
    public void ToggleExpeditionBoard()
    {

        for (int i = 0; i < expeditionDetails.Length; i++)
        {
            expeditionDetails[i].SetActive(false);
        }

        expeditionTextHolder.SetActive(true);

        expeditionsBoard.SetActive(!expeditionsBoard.activeInHierarchy);
        
        AudioManager.Instance.Play("CheckBoard");
    }
}
