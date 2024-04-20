using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownHandler : MonoBehaviour
{
    public static bool onCooldown, killTrigger = false;
    public Animator animator;

    public void Start(){
        animator = GetComponent<Animator>();
    }

    public void LateUpdate(){
        if(killTrigger){
            killTrigger = false;
            StartCoroutine(cooldownHandler());
        }
    }

    public IEnumerator cooldownHandler(){
        onCooldown = true;
        animator.Play("cooldownRegen");
        while(animator.GetCurrentAnimatorStateInfo(0).IsName("cooldownRegen") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f){
            yield return null;
        }
        onCooldown = false;
    }
}
