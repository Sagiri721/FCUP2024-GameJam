using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // Ui elements
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI dialogueText;

    public Animator animator;

    // Singleton instance
    private static DialogueManager instance = null;

    // Sentences to display
    public Queue<string> dialogues;

    // Get current instance
    public static DialogueManager getInstance(){ return instance; }

    void Start()
    {
        if (getInstance() == null){

            instance = this;

            // Instantiate like normal
            dialogues = new Queue<string>();

        }else{

            Debug.LogError("Cannot create another instance of a singleton class");
        }
    }

    public void Initialize(DialogueObject dialogueToDraw){

        animator.SetBool("isOpen", true);

        // Initialize dialogue
        this.dialogues = new Queue<string>(dialogueToDraw.text);
        this.name = dialogueToDraw.name;

        nameText.text = this.name;

        // Show textbox
    }

    public void ClearScreen(){

        dialogues.Clear();
        
        // Clear textbox
        animator.SetBool("isOpen", false);
    }

    public bool DisplaySentence() {

        if (dialogues.Count == 0){

            ClearScreen();
            return false;
        }

        string sentence = dialogues.Dequeue();
        
        // Update UI
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        return true;
    }

    IEnumerator TypeSentence(string sentence) {

        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray()){

            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
