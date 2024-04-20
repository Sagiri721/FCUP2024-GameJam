using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroContinue : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown){
            FindObjectOfType<AudioManager>().StopAllSound();
            StartCoroutine(Transition.getInstance().DoTransition(ChangeScene));
        }
    }

    void ChangeScene(){
        SceneManager.LoadScene("Floor1");
    }
}
