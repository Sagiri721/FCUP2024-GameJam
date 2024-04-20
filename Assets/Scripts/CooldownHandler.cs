using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CooldownHandler : MonoBehaviour
{
    public static bool onCooldown, killTrigger = false;
    public static int killType = -1, boostCount1 = 0, boostCount2 = 0, boostCount3 = 0;
    public Animator animator;

    public void Start(){
        animator = GetComponent<Animator>();
    }

    public void LateUpdate(){
        if(killTrigger){
            killTrigger = false;
            StartCoroutine(cooldownHandler());
        }
        if(killType != -1){
            switch(killType){
                case 0: GetComponentsInChildren<TextMeshProUGUI>(true)[0].GetComponent<RectTransform>().parent.gameObject.SetActive(true);
                    boostCount1++;
                    GetComponentsInChildren<TextMeshProUGUI>(true)[0].text = "x"+boostCount1+" Strong Arms";
                    break;
                case 1: GetComponentsInChildren<TextMeshProUGUI>(true)[1].GetComponent<RectTransform>().parent.gameObject.SetActive(true);
                    boostCount2++;
                    GetComponentsInChildren<TextMeshProUGUI>(true)[1].text = "x"+boostCount2+" Long Tentacles";
                    break;
                case 2: GetComponentsInChildren<TextMeshProUGUI>(true)[2].GetComponent<RectTransform>().parent.gameObject.SetActive(true);
                    boostCount3++;
                    GetComponentsInChildren<TextMeshProUGUI>(true)[2].text = "x"+boostCount3+" Ghost Speed";
                    break;
            }
            killType = -1;
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
