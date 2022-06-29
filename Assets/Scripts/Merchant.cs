using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Merchant : MonoBehaviour
{
    [SerializeField] GameObject graphics;
    [SerializeField] GameObject doorInteractable;
    [SerializeField] int minimumDaysUntilVisit = 7;
    [SerializeField] int maximumDaysUntilVisit = 12;

    [Space, Header("Dialogue")]
    [SerializeField] TextMeshProUGUI dialogueBox;
    [SerializeField] float typingSpeed = 0.02f;
    [SerializeField] string firstGreeting;
    [SerializeField] string generalGreeting;
    [SerializeField] string rejectedDialogue;

    bool isFirstVisit = true;
    int nextVisit;
    //int daysSinceLastVisit;

    void Start()
    {
        DecideNextVisit();
    }

    void DecideNextVisit()
    {
        nextVisit = Random.Range(minimumDaysUntilVisit, maximumDaysUntilVisit);
    }

    public void NewDay()
    {
        nextVisit -= 1;

        // If the time for the visit has arrived
        if (nextVisit == 0)
        {
            doorInteractable.SetActive(true);
            AudioManager.Instance.Play("Door Knock");
            InformationManager.Instance.SendInfo(5, "There's someone at the door");            
            DecideNextVisit();
        }
    }

    public void OpenDoor()
    {
        doorInteractable.SetActive(false);
        graphics.SetActive(true);
        doorInteractable.GetComponent<OutlineToggler>().ToggleOutline(); // if we don't do this, next time the outline will be on when the mouse is not on top of the door, which is wrong
        AudioManager.Instance.Play("Open Door");

        if (isFirstVisit)
        {
            StartCoroutine(Type(firstGreeting, false));
            isFirstVisit = false;
        }
        else
            StartCoroutine(Type(generalGreeting, false));
    }

    public void Rejected()
    {
        StartCoroutine(Type(rejectedDialogue, true));        
    }

    IEnumerator Type(string text, bool endDialogue)
    {
        StopCoroutine("Type");
        dialogueBox.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (endDialogue)
        {
            yield return new WaitForSeconds(2f);
            AudioManager.Instance.Play("Close Door"); 
            graphics.SetActive(false);
        }
    }
}
