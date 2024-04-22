using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public bool managerExists;

    void Awake(){

        // Load heads up display into current scene
        SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
    }

    void Start()
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
        Destroy(GameObject.FindWithTag("menuUI"));
        if(!FindObjectOfType<AudioManager>().IsCurrentlyPlaying("Level")){
            FindObjectOfType<AudioManager>().Play("Level");
        }
        
        // Get the animators and UI references from loaded scene
        DialogueManager dialogues = DialogueManager.getInstance();
        DontDestroyOnLoad(GameObject.FindWithTag("gameUI"));

        //Transition.getInstance().fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();

        DialogueManager.getInstance().animator = GameObject.Find("DialogueBox").GetComponent<Animator>();
        DialogueManager.getInstance().dialogueText = GameObject.Find("BodyText").GetComponent<TMPro.TextMeshProUGUI>();

        /*DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Transition.getInstance().gameObject);
        DontDestroyOnLoad(DialogueManager.getInstance().gameObject);*/
    }
}
