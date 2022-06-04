using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Settings()
    {
        animator.Play("OpenSettings");
    }
    
    public void Credits()
    {
        animator.Play("OpenCredits");
    }

    public void BackToMenuFromSettings()
    {
        animator.Play("BackToMenuFromSettings");
    }    
    
    public void BackToMenuFromCredits()
    {
        animator.Play("BackToMenuFromCredits");
    }
}
