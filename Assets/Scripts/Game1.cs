using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game1 : MonoBehaviour
{

    void Awake(){
        
        // Load heads up display into current scene       
        SceneManager.LoadScene("HUD 1", LoadSceneMode.Additive);
    }

    void Start()
    {

        // Get the animators and UI references from loaded scene
        Transition.getInstance().fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
        DontDestroyOnLoad(GameObject.FindWithTag("menuUI"));
    }
}
