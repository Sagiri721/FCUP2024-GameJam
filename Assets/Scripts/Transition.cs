using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public Animator animator;
    private static Transition instance = null;

    void Awake(){

        if (instance == null){
            instance = this;
        }
    }

    public static Transition getInstance(){ return instance; }

    public IEnumerator DoTransition(System.Action callback){

        animator.SetBool("isOpen", false);
        // Wait for transition to end
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);

        // Do the callback
        callback();

        animator.SetBool("isOpen", true);
    }
}
