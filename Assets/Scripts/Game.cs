using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{

    public Image hungerBar;
    private float hungerSize;
    private PlayerController playerStats;

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

        hungerBar = GameObject.Find("bar").GetComponent<Image>();

        playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        hungerSize = hungerBar.rectTransform.sizeDelta.y;
    }

    void Update(){

        // Hunger display update
        //hungerBar.rectTransform.sizeDelta = new Vector2(hungerBar.rectTransform.sizeDelta.x, hungerSize * playerStats.stats.currentHunger);
    }
}
