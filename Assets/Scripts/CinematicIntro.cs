using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CinematicIntro : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;

    void Start()
    {
        InvokeRepeating("CheckIfVideoFinished", 1f, 1f);
    }

    void CheckIfVideoFinished()
    {
        if (!videoPlayer.isPlaying)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
