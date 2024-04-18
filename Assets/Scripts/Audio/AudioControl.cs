using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioControl : MonoBehaviour
{
    public static bool muted = false;
    public Sprite[] sprites;

    public void OnEnable()
    {
        if (muted)
        {
            GetComponent<Image>().sprite = sprites[1];
        }
        else
        {
            GetComponent<Image>().sprite = sprites[0];
        }
    }
    public void Click()
    {
        if (muted)
        {
            FindObjectOfType<AudioManager>().Unmute();
            GetComponent<Image>().sprite = sprites[0];
        }
        else
        {
            FindObjectOfType<AudioManager>().Mute();
            GetComponent<Image>().sprite = sprites[1];
        }
        muted = !muted;
    }
}
