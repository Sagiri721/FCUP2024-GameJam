using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private float checkRadius = 2;
    private bool inDialogue = false;

    private Transform player;
    public DialogueObject dialogue;

    void Start(){

        // Find player
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update(){

        if (player != null && Utils.GetKeyDownAll(player.gameObject.GetComponent<PlayerController>().stats.actionKeys)){

            float distance = (player.position - transform.position).magnitude;
            if (distance < checkRadius){

                if (inDialogue){

                    // Goto next
                    inDialogue = DialogueManager.getInstance().DisplaySentence();

                }else{

                    // Start dialogue
                    inDialogue = true;

                    StartDialogue();
                    DialogueManager.getInstance().DisplaySentence();
                }

            }
        }

        // Cancel dialogue
        if (inDialogue){

            // Try to see if player outside dialogue range
            float distance = (player.position - transform.position).magnitude;
            if (distance > checkRadius){

                inDialogue = false;
                DialogueManager.getInstance().ClearScreen();
            }
        }
    }

    void StartDialogue(){

        DialogueManager manager = DialogueManager.getInstance();

        // Dialogue manager non existance
        if (manager == null) {
            Debug.LogError("Something went terribly wrong\nThe dialogue manager isn't instantiated yet");
            return;
        }

        manager.Initialize(dialogue);
    }
}
