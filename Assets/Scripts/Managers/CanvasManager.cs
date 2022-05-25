using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance { get; set; }

    [SerializeField] GameObject inventoryBoard;
    [SerializeField] GameObject expeditionsBoard;
    [SerializeField] GameObject expeditionRecapBoard;
    [SerializeField] GameObject expeditionTextHolder;
    [SerializeField] GameObject[] expeditionDetails;

    void Awake()
    {
        Instance = this;       
    }

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
    
    public void ToggleExpeditionRecapBoard()
    {
        expeditionRecapBoard.SetActive(!expeditionRecapBoard.activeInHierarchy);
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

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void HideAllBoards()
    {
        inventoryBoard.SetActive(false);
        expeditionTextHolder.SetActive(false);
        expeditionsBoard.SetActive(false);
    }
}
