using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    // Static instance desta classe, para poder ser referenciada em qualquer local
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.group;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // Reset pitch in case this sound has been played with the PlayRandomPitch() method before
        s.source.pitch = 1;

        if (s.source.isPlaying == false)
            s.source.Play();
    }

    public void PlayRandomPitch(string name, float minPitch, float maxPitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        float pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        s.source.pitch = pitch;

        if (s.source.isPlaying == false)
            s.source.Play();
    }

    public void StopAllSound()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.Stop();
        }
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (s.source.isPlaying)
            s.source.Stop();        
    }

    public void PauseUnpauseSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (s.source.isPlaying)
            s.source.Pause();
        else
            s.source.Play();
    }

    public bool IsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s.source.isPlaying)
            return true;
        else
            return false;
    }

    public AudioClip FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }

        return s.clip;
    }

    public void FadeCoroutine(bool fadeIn, string name, float duration, float targetVolume)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(fadeIn, name, duration, targetVolume));
    }

    IEnumerator Fade(bool fadeIn, string name, float duration, float targetVolume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        float currentTime = 0;
        float start = s.source.volume;

        if (!s.source.isPlaying)
        {
            s.source.Play();
            //Debug.Log("Fading In");
        }

        while (currentTime < duration)
        {            
            currentTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

}
