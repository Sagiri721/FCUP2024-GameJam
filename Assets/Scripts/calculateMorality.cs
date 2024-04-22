using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class calculateMorality : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown){
            if(CooldownHandler.boostCount1 == 0 && CooldownHandler.boostCount2 == 0 && CooldownHandler.boostCount3 == 0){
                SceneManager.LoadScene("good end");
            }else{
                SceneManager.LoadScene("Bad ending");
            }
        }
    }
}
