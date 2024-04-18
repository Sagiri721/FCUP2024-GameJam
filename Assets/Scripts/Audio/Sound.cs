using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public float volume;
    public float pitch;
    public bool loop;
    public string name;
    public AudioSource source;
}
