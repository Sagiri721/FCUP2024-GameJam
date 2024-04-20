using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    public void Exit(){

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void Credits(){
        SceneManager.LoadScene("credits");
    }

    public void GoBack(){

        
        SceneManager.LoadScene("Menu");
    }

    public void StartGame(){

        System.Action dg = () => {
            SceneManager.LoadScene("Intro");
        };

        StartCoroutine(Transition.getInstance().DoTransition(dg));
    }
}
