using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Cinematic : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] int sceneToLoad = 0;

    void Start()
    {
        InvokeRepeating("CheckIfVideoFinished", 1f, 1f);
    }

    void CheckIfVideoFinished()
    {
        if (!videoPlayer.isPlaying)
            SceneManager.LoadScene(sceneToLoad);
    }

    public void Skip()
    {
        videoPlayer.Stop();
        SceneManager.LoadScene(sceneToLoad);
    }
}
