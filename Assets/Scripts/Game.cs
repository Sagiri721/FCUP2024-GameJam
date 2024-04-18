using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{

    void Awake(){

        // Load heads up display into current scene       
        SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
    }

    void Start()
    {

        // Get the animators and UI references from loaded scene
        DialogueManager dialogues = DialogueManager.getInstance();

        Animator transitionAnimator = GameObject.Find("TransitionBlinds").GetComponent<Animator>();
        Animator dialogueAnimator = GameObject.Find("DialogueBox").GetComponent<Animator>();

        TMPro.TextMeshProUGUI nameText = GameObject.Find("NameText").GetComponent<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI bodyText = GameObject.Find("BodyText").GetComponent<TMPro.TextMeshProUGUI>();

        Transition.getInstance().animator = transitionAnimator;
        
        DialogueManager.getInstance().animator = dialogueAnimator;
        DialogueManager.getInstance().nameText = nameText;
        DialogueManager.getInstance().dialogueText = bodyText;
    }
}
