using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static bool managerExists;
    void Awake()
    {
        if (!managerExists)
        {
            managerExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.loop = sound.loop;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.playOnAwake = false;
        }
    }

    public bool IsCurrentlyPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { return false; }
        return s.source.isPlaying;
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { return; }
        s.source.Play();
    }
    public void StopAny(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { return; }
        s.source.Stop();
    }
    public void StopBackground()
    {
        Sound s = Array.Find(sounds, sound => sound.source.isPlaying && sound.source.loop);
        if (s == null) { return; }
        s.source.Stop();
    }
    public IEnumerator FadeBackground(float duration)
    {
        /*if (AudioControl.muted)
        {
            StopBackground();
            yield break;
        }*/
        Sound s = Array.Find(sounds, sound => sound.source.isPlaying && sound.source.loop);
        if (s == null) { yield break; }
        float currentTime = 0;
        float start = s.source.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }
        s.source.volume = start;
        Debug.Log("Stopping");
        s.source.Stop();
    }

    public IEnumerator FadeAny(string name, float duration){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) { yield break; }
        float currentTime = 0;
        float start = s.source.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            s.source.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }
        s.source.volume = s.volume;
        s.source.Stop();
    }

    public void StopAllSound()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.source.isPlaying) { sound.source.Stop(); }
        }
    }

    public void Mute()
    {
        foreach (Sound sound in sounds)
        {
            sound.source.volume = 0;
        }
    }

    public void Unmute()
    {
        foreach (Sound sound in sounds)
        {
            sound.source.volume = sound.volume;
        }
    }
}
