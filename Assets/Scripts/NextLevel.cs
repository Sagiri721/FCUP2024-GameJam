using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public Vector3 entryPoint;
    public string sceneToLoad;
    void Start(){
        GameObject.FindWithTag("Player").transform.position = entryPoint;
        StartCoroutine(Transition.getInstance().DoTransition(() => {}, true));
    }
    void OnTriggerEnter2D(Collider2D other){
        Transition.getInstance().fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        StartCoroutine(Transition.getInstance().DoTransition(LoadNextLevel));
    }

    void LoadNextLevel(){
        if(sceneToLoad == "potion"){
            FindObjectOfType<AudioManager>().StopAllSound();
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}
