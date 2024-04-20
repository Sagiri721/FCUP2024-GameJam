using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 0.4f;
    private static Transition instance = null;

    void Awake(){

        if (instance == null){
            instance = this;
        }
    }

    public static Transition getInstance(){ return instance; }

    public IEnumerator DoTransition(System.Action callback, bool onlyDetransition = false){

        float alpha = 0;
        float fadeEndValue = 1;

        if(!onlyDetransition){
            fadeImage.enabled = true;
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, true);
                yield return null;
            }
        }

        callback();

        alpha = 1;
        fadeEndValue = 0;

        while (alpha >= fadeEndValue)
        {
            SetColorImage(ref alpha, false);
            yield return null;
        }
        fadeImage.enabled = false;
    }

    private void SetColorImage(ref float alpha, bool fadeDirection)
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == false) ? -1 : 1);
    }
}
