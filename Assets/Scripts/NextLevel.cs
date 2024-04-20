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
        if(GameObject.FindWithTag("Player").GetComponent<PlayerController>().snapMeDaddy){
            GameObject.FindWithTag("Player").transform.position = entryPoint;
        }
        StartCoroutine(Transition.getInstance().DoTransition(() => {}, true));
    }
    void OnTriggerEnter2D(Collider2D other){
        Transition.getInstance().fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        other.gameObject.GetComponent<PlayerController>().snapMeDaddy = true;
        StartCoroutine(Transition.getInstance().DoTransition(LoadNextLevel));
    }

    void LoadNextLevel(){
        SceneManager.LoadScene(sceneToLoad);
    }
}
