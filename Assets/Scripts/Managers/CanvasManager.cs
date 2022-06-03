using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance { get; set; }

    [SerializeField] GameObject inventoryBoard;
    [SerializeField] GameObject activitiesBoard;
    [SerializeField] GameObject expeditionRecapBoard;

    void Awake()
    {
        Instance = this;       
    }

    public void ToggleInventory()
    {
        inventoryBoard.SetActive(!inventoryBoard.activeInHierarchy);
    }    
    
    public void ToggleExpeditionRecapBoard()
    {
        expeditionRecapBoard.SetActive(!expeditionRecapBoard.activeInHierarchy);
        AudioManager.Instance.Play("CheckBoard");
    }    
    
    public void ActivateActivityBoard()
    {
        activitiesBoard.SetActive(true);
        
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
        activitiesBoard.SetActive(false);
        if (ActivityBoard.Instance != null)
            ActivityBoard.Instance.CloseActivityBoard();
    }
}
